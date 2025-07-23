using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using SFB;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelEditorUI : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown miniGameDropdown;
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private Button addQuestionBtn;
    [SerializeField] private Transform questionsListParent;
    [SerializeField] private Button saveBtn;
    [SerializeField] private GameObject questionEditorPrefab;


    private List<QuestionEditorUI> questionEditors = new List<QuestionEditorUI>();

    private void Start()
    {
        miniGameDropdown.onValueChanged.AddListener(OnMiniGameChanged);
        addQuestionBtn.onClick.AddListener(AddQuestion);
        saveBtn.onClick.AddListener(SaveQuiz);
        OnMiniGameChanged(miniGameDropdown.value);
    }

    void OnMiniGameChanged(int index)
    {
        // Verifica se há opções suficientes no dropdown
        if (miniGameDropdown.options.Count == 0 || index < 0 || index >= miniGameDropdown.options.Count)
        {
            quizPanel.SetActive(false);
            Debug.LogWarning("Dropdown de mini-game sem opções ou índice inválido.");
            return;
        }

        // Só mostra painel do quiz se quiz estiver selecionado
        void OnMiniGameChanged(int index)
        {
            if (miniGameDropdown.options.Count == 0 || index < 0 || index >= miniGameDropdown.options.Count)
            {
                quizPanel.SetActive(false);
                Debug.LogWarning("Dropdown de mini-game sem opções ou índice inválido.");
                return;
            }

            // Ignora maiúsculas/minúsculas e espaços
            string selected = miniGameDropdown.options[index].text.Trim().ToLower();
            quizPanel.SetActive(selected == "quiz");
        }
    }

    void AddQuestion()
    {
        // Instancie o prefab corretamente
        var questionUI = Instantiate(questionEditorPrefab, questionsListParent);
        var editor = questionUI.GetComponent<QuestionEditorUI>();
        questionEditors.Add(editor);
    }

    void SaveQuiz()
    {
        var quizData = ScriptableObject.CreateInstance<QuizDataScriptable>();
        quizData.categoryName = "Nova Fase";
        quizData.questions = new List<Question>();
        foreach (var editor in questionEditors)
        {
            quizData.questions.Add(editor.GetQuestion());
        }
#if UNITY_EDITOR
    AssetDatabase.CreateAsset(quizData, $"Assets/Quiz/Resources/{quizData.categoryName}.asset");
    AssetDatabase.SaveAssets();
#endif
        Debug.Log("Fase salva!");
    }
    public void OnImportMedia(QuestionEditorUI editor)
    {
        var extensions = new[] {
        new ExtensionFilter("Imagens", "png", "jpg", "jpeg"),
        new ExtensionFilter("Áudio", "mp3", "wav", "ogg"),
        new ExtensionFilter("Vídeo", "mp4", "mov", "avi")
    };

        string[] paths = StandaloneFileBrowser.OpenFilePanel("Escolha o arquivo de mídia", "", extensions, false);

        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            string path = paths[0];
            string fileName = System.IO.Path.GetFileName(path);
            string destDir = Application.dataPath + "/Quiz/Resources/ImportedMedia/";
            if (!System.IO.Directory.Exists(destDir))
                System.IO.Directory.CreateDirectory(destDir);

            string destPath = destDir + fileName;
            System.IO.File.Copy(path, destPath, true);

            // Salva o caminho relativo no editor da questão
            editor.SetMediaPath("ImportedMedia/" + fileName);

            Debug.Log("Arquivo copiado para: " + destPath);
        }
    }
}