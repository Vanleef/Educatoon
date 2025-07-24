// QuestionEditor.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using SimpleFileBrowser; // Mudança aqui - usar SimpleFileBrowser em vez de SFB
using System.IO;

public class QuestionEditor : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField questionTextInput;
    [SerializeField] private TMP_Dropdown questionTypeDropdown;
    [SerializeField] private TMP_InputField questionTimeInput;
    [SerializeField] private Button mediaButton;
    [SerializeField] private TMP_Text mediaPathText;
    [SerializeField] private Transform optionsContainer;
    [SerializeField] private GameObject optionPrefab;
    [SerializeField] private Button addOptionBtn;
    [SerializeField] private Button removeQuestionBtn;

    private Question question;
    private QuizEditorController editorManager;
    private List<OptionEditor> optionEditors = new List<OptionEditor>();

    public void Initialize(Question q, QuizEditorController manager)
    {
        question = q;
        editorManager = manager;
        SetupUI();
        SetupButtons();
    }

    private void SetupUI()
    {
        questionTextInput.text = question.questionInfo;
        questionTypeDropdown.value = (int)question.questionType;
        questionTimeInput.text = question.questionTime.ToString();

        UpdateMediaButton();
        CreateOptionEditors();
    }

    private void SetupButtons()
    {
        questionTextInput.onValueChanged.AddListener(OnQuestionTextChanged);
        questionTypeDropdown.onValueChanged.AddListener(OnQuestionTypeChanged);
        questionTimeInput.onValueChanged.AddListener(OnQuestionTimeChanged);
        mediaButton.onClick.AddListener(SelectMediaFile);
        addOptionBtn.onClick.AddListener(AddOption);
        removeQuestionBtn.onClick.AddListener(RemoveQuestion);
    }

    private void OnQuestionTextChanged(string value)
    {
        question.questionInfo = value;
    }

    private void OnQuestionTypeChanged(int value)
    {
        question.questionType = (QuestionType)value;
        UpdateMediaButton();
        ClearMediaPath();
    }

    private void OnQuestionTimeChanged(string value)
    {
        if (float.TryParse(value, out float time))
        {
            question.questionTime = time;
        }
    }

    private void UpdateMediaButton()
    {
        bool needsMedia = question.questionType != QuestionType.Text;
        mediaButton.gameObject.SetActive(needsMedia);
        mediaPathText.gameObject.SetActive(needsMedia);

        if (needsMedia)
        {
            string currentPath = GetCurrentMediaPath();
            mediaPathText.text = string.IsNullOrEmpty(currentPath) ? "Nenhuma mídia selecionada" : Path.GetFileName(currentPath);
        }
    }

    private string GetCurrentMediaPath()
    {
        switch (question.questionType)
        {
            case QuestionType.Image: return question.imagePath;
            case QuestionType.Audio: return question.audioPath;
            case QuestionType.Video: return question.videoPath;
            default: return "";
        }
    }

    private void ClearMediaPath()
    {
        question.imagePath = "";
        question.audioPath = "";
        question.videoPath = "";
        UpdateMediaButton();
    }

    private void SelectMediaFile()
    {
        // Configurar filtros baseado no tipo de pergunta
        SetupFileBrowserForQuestionType();

        // Usar SimpleFileBrowser
        FileBrowser.ShowLoadDialog(
            (paths) => OnMediaFileSelected(paths),
            () => Debug.Log("Seleção de mídia cancelada"),
            FileBrowser.PickMode.Files,
            false,
            null,
            null,
            "Selecionar Mídia"
        );
    }

    private void OnMediaFileSelected(string[] paths)
    {
        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            string selectedPath = paths[0];
            string fileName = Path.GetFileName(selectedPath);

            // Copy file to Resources folder
            string destDir = Path.Combine(Application.dataPath, "Quiz", "Resources", "ImportedMedia");
            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            string destPath = Path.Combine(destDir, fileName);

            try
            {
                File.Copy(selectedPath, destPath, true);

                // Set the relative path for Resources.Load
                string relativePath = "ImportedMedia/" + Path.GetFileNameWithoutExtension(fileName);
                SetMediaPath(relativePath);

                UpdateMediaButton();
                Debug.Log($"Mídia importada: {fileName}");

#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erro ao importar mídia: {e.Message}");
            }
        }
    }

    private void SetupFileBrowserForQuestionType()
    {
        switch (question.questionType)
        {
            case QuestionType.Image:
                FileBrowser.SetFilters(true,
                    new FileBrowser.Filter("Imagens", ".png", ".jpg", ".jpeg", ".gif", ".bmp"));
                break;
            case QuestionType.Audio:
                FileBrowser.SetFilters(true,
                    new FileBrowser.Filter("Áudio", ".mp3", ".wav", ".ogg", ".aac"));
                break;
            case QuestionType.Video:
                FileBrowser.SetFilters(true,
                    new FileBrowser.Filter("Vídeo", ".mp4", ".mov", ".avi", ".webm", ".mkv"));
                break;
            default:
                FileBrowser.SetFilters(true,
                    new FileBrowser.Filter("Todos os arquivos", ".*"));
                break;
        }
    }

    private void SetMediaPath(string path)
    {
        switch (question.questionType)
        {
            case QuestionType.Image:
                question.imagePath = path;
                break;
            case QuestionType.Audio:
                question.audioPath = path;
                break;
            case QuestionType.Video:
                question.videoPath = path;
                break;
        }
    }

    private void CreateOptionEditors()
    {
        // Clear existing
        foreach (var editor in optionEditors)
        {
            if (editor != null && editor.gameObject != null)
                DestroyImmediate(editor.gameObject);
        }
        optionEditors.Clear();

        // Create new ones
        for (int i = 0; i < question.options.Count; i++)
        {
            CreateOptionEditor(i);
        }
    }

    private void CreateOptionEditor(int index)
    {
        var optionObj = Instantiate(optionPrefab, optionsContainer);
        var optionEditor = optionObj.GetComponent<OptionEditor>();
        optionEditor.Initialize(question, index, this);
        optionEditors.Add(optionEditor);
    }

    private void AddOption()
    {
        question.options.Add("Nova opção");
        CreateOptionEditor(question.options.Count - 1);
    }

    public void RemoveOption(OptionEditor optionEditor)
    {
        int index = optionEditors.IndexOf(optionEditor);
        if (index >= 0)
        {
            question.options.RemoveAt(index);
            optionEditors.RemoveAt(index);
            DestroyImmediate(optionEditor.gameObject);

            // Adjust correct answer index if necessary
            if (question.correctAnswerIndex >= question.options.Count)
            {
                question.correctAnswerIndex = Mathf.Max(0, question.options.Count - 1);
            }

            // Refresh all option editors to update indices
            RefreshOptionEditors();
        }
    }

    private void RefreshOptionEditors()
    {
        for (int i = 0; i < optionEditors.Count; i++)
        {
            optionEditors[i].SetIndex(i);
        }
    }

    public void SetCorrectAnswer(int index)
    {
        question.correctAnswerIndex = index;
        RefreshCorrectAnswerToggles();
    }

    private void RefreshCorrectAnswerToggles()
    {
        for (int i = 0; i < optionEditors.Count; i++)
        {
            optionEditors[i].UpdateCorrectToggle(i == question.correctAnswerIndex);
        }
    }

    private void RemoveQuestion()
    {
        // CORREÇÃO: Usar o método correto
        editorManager.RemoveQuestionEditor(this);
    }

    public Question GetQuestion()
    {
        return question;
    }
}