// LevelEditorUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using SimpleFileBrowser; // Mudança aqui


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
        // CORREÇÃO: Removida função duplicada
        string selected = miniGameDropdown.options[index].text.Trim().ToLower();
        quizPanel.SetActive(selected == "quiz");
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
        // Usar SimpleFileBrowser em vez de SFB
        FileBrowser.ShowLoadDialog(
            (paths) =>
            {
                if (paths.Length > 0)
                {
                    editor.SetMediaPath(paths[0]);
                }
            },
            () => Debug.Log("Seleção cancelada"),
            FileBrowser.PickMode.Files,
            false,
            null,
            null,
            "Selecionar Mídia"
        );
    }
}