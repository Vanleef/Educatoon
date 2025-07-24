using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QuestionEditorUI : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown typeDropdown;
    [SerializeField] private TMP_InputField statementInput;
    [SerializeField] private Button importMediaBtn;
    [SerializeField] private Transform answersParent;
    [SerializeField] private GameObject answerPrefab;
    [SerializeField] private Button addAnswerBtn;

    private string mediaPath;
    private List<AnswerUI> answerUIs = new List<AnswerUI>();

    private void Start()
    {
        typeDropdown.onValueChanged.AddListener(OnTypeChanged);
        addAnswerBtn.onClick.AddListener(AddAnswer);
        OnTypeChanged(typeDropdown.value);
        AddAnswer(); // Começa com pelo menos uma resposta
        AddAnswer();
    }

    void OnTypeChanged(int index)
    {
        // 0=Texto, 1=Imagem, 2=Áudio, 3=Vídeo
        importMediaBtn.gameObject.SetActive(index == 1 || index == 2 || index == 3);
    }

    public void SetMediaPath(string path)
    {
        mediaPath = path;
        // Atualize a UI se quiser mostrar o nome do arquivo, etc.
    }

    void AddAnswer()
    {
        var go = Instantiate(answerPrefab, answersParent);
        var answerUI = go.GetComponent<AnswerUI>();
        answerUI.Init(this);
        answerUIs.Add(answerUI);
    }

    public void RemoveAnswer(AnswerUI answer)
    {
        answerUIs.Remove(answer);
        Destroy(answer.gameObject);
    }

    public Question GetQuestion()
    {
        Question q = new Question();
        q.questionInfo = statementInput.text;
        q.questionType = (QuestionType)typeDropdown.value;
        q.options = new List<string>();

        // Em vez de usar uma string para a resposta correta, usamos o índice correto
        q.correctAnswerIndex = 0; // valor padrão

        int idx = 0;
        foreach (var ans in answerUIs)
        {
            q.options.Add(ans.GetText());
            if (ans.IsCorrect())
            {
                q.correctAnswerIndex = idx;
            }
            idx++;
        }

        // Carregue a mídia, se necessário.
        // Exemplo:
        // if (q.questionType == QuestionType.Image)
        //     q.questionImage = Resources.Load<Sprite>(mediaPath);
        // if (q.questionType == QuestionType.Audio)
        //     q.audioClip = Resources.Load<AudioClip>(mediaPath);
        // if (q.questionType == QuestionType.Video)
        //     q.videoClip = Resources.Load<VideoClip>(mediaPath);

        return q;
    }
}