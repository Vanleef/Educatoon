using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using TMPro;
using UnityEngine.UIElements;
using UIButton = UnityEngine.UI.Button;     // Alias para Unity UI Button
using UIImage = UnityEngine.UI.Image;       // Alias para Unity UI Image

public class QuizGameUI : MonoBehaviour
{
#pragma warning disable 649
    [SerializeField] private QuizManager quizManager;
    [SerializeField] private CategoryBtnScript categoryBtnPrefab;
    [SerializeField] private GameObject scrollHolder;
    [SerializeField] private TMP_Text scoreText, timerText;
    [SerializeField] private List<UIImage> lifeImageList;
    [SerializeField] private GameObject gameOverPanel, mainMenu, gamePanel;
    [SerializeField] private Color correctCol, wrongCol, normalCol;
    [SerializeField] private UIImage questionImg;
    [SerializeField] private UnityEngine.Video.VideoPlayer questionVideo;
    [SerializeField] private AudioSource questionAudio;
    [SerializeField] private TMP_Text questionInfoText;
    [SerializeField] private List<UIButton> options;

    // Botões para controle de mídia
    [SerializeField] private UIButton replayMediaButton;
    [SerializeField] private UIButton stopMediaButton;

    [SerializeField] private UIButton openEditorBtn;
    [SerializeField] private QuizEditorController quizEditor;
#pragma warning restore 649

    private float audioLength;
    private Question question;
    private bool answered = false;
    private Coroutine audioCoroutine;

    public TMP_Text TimerText { get => timerText; }
    public TMP_Text ScoreText { get => scoreText; }
    public GameObject GameOverPanel { get => gameOverPanel; }

    [Header("UI Document")]
    [SerializeField] private UIDocument uiDocument;

    [Header("References")]
    [SerializeField] private QuizEditorController quizEditorController;

    [Header("Teacher Settings")]
    [SerializeField] private bool isTeacher = false;

    private VisualElement rootContainer;
    private UnityEngine.UIElements.Button editorButton; // CORRIGIDO: Especificar explicitamente UIElements.Button

    public bool IsTeacher => isTeacher;

    private void Start()
    {
        // 1. Primeiro configurar UI Elements se disponível
        SetupUI();

        // 2. Configurar modo professor
        if (quizEditor != null)
        {
            quizEditor.SetTeacherMode(isTeacher);
            Debug.Log($"QuizGameUI: Configurando modo professor como {isTeacher}");
        }

        if (quizEditorController != null)
        {
            quizEditorController.SetTeacherMode(isTeacher);
            Debug.Log($"QuizGameUI: Configurando QuizEditorController modo professor como {isTeacher}");
        }

        // 3. Configurar botão do editor (tanto UIElements quanto Unity UI)
        ConfigureEditorButton();

        // 4. Configurar listeners dos botões de opção
        for (int i = 0; i < options.Count; i++)
        {
            UIButton localBtn = options[i];
            localBtn.onClick.AddListener(() => OnClick(localBtn));
        }

        // 5. Configurar botões de mídia
        if (replayMediaButton != null)
            replayMediaButton.onClick.AddListener(ReplayQuestionMedia);

        if (stopMediaButton != null)
            stopMediaButton.onClick.AddListener(StopQuestionMedia);

        // 6. Configurar botão do editor Unity UI (fallback)
        if (openEditorBtn != null)
            openEditorBtn.onClick.AddListener(OpenQuizEditor);

        // 7. Criar botões de categoria
        CreateCategoryButtons();
    }

    private void OpenQuizEditor()
    {
        Debug.Log($"QuizGameUI: OpenQuizEditor chamado. IsTeacher = {isTeacher}");

        if (quizEditor != null)
        {
            // NOVO: Garantir que o modo professor esteja sincronizado
            quizEditor.SetTeacherMode(isTeacher);
            quizEditor.OpenEditor();
        }
        else
        {
            Debug.LogError("QuizGameUI: quizEditor é null!");
        }
    }

    private void SetupUI()
    {
        if (uiDocument != null)
        {
            rootContainer = uiDocument.rootVisualElement;

            // CORRIGIDO: Verificar se rootContainer não é null antes de usar Q<>
            if (rootContainer != null)
            {
                editorButton = rootContainer.Q<UnityEngine.UIElements.Button>("EditorButton");

                if (editorButton != null)
                {
                    Debug.Log("QuizGameUI: EditorButton encontrado no UIDocument");
                }
                else
                {
                    Debug.LogWarning("QuizGameUI: EditorButton não encontrado no UIDocument. Certifique-se de que existe um Button com name='EditorButton' no UXML.");
                }
            }
            else
            {
                Debug.LogError("QuizGameUI: uiDocument.rootVisualElement é null. Verifique se o UIDocument está configurado corretamente.");
            }
        }
        else
        {
            Debug.LogWarning("QuizGameUI: uiDocument é null. O botão do editor não será configurado via UIElements.");
        }
    }

    private void ConfigureEditorButton()
    {
        if (editorButton != null)
        {
            editorButton.style.display = isTeacher ? DisplayStyle.Flex : DisplayStyle.None;

            // CORRIGIDO: Remover listener anterior antes de adicionar novo
            editorButton.clicked -= OnEditorButtonClicked;
            editorButton.clicked += OnEditorButtonClicked;

            Debug.Log($"QuizGameUI: Botão editor configurado. Visível: {isTeacher}");
        }
        else
        {
            Debug.LogWarning("QuizGameUI: editorButton é null. Não foi possível configurar o botão do editor.");
        }
    }

    private void OnEditorButtonClicked()
    {
        if (isTeacher && quizEditor != null)
        {
            quizEditor.OpenEditor();
        }
        else
        {
            Debug.LogWarning("Acesso negado ao editor de fases.");
        }
    }

    private void ResetAllButtonColors()
    {
        for (int i = 0; i < options.Count; i++)
        {
            options[i].image.color = normalCol;
        }
    }

    public void SetQuestion(Question q)
    {
        question = q;
        answered = false;

        questionInfoText.text = q.questionInfo;

        if (audioCoroutine != null)
        {
            StopCoroutine(audioCoroutine);
            audioCoroutine = null;
        }

        ResetAllButtonColors();

        bool hasMedia = (!string.IsNullOrEmpty(q.imagePath) && q.questionType == QuestionType.Image) ||
                        (!string.IsNullOrEmpty(q.audioPath) && q.questionType == QuestionType.Audio) ||
                        (!string.IsNullOrEmpty(q.videoPath) && q.questionType == QuestionType.Video);

        Transform imageHolder = questionImg.transform.parent;
        imageHolder.gameObject.SetActive(hasMedia);

        questionImg.gameObject.SetActive(q.questionType == QuestionType.Image && !string.IsNullOrEmpty(q.imagePath));
        questionVideo.gameObject.SetActive(q.questionType == QuestionType.Video && !string.IsNullOrEmpty(q.videoPath));
        questionAudio.gameObject.SetActive(q.questionType == QuestionType.Audio && !string.IsNullOrEmpty(q.audioPath));

        if (q.questionType == QuestionType.Image && !string.IsNullOrEmpty(q.imagePath))
        {
            Sprite sprite = MediaLoader.LoadSprite(q.imagePath);
            if (sprite != null)
            {
                questionImg.sprite = sprite;
            }
        }

        if (q.questionType == QuestionType.Audio && !string.IsNullOrEmpty(q.audioPath))
        {
            AudioClip clip = MediaLoader.LoadAudio(q.audioPath);
            if (clip != null)
            {
                questionAudio.clip = clip;
                audioLength = clip.length;
                questionAudio.enabled = true;

                if (questionAudio.gameObject.activeInHierarchy)
                {
                    questionAudio.Play();
                    audioCoroutine = StartCoroutine(PlayAudio());
                }
            }
        }

        if (q.questionType == QuestionType.Video && !string.IsNullOrEmpty(q.videoPath))
        {
            VideoClip clip = MediaLoader.LoadVideo(q.videoPath);
            if (clip != null)
            {
                questionVideo.clip = clip;
                questionVideo.loopPointReached += OnVideoEnd;
                questionVideo.Play();
            }
        }

        for (int i = 0; i < options.Count; i++)
        {
            if (i < q.options.Count)
            {
                options[i].gameObject.SetActive(true);
                options[i].name = q.options[i];
                options[i].GetComponentInChildren<TMP_Text>().text = q.options[i];
                options[i].image.color = normalCol;
            }
            else
            {
                options[i].gameObject.SetActive(false);
            }
        }

        UpdateMediaButtonsVisibility();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        UpdateMediaButtonsVisibility();
    }

    private void UpdateMediaButtonsVisibility()
    {
        if (question == null) return;

        bool isMediaPlaying = false;

        if (question.questionType == QuestionType.Audio && questionAudio != null && questionAudio.isPlaying)
        {
            isMediaPlaying = true;
        }
        else if (question.questionType == QuestionType.Video && questionVideo != null && questionVideo.isPlaying)
        {
            isMediaPlaying = true;
        }

        bool hasPlayableMedia = (question.questionType == QuestionType.Audio && !string.IsNullOrEmpty(question.audioPath)) ||
                               (question.questionType == QuestionType.Video && !string.IsNullOrEmpty(question.videoPath));

        if (replayMediaButton != null)
        {
            replayMediaButton.gameObject.SetActive(hasPlayableMedia && !isMediaPlaying);
        }

        if (stopMediaButton != null)
        {
            stopMediaButton.gameObject.SetActive(hasPlayableMedia && isMediaPlaying);
        }
    }

    public void ReduceLife(int remainingLife)
    {
        if (remainingLife >= 0 && remainingLife < lifeImageList.Count)
        {
            lifeImageList[remainingLife].color = Color.red;
        }
    }

    IEnumerator PlayAudio()
    {
        while (question != null && question.questionType == QuestionType.Audio &&
               !string.IsNullOrEmpty(question.audioPath) && questionAudio.clip != null)
        {
            yield return new WaitForSeconds(audioLength + 0.5f);

            if (question.questionType == QuestionType.Audio &&
                questionAudio.gameObject.activeInHierarchy &&
                questionAudio.enabled)
            {
                questionAudio.Play();
            }
            else
            {
                break;
            }
        }
    }

    void OnClick(UIButton btn)
    {
        if (quizManager.GameStatus == GameStatus.PLAYING && !answered)
        {
            answered = true;
            bool val = quizManager.Answer(btn.name);

            if (val)
            {
                btn.image.color = correctCol;
                StartCoroutine(BlinkImg(btn.image));
            }
            else
            {
                btn.image.color = wrongCol;

                for (int i = 0; i < options.Count; i++)
                {
                    if (options[i].name == question.options[question.correctAnswerIndex])
                    {
                        options[i].image.color = correctCol;
                        break;
                    }
                }

                StartCoroutine(BlinkImg(btn.image));
            }

            StartCoroutine(ResetButtonColors());
        }
    }

    IEnumerator ResetButtonColors()
    {
        yield return new WaitForSeconds(2f);
        ResetAllButtonColors();
    }

    private void CategoryBtn(int index, string category)
    {
        quizManager.StartGame(index, category);
        mainMenu.SetActive(false);
        gamePanel.SetActive(true);
    }

    IEnumerator BlinkImg(UIImage img)
    {
        Color originalColor = img.color;

        for (int i = 0; i < 3; i++)
        {
            img.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            img.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void RestryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void CreateCategoryButtons()
    {
        foreach (Transform child in scrollHolder.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < quizManager.AllQuizDatas.Count; i++)
        {
            CategoryBtnScript categoryBtn = Instantiate(categoryBtnPrefab, scrollHolder.transform);
            categoryBtn.SetButton(quizManager.AllQuizDatas[i].categoryName, quizManager.AllQuizDatas[i].questions.Count);
            int index = i;
            categoryBtn.Btn.onClick.AddListener(() => CategoryBtn(index, quizManager.AllQuizDatas[index].categoryName));
        }
    }

    public void ReplayQuestionMedia()
    {
        if (question == null) return;

        if (question.questionType == QuestionType.Audio && !string.IsNullOrEmpty(question.audioPath))
        {
            if (questionAudio.clip != null && questionAudio.gameObject.activeInHierarchy)
            {
                questionAudio.Stop();

                if (audioCoroutine != null)
                {
                    StopCoroutine(audioCoroutine);
                    audioCoroutine = null;
                }

                questionAudio.Play();
                audioCoroutine = StartCoroutine(PlayAudio());
            }
        }
        else if (question.questionType == QuestionType.Video && !string.IsNullOrEmpty(question.videoPath))
        {
            if (questionVideo.clip != null && questionVideo.gameObject.activeInHierarchy)
            {
                questionVideo.Stop();
                questionVideo.Play();
            }
        }

        UpdateMediaButtonsVisibility();
    }

    public void StopQuestionMedia()
    {
        if (questionAudio != null && questionAudio.isPlaying)
        {
            questionAudio.Stop();
        }

        if (audioCoroutine != null)
        {
            StopCoroutine(audioCoroutine);
            audioCoroutine = null;
        }

        if (questionVideo != null && questionVideo.isPlaying)
        {
            questionVideo.Stop();
        }

        UpdateMediaButtonsVisibility();
    }

    public void RefreshCategoryButtons()
    {
        foreach (Transform child in scrollHolder.transform)
        {
            Destroy(child.gameObject);
        }

        CreateCategoryButtons();
    }

    private void Update()
    {
        if (question != null)
        {
            UpdateMediaButtonsVisibility();
        }
    }

    // MELHORADO: Método SetTeacherMode com logs
    public void SetTeacherMode(bool teacherMode)
    {
        isTeacher = teacherMode;
        Debug.Log($"QuizGameUI: SetTeacherMode chamado com {teacherMode}");

        ConfigureEditorButton();

        if (quizEditorController != null)
        {
            quizEditorController.SetTeacherMode(teacherMode);
        }

        if (quizEditor != null)
        {
            quizEditor.SetTeacherMode(teacherMode);
        }
    }
}