using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using SimpleFileBrowser;
using Newtonsoft.Json;
using UnityCursor = UnityEngine.Cursor; // NOVO: Alias para evitar ambiguidade
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuizEditorController : MonoBehaviour
{
    [Header("UI Document")]
    [SerializeField] private UIDocument uiDocument;

    [Header("Templates")]
    [SerializeField] private VisualTreeAsset questionItemTemplate;
    [SerializeField] private VisualTreeAsset optionItemTemplate;

    [Header("References")]
    [SerializeField] private QuizGameUI quizGameUI;

    [Header("Teacher Settings")]
    [SerializeField] private bool isTeacher = false; // NOVO: Controla se é professor

    private QuizData currentQuizData;
    private Question selectedQuestion;
    private int selectedQuestionIndex = -1;

    // UI Elements
    private VisualElement rootContainer;
    private TextField categoryNameField;
    private FloatField defaultTimeField;
    private DropdownField existingQuizzesDropdown;
    private ListView questionsList;
    private VisualElement questionEditorContainer;
    private VisualElement mediaSection;
    private VisualElement optionsContainer;

    // Question Editor Elements
    private TextField questionTextField;
    private DropdownField questionTypeDropdown;
    private FloatField questionTimeField;
    private Label mediaPathLabel;

    private string ResourcesQuizzesPath => Path.Combine(Application.dataPath, "Quiz", "Resources", "Quizzes");

    void Awake()
    {
        CreateResourcesQuizzesDirectory();
        SetupFileBrowser();
    }

    void OnEnable()
    {
        SetupUI();
        SetupEventListeners();
        LoadExistingQuizzes();

        if (existingQuizzesDropdown.choices.Count > 0)
        {
            existingQuizzesDropdown.SetValueWithoutNotify(existingQuizzesDropdown.choices[0]);
            LoadSelectedQuiz();
        }
        else
        {
            CreateNewQuiz();
        }
    }

    private void SetupUI()
    {
        rootContainer = uiDocument.rootVisualElement;

        // Quiz Info Elements
        categoryNameField = rootContainer.Q<TextField>("CategoryName");
        defaultTimeField = rootContainer.Q<FloatField>("DefaultTime");
        existingQuizzesDropdown = rootContainer.Q<DropdownField>("ExistingQuizzesDropdown");

        // Questions List
        questionsList = rootContainer.Q<ListView>("QuestionsList");
        SetupQuestionsList();

        // Question Editor Elements
        questionEditorContainer = rootContainer.Q<VisualElement>("QuestionEditorContainer");
        questionTextField = rootContainer.Q<TextField>("QuestionText");
        questionTypeDropdown = rootContainer.Q<DropdownField>("QuestionType");
        questionTimeField = rootContainer.Q<FloatField>("QuestionTime");
        mediaSection = rootContainer.Q<VisualElement>("MediaSection");
        mediaPathLabel = rootContainer.Q<Label>("MediaPathLabel");
        optionsContainer = rootContainer.Q<VisualElement>("OptionsContainer");

        // Setup Question Type Dropdown
        questionTypeDropdown.choices = new List<string> { "Texto", "Imagem", "Áudio", "Vídeo" };

        // Initially hide question editor
        questionEditorContainer.AddToClassList("disabled");
    }

    private void SetupEventListeners()
    {
        // Header buttons
        rootContainer.Q<Button>("BackButton").clicked += BackToMenu;

        // Quiz controls - AJUSTADO: Textos mudados para "Fase"
        rootContainer.Q<Button>("NewQuizButton").clicked += () => ShowConfirmationDialog(
            "Criar Nova Fase",
            "Tem certeza que deseja criar uma nova fase? Alterações não salvas serão perdidas.",
            CreateNewQuiz
        );

        rootContainer.Q<Button>("SaveQuizButton").clicked += () => ShowConfirmationDialog(
            "Salvar Fase",
            "Tem certeza que deseja salvar a fase atual?",
            SaveQuiz
        );

        rootContainer.Q<Button>("LoadQuizButton").clicked += () => ShowConfirmationDialog(
            "Carregar Fase",
            "Tem certeza que deseja carregar a fase selecionada? Alterações não salvas serão perdidas.",
            LoadSelectedQuiz
        );

        // NOVO: Botão para remover quiz - AJUSTADO: Texto mudado para "Fase"
        rootContainer.Q<Button>("RemoveQuizButton").clicked += () => ShowConfirmationDialog(
            "REMOVER FASE",
            $"ATENÇÃO: Tem certeza que deseja REMOVER PERMANENTEMENTE a fase '{existingQuizzesDropdown.value}'?\n\nEsta ação não pode ser desfeita!",
            RemoveSelectedQuiz,
            true // isDangerous
        );

        rootContainer.Q<Button>("AddQuestionButton").clicked += () => ShowConfirmationDialog(
            "Adicionar Pergunta",
            "Deseja adicionar uma nova pergunta à fase?",
            AddNewQuestion
        );

        // Quiz info fields
        categoryNameField.RegisterValueChangedCallback(evt =>
        {
            if (currentQuizData != null)
                currentQuizData.categoryName = evt.newValue;
        });

        defaultTimeField.RegisterValueChangedCallback(evt =>
        {
            if (currentQuizData != null)
                currentQuizData.defaultQuestionTime = evt.newValue;
        });

        // Question editor fields
        questionTextField.RegisterValueChangedCallback(evt =>
        {
            if (selectedQuestion != null)
            {
                selectedQuestion.questionInfo = evt.newValue;
                questionsList.RefreshItems();
            }
        });

        questionTypeDropdown.RegisterValueChangedCallback(evt =>
        {
            if (selectedQuestion != null)
            {
                int typeIndex = questionTypeDropdown.choices.IndexOf(evt.newValue);
                if (typeIndex >= 0)
                {
                    selectedQuestion.questionType = (QuestionType)typeIndex;
                    ClearMediaPath();
                    UpdateMediaSection();
                    questionsList.RefreshItems();
                }
            }
        });

        questionTimeField.RegisterValueChangedCallback(evt =>
        {
            if (selectedQuestion != null)
                selectedQuestion.questionTime = evt.newValue;
        });

        // Media and options
        rootContainer.Q<Button>("ImportMediaButton").clicked += ImportMedia;
        rootContainer.Q<Button>("AddOptionButton").clicked += AddOption;
        rootContainer.Q<Button>("RemoveQuestionButton").clicked += () => ShowConfirmationDialog(
            "Remover Pergunta",
            "Tem certeza que deseja remover esta pergunta?",
            RemoveSelectedQuestion,
            true
        );
    }

    private void SetupQuestionsList()
    {
        // CORRIGIDO: Configurar propriedades do ListView
        questionsList.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
        questionsList.fixedItemHeight = 80; // CORRIGIDO: Usar fixedItemHeight em vez de itemHeight
        questionsList.showBorder = false;
        questionsList.showFoldoutHeader = false;
        questionsList.showAddRemoveFooter = false;
        questionsList.reorderable = false;
        questionsList.selectionType = SelectionType.Single;

        questionsList.makeItem = () =>
        {
            var item = questionItemTemplate.Instantiate();

            // ADICIONADO: Garantir que o item tem as classes corretas
            var container = item.Q<VisualElement>("QuestionItem");
            if (container != null)
            {
                container.style.backgroundColor = new Color(0.3f, 0.3f, 0.3f, 1f); // Cor de fallback
                container.style.minHeight = 70;
            }

            return item;
        };

        questionsList.bindItem = (element, index) =>
        {
            if (currentQuizData == null || index >= currentQuizData.questions.Count)
                return;

            var question = currentQuizData.questions[index];

            // CORRIGIDO: Agora conta espaços na verificação dos 10 caracteres
            string preview;
            if (string.IsNullOrEmpty(question.questionInfo) || question.questionInfo.Trim() == "")
            {
                preview = "Sem texto";
            }
            else
            {
                // CORRIGIDO: Contar espaços para verificação dos 10 caracteres
                string originalText = question.questionInfo.Trim();
                if (originalText.Length > 10)
                {
                    preview = originalText.Substring(0, 10) + "...";
                }
                else
                {
                    preview = originalText;
                }
            }

            var previewLabel = element.Q<Label>("QuestionPreview");
            var typeLabel = element.Q<Label>("QuestionType");

            if (previewLabel != null)
            {
                previewLabel.text = preview;
                // MELHORADO: Garantir visibilidade do texto
                previewLabel.style.color = Color.white;
                previewLabel.style.fontSize = 15;
                previewLabel.style.unityFontStyleAndWeight = FontStyle.Normal;
            }

            if (typeLabel != null)
            {
                string typeName = GetQuestionTypeDisplayName(question.questionType);
                typeLabel.text = typeName;

                // MELHORADO: Cores mais visíveis
                typeLabel.style.color = Color.white;
                typeLabel.style.fontSize = 11;
                typeLabel.style.unityFontStyleAndWeight = FontStyle.Bold;

                // Remover classes antigas
                typeLabel.RemoveFromClassList("image");
                typeLabel.RemoveFromClassList("audio");
                typeLabel.RemoveFromClassList("video");

                // Aplicar cores diretamente
                switch (question.questionType)
                {
                    case QuestionType.Image:
                        typeLabel.AddToClassList("image");
                        typeLabel.style.backgroundColor = new Color(0.31f, 0.59f, 0.31f, 1f); // Verde
                        break;
                    case QuestionType.Audio:
                        typeLabel.AddToClassList("audio");
                        typeLabel.style.backgroundColor = new Color(0.59f, 0.31f, 0.59f, 1f); // Roxo
                        break;
                    case QuestionType.Video:
                        typeLabel.AddToClassList("video");
                        typeLabel.style.backgroundColor = new Color(0.31f, 0.31f, 0.59f, 1f); // Azul
                        break;
                    default:
                        typeLabel.style.backgroundColor = new Color(0.39f, 0.39f, 0.39f, 1f); // Cinza
                        break;
                }
            }

            // Aplicar seleção
            var container = element.Q<VisualElement>("QuestionItem");
            if (container != null)
            {
                // Remover classe selected de todos primeiro
                container.RemoveFromClassList("selected");

                if (index == selectedQuestionIndex)
                {
                    container.AddToClassList("selected");
                    // CORRIGIDO: Cores diretas para garantir visibilidade
                    container.style.backgroundColor = new Color(0f, 0.55f, 1f, 0.25f);
                    container.style.borderLeftColor = new Color(0f, 0.55f, 1f, 1f);
                    container.style.borderLeftWidth = 4;
                }
                else
                {
                    // CORRIGIDO: Cores padrão
                    container.style.backgroundColor = new Color(0.29f, 0.29f, 0.29f, 1f);
                    container.style.borderLeftColor = new Color(0.47f, 0.47f, 0.47f, 1f);
                    container.style.borderLeftWidth = 4;
                }
            }
        };

        questionsList.selectionChanged += OnQuestionSelected;

        // ADICIONADO: Forçar refresh inicial
        if (currentQuizData != null)
        {
            questionsList.itemsSource = currentQuizData.questions;
            questionsList.RefreshItems();
        }
    }

    // NOVO: Sistema de dialogs de confirmação
    private void ShowConfirmationDialog(string title, string message, System.Action onConfirm, bool isDangerous = false)
    {
        var dialog = new VisualElement();
        dialog.AddToClassList("confirmation-dialog");

        var dialogBox = new VisualElement();
        dialogBox.AddToClassList("dialog-box");

        var titleLabel = new Label(title);
        titleLabel.AddToClassList("dialog-title");

        var messageLabel = new Label(message);
        messageLabel.AddToClassList("dialog-message");

        var buttonsContainer = new VisualElement();
        buttonsContainer.AddToClassList("dialog-buttons");

        var cancelButton = new Button(() => rootContainer.Remove(dialog));
        cancelButton.text = "Cancelar";
        cancelButton.AddToClassList("dialog-button");
        cancelButton.AddToClassList("cancel");

        var confirmButton = new Button(() =>
        {
            rootContainer.Remove(dialog);
            onConfirm?.Invoke();
        });
        confirmButton.text = isDangerous ? "SIM, REMOVER" : "Confirmar";
        confirmButton.AddToClassList("dialog-button");
        confirmButton.AddToClassList(isDangerous ? "danger" : "confirm");

        buttonsContainer.Add(cancelButton);
        buttonsContainer.Add(confirmButton);

        dialogBox.Add(titleLabel);
        dialogBox.Add(messageLabel);
        dialogBox.Add(buttonsContainer);
        dialog.Add(dialogBox);

        rootContainer.Add(dialog);
    }

    // NOVO: Método para remover quiz
    private void RemoveSelectedQuiz()
    {
        var selectedQuiz = existingQuizzesDropdown.value;
        if (string.IsNullOrEmpty(selectedQuiz))
        {
            ShowSimpleDialog("Erro", "Nenhuma fase selecionada para remoção.", false);
            return;
        }

        string filePath = Path.Combine(ResourcesQuizzesPath, selectedQuiz + ".json");

        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
                Debug.Log($"Fase removida: {selectedQuiz}");

#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif

                LoadExistingQuizzes();
                UpdateMainGameQuizList();

                // Criar nova fase se não houver mais nenhuma
                if (existingQuizzesDropdown.choices.Count == 0)
                {
                    CreateNewQuiz();
                }
                else
                {
                    existingQuizzesDropdown.SetValueWithoutNotify(existingQuizzesDropdown.choices[0]);
                    LoadSelectedQuiz();
                }

                ShowSimpleDialog("Sucesso", "Fase removida com sucesso!", true);
            }
            catch (Exception e)
            {
                Debug.LogError($"Erro ao remover a fase: {e.Message}");
                ShowSimpleDialog("Erro", $"Erro ao remover a fase:\n{e.Message}", false);
            }
        }
        else
        {
            ShowSimpleDialog("Erro", "Arquivo da fase não encontrado.", false);
        }
    }

    // NOVO: Dialog simples para notificações (sem o blink verde)
    private void ShowSimpleDialog(string title, string message, bool isSuccess)
    {
        var dialog = new VisualElement();
        dialog.AddToClassList("confirmation-dialog");

        var dialogBox = new VisualElement();
        dialogBox.AddToClassList("dialog-box");

        var titleLabel = new Label(title);
        titleLabel.AddToClassList("dialog-title");

        // CORREÇÃO: Usar classes CSS em vez de definir cores diretamente
        if (isSuccess)
        {
            dialogBox.AddToClassList("success-dialog");
            titleLabel.AddToClassList("success-title");
        }
        else
        {
            dialogBox.AddToClassList("error-dialog");
            titleLabel.AddToClassList("error-title");
        }

        var messageLabel = new Label(message);
        messageLabel.AddToClassList("dialog-message");

        var okButton = new Button(() => rootContainer.Remove(dialog));
        okButton.text = "OK";
        okButton.AddToClassList("dialog-button");
        okButton.AddToClassList(isSuccess ? "confirm" : "cancel");
        okButton.style.alignSelf = Align.Center;
        okButton.style.width = 100;

        dialogBox.Add(titleLabel);
        dialogBox.Add(messageLabel);
        dialogBox.Add(okButton);
        dialog.Add(dialogBox);

        rootContainer.Add(dialog);

        // Auto-remove após 3 segundos apenas para mensagens de sucesso
        if (isSuccess)
        {
            StartCoroutine(RemoveDialogAfterDelay(dialog, 3f));
        }
    }

    private void OnQuestionSelected(IEnumerable<object> selectedItems)
    {
        var selected = questionsList.selectedIndex;
        if (selected >= 0 && selected < currentQuizData.questions.Count)
        {
            selectedQuestionIndex = selected;
            selectedQuestion = currentQuizData.questions[selected];
            UpdateQuestionEditor();
            questionEditorContainer.RemoveFromClassList("disabled");
            questionsList.RefreshItems();
        }
    }

    private void UpdateQuestionEditor()
    {
        if (selectedQuestion == null) return;

        questionTextField.SetValueWithoutNotify(selectedQuestion.questionInfo);
        string questionTypeString = GetQuestionTypeDisplayName(selectedQuestion.questionType);
        questionTypeDropdown.SetValueWithoutNotify(questionTypeString);
        questionTimeField.SetValueWithoutNotify(selectedQuestion.questionTime);

        UpdateMediaSection();
        UpdateOptionsContainer();
    }

    private void UpdateMediaSection()
    {
        bool needsMedia = selectedQuestion.questionType != QuestionType.Text;

        if (needsMedia)
        {
            mediaSection.AddToClassList("visible");
            string currentPath = GetCurrentMediaPath();
            mediaPathLabel.text = string.IsNullOrEmpty(currentPath) ?
                "Nenhuma mídia selecionada" : Path.GetFileName(currentPath);
        }
        else
        {
            mediaSection.RemoveFromClassList("visible");
        }
    }

    private void UpdateOptionsContainer()
    {
        if (selectedQuestion == null || optionsContainer == null) return;

        optionsContainer.Clear();

        for (int i = 0; i < selectedQuestion.options.Count; i++)
        {
            CreateOptionElement(i);
        }

        // Forçar atualização do layout
        optionsContainer.MarkDirtyRepaint();
    }

    private void CreateOptionElement(int index)
    {
        var optionElement = optionItemTemplate.Instantiate();
        optionElement.AddToClassList("option-item");

        var textField = optionElement.Q<TextField>("OptionTextField");
        var correctToggle = optionElement.Q<Toggle>("CorrectToggle");
        var removeButton = optionElement.Q<Button>("RemoveOptionButton");

        textField.SetValueWithoutNotify(selectedQuestion.options[index]);
        correctToggle.SetValueWithoutNotify(selectedQuestion.correctAnswerIndex == index);

        textField.RegisterValueChangedCallback(evt =>
        {
            selectedQuestion.options[index] = evt.newValue;
            questionsList.RefreshItems();
        });

        correctToggle.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue)
            {
                selectedQuestion.correctAnswerIndex = index;
                UpdateOptionsContainer();
            }
        });

        removeButton.clicked += () => RemoveOption(index);

        if (selectedQuestion.options.Count <= 2)
            removeButton.style.display = DisplayStyle.None;

        optionsContainer.Add(optionElement);
    }

    private bool ValidateQuizData()
    {
        List<string> errors = new List<string>();

        if (string.IsNullOrEmpty(currentQuizData.categoryName) || currentQuizData.categoryName.Trim() == "")
        {
            errors.Add("• Nome da categoria não pode estar vazio");
        }

        if (currentQuizData.defaultQuestionTime <= 0)
        {
            errors.Add("• Tempo padrão deve ser maior que zero");
        }

        if (currentQuizData.questions.Count == 0)
        {
            errors.Add("• A fase deve ter pelo menos uma pergunta");
        }

        for (int i = 0; i < currentQuizData.questions.Count; i++)
        {
            var question = currentQuizData.questions[i];

            if (string.IsNullOrEmpty(question.questionInfo) || question.questionInfo.Trim() == "")
            {
                errors.Add($"• Pergunta {i + 1}: Texto não pode estar vazio");
            }

            if (question.questionTime <= 0)
            {
                errors.Add($"• Pergunta {i + 1}: Tempo deve ser maior que zero");
            }

            if (question.options.Count < 2)
            {
                errors.Add($"• Pergunta {i + 1}: Deve ter pelo menos 2 opções de resposta");
            }
            else
            {
                for (int j = 0; j < question.options.Count; j++)
                {
                    if (string.IsNullOrEmpty(question.options[j]) || question.options[j].Trim() == "")
                    {
                        errors.Add($"• Pergunta {i + 1}, Opção {j + 1}: Não pode estar vazia");
                    }
                }

                if (question.correctAnswerIndex < 0 || question.correctAnswerIndex >= question.options.Count)
                {
                    errors.Add($"• Pergunta {i + 1}: Resposta correta inválida");
                }
            }

            if (question.questionType != QuestionType.Text)
            {
                string mediaPath = GetMediaPathForQuestion(question);
                if (string.IsNullOrEmpty(mediaPath))
                {
                    errors.Add($"• Pergunta {i + 1}: Mídia é obrigatória para o tipo {GetQuestionTypeDisplayName(question.questionType)}");
                }
            }
        }

        if (errors.Count > 0)
        {
            string errorMessage = "Não é possível salvar a fase. Corrija os seguintes erros:\n\n" + string.Join("\n", errors);
            ShowSimpleDialog("Erro de Validação", errorMessage, false);
            return false;
        }

        return true;
    }

    private string GetMediaPathForQuestion(Question question)
    {
        switch (question.questionType)
        {
            case QuestionType.Image: return question.imagePath;
            case QuestionType.Audio: return question.audioPath;
            case QuestionType.Video: return question.videoPath;
            default: return "";
        }
    }

    private void AddNewQuestion()
    {
        if (currentQuizData == null)
        {
            Debug.LogWarning("Não é possível adicionar pergunta: currentQuizData é null");
            return;
        }

        var newQuestion = new Question
        {
            questionInfo = "Nova Pergunta",
            questionType = QuestionType.Text,
            questionTime = currentQuizData.defaultQuestionTime,
            options = new List<string> { "Opção 1", "Opção 2" },
            correctAnswerIndex = 0,
            imagePath = "",
            audioPath = "",
            videoPath = ""
        };

        currentQuizData.questions.Add(newQuestion);
        questionsList.itemsSource = currentQuizData.questions;
        questionsList.RefreshItems();

        int newIndex = currentQuizData.questions.Count - 1;
        selectedQuestionIndex = newIndex;
        selectedQuestion = newQuestion;
        questionsList.SetSelection(newIndex);
        UpdateQuestionEditor();
        questionEditorContainer.RemoveFromClassList("disabled");

        ShowSimpleDialog("Sucesso", "Nova pergunta adicionada!", true);
    }

    private void AddOption()
    {
        if (selectedQuestion != null)
        {
            selectedQuestion.options.Add($"Opção {selectedQuestion.options.Count + 1}");
            UpdateOptionsContainer();

            // NOVO: Scroll automático para mostrar a nova opção
            ScrollToBottomOfOptions();
        }
    }

    // NOVO: Método para fazer scroll até o final das opções
    private void ScrollToBottomOfOptions()
    {
        var questionEditorScroll = rootContainer.Q<ScrollView>("QuestionEditorScrollView");
        if (questionEditorScroll != null)
        {
            // Usar schedule para garantir que o layout foi atualizado
            questionEditorScroll.schedule.Execute(() =>
            {
                // Scroll para o final
                questionEditorScroll.scrollOffset = new Vector2(0, questionEditorScroll.contentContainer.layout.height);
            }).ExecuteLater(50); // Delay de 50ms para garantir que o layout foi processado
        }
    }

    /*// MELHORADO: Método CreateOptionElement com melhor estrutura
    private void CreateOptionElement(int index)
    {
        var optionElement = optionItemTemplate.Instantiate();
        optionElement.AddToClassList("option-item");

        var textField = optionElement.Q<TextField>("OptionTextField");
        var correctToggle = optionElement.Q<Toggle>("CorrectToggle");
        var removeButton = optionElement.Q<Button>("RemoveOptionButton");

        textField.SetValueWithoutNotify(selectedQuestion.options[index]);
        correctToggle.SetValueWithoutNotify(selectedQuestion.correctAnswerIndex == index);

        textField.RegisterValueChangedCallback(evt =>
        {
            selectedQuestion.options[index] = evt.newValue;
            questionsList.RefreshItems();
        });

        correctToggle.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue)
            {
                selectedQuestion.correctAnswerIndex = index;
                UpdateOptionsContainer();
            }
        });

        removeButton.clicked += () => RemoveOption(index);

        if (selectedQuestion.options.Count <= 2)
            removeButton.style.display = DisplayStyle.None;

        optionsContainer.Add(optionElement);
    }
*/
    // MELHORADO: Método RemoveOption também com scroll inteligente
    private void RemoveOption(int index)
    {
        if (selectedQuestion != null && selectedQuestion.options.Count > 2)
        {
            selectedQuestion.options.RemoveAt(index);

            if (selectedQuestion.correctAnswerIndex >= selectedQuestion.options.Count)
                selectedQuestion.correctAnswerIndex = selectedQuestion.options.Count - 1;
            else if (selectedQuestion.correctAnswerIndex > index)
                selectedQuestion.correctAnswerIndex--;

            UpdateOptionsContainer();

            // NOVO: Scroll para manter a visualização das opções
            var questionEditorScroll = rootContainer.Q<ScrollView>("QuestionEditorScrollView");
            if (questionEditorScroll != null)
            {
                questionEditorScroll.schedule.Execute(() =>
                {
                    // Manter posição atual ou ajustar se necessário
                    var currentOffset = questionEditorScroll.scrollOffset;
                    var maxOffset = questionEditorScroll.contentContainer.layout.height - questionEditorScroll.layout.height;

                    if (currentOffset.y > maxOffset)
                    {
                        questionEditorScroll.scrollOffset = new Vector2(0, maxOffset);
                    }
                }).ExecuteLater(50);
            }
        }
    }

    private void RemoveSelectedQuestion()
    {
        if (selectedQuestionIndex >= 0)
        {
            currentQuizData.questions.RemoveAt(selectedQuestionIndex);
            questionsList.itemsSource = currentQuizData.questions;
            questionsList.RefreshItems();

            selectedQuestion = null;
            selectedQuestionIndex = -1;
            questionEditorContainer.AddToClassList("disabled");

            ShowSimpleDialog("Sucesso", "Pergunta removida!", true);
        }
    }

    private void ImportMedia()
    {
        SetupFileBrowserForQuestionType();

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

            string destDir = Path.Combine(Application.dataPath, "Quiz", "Resources", "ImportedMedia");
            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            string destPath = Path.Combine(destDir, fileName);

            try
            {
                File.Copy(selectedPath, destPath, true);

                string relativePath = "ImportedMedia/" + Path.GetFileNameWithoutExtension(fileName);
                SetMediaPath(relativePath);

                UpdateMediaSection();
                Debug.Log($"Mídia importada: {fileName}");

#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif

                ShowSimpleDialog("Sucesso", $"Mídia importada: {fileName}", true);
            }
            catch (Exception e)
            {
                Debug.LogError($"Erro ao importar mídia: {e.Message}");
                ShowSimpleDialog("Erro", $"Erro ao importar mídia:\n{e.Message}", false);
            }
        }
    }

    private void CreateNewQuiz()
    {
        currentQuizData = new QuizData
        {
            categoryName = "Nova Categoria",
            defaultQuestionTime = 30f,
            questions = new List<Question>()
        };

        UpdateUI();
        AddNewQuestion();
        ShowSimpleDialog("Sucesso", "Nova fase criada!", true);
    }

    private void UpdateUI()
    {
        if (currentQuizData == null)
        {
            Debug.LogWarning("currentQuizData é null");
            return;
        }

        categoryNameField.SetValueWithoutNotify(currentQuizData.categoryName);
        defaultTimeField.SetValueWithoutNotify(currentQuizData.defaultQuestionTime);

        questionsList.itemsSource = currentQuizData.questions;
        questionsList.RefreshItems();

        if (currentQuizData.questions.Count > 0)
        {
            selectedQuestionIndex = 0;
            selectedQuestion = currentQuizData.questions[0];
            questionsList.SetSelection(0);
            UpdateQuestionEditor();
            questionEditorContainer.RemoveFromClassList("disabled");
        }
        else
        {
            selectedQuestion = null;
            selectedQuestionIndex = -1;
            questionEditorContainer.AddToClassList("disabled");
            ClearQuestionEditor();
        }
    }

    private void ClearQuestionEditor()
    {
        questionTextField.SetValueWithoutNotify("");
        questionTypeDropdown.SetValueWithoutNotify("Texto");
        questionTimeField.SetValueWithoutNotify(currentQuizData?.defaultQuestionTime ?? 30f);
        mediaPathLabel.text = "Nenhuma mídia selecionada";
        mediaSection.RemoveFromClassList("visible");
        optionsContainer.Clear();
    }

    private void LoadSelectedQuiz()
    {
        var selectedQuiz = existingQuizzesDropdown.value;
        if (string.IsNullOrEmpty(selectedQuiz)) return;

        string filePath = Path.Combine(ResourcesQuizzesPath, selectedQuiz + ".json");

        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                currentQuizData = JsonConvert.DeserializeObject<QuizData>(json);
                UpdateUI();
                Debug.Log($"Fase carregada: {selectedQuiz}");
                ShowSimpleDialog("Sucesso", $"Fase '{selectedQuiz}' carregada com sucesso!", true);
            }
            catch (Exception e)
            {
                Debug.LogError($"Erro ao carregar a fase: {e.Message}");
                ShowSimpleDialog("Erro", $"Erro ao carregar a fase:\n{e.Message}", false);
            }
        }
    }

    private void UpdateMainGameQuizList()
    {
        var quizManager = FindFirstObjectByType<QuizManager>();
        if (quizManager != null)
        {
            if (quizManager.GetType().GetMethod("RefreshQuizList") != null)
            {
                quizManager.RefreshQuizList();
            }
            else
            {
                var method = quizManager.GetType().GetMethod("LoadAllQuizDatas",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (method != null)
                {
                    method.Invoke(quizManager, null);
                }
            }

            if (quizGameUI != null && quizGameUI.GetType().GetMethod("RefreshCategoryButtons") != null)
            {
                quizGameUI.RefreshCategoryButtons();
            }
        }
    }

    private void SaveQuiz()
    {
        if (currentQuizData == null) return;

        if (!ValidateQuizData())
        {
            return;
        }

        string sanitizedName = SanitizeFileName(currentQuizData.categoryName);
        string fileName = sanitizedName + ".json";
        string filePath = Path.Combine(ResourcesQuizzesPath, fileName);

        string json = JsonConvert.SerializeObject(currentQuizData, Formatting.Indented);

        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log($"Fase salva em: {filePath}");

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

            LoadExistingQuizzes();
            UpdateMainGameQuizList();
            ShowSimpleDialog("Sucesso", "Fase salva com sucesso!", true);
        }
        catch (Exception e)
        {
            Debug.LogError($"Erro ao salvar a fase: {e.Message}");
            ShowSimpleDialog("Erro", $"Erro ao salvar a fase:\n{e.Message}", false);
        }
    }

    private string SanitizeFileName(string fileName)
    {
        string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        foreach (char c in invalidChars)
        {
            fileName = fileName.Replace(c, '_');
        }
        return fileName.Replace(" ", "_");
    }

    private System.Collections.IEnumerator RemoveDialogAfterDelay(VisualElement dialog, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (dialog.parent != null)
        {
            rootContainer.Remove(dialog);
        }
    }

    private void LoadExistingQuizzes()
    {
        var choices = new List<string>();

        if (Directory.Exists(ResourcesQuizzesPath))
        {
            string[] files = Directory.GetFiles(ResourcesQuizzesPath, "*.json");
            foreach (string file in files)
            {
                choices.Add(Path.GetFileNameWithoutExtension(file));
            }
        }

        existingQuizzesDropdown.choices = choices;
    }

    private void BackToMenu()
    {
        UpdateMainGameQuizList();
        gameObject.SetActive(false);
    }

    // Utility methods
    private void CreateResourcesQuizzesDirectory()
    {
        if (!Directory.Exists(ResourcesQuizzesPath))
        {
            Directory.CreateDirectory(ResourcesQuizzesPath);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
    }

    private void SetupFileBrowser()
    {
        FileBrowser.SetDefaultFilter(".json");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
    }

    private void SetupFileBrowserForQuestionType()
    {
        if (selectedQuestion == null) return;

        switch (selectedQuestion.questionType)
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

    private string GetQuestionTypeDisplayName(QuestionType type)
    {
        switch (type)
        {
            case QuestionType.Text: return "Texto";
            case QuestionType.Image: return "Imagem";
            case QuestionType.Audio: return "Áudio";
            case QuestionType.Video: return "Vídeo";
            default: return "Desconhecido";
        }
    }

    private string GetCurrentMediaPath()
    {
        if (selectedQuestion == null) return "";

        switch (selectedQuestion.questionType)
        {
            case QuestionType.Image: return selectedQuestion.imagePath;
            case QuestionType.Audio: return selectedQuestion.audioPath;
            case QuestionType.Video: return selectedQuestion.videoPath;
            default: return "";
        }
    }

    private void SetMediaPath(string path)
    {
        if (selectedQuestion == null) return;

        switch (selectedQuestion.questionType)
        {
            case QuestionType.Image:
                selectedQuestion.imagePath = path;
                break;
            case QuestionType.Audio:
                selectedQuestion.audioPath = path;
                break;
            case QuestionType.Video:
                selectedQuestion.videoPath = path;
                break;
        }
    }

    private void ClearMediaPath()
    {
        if (selectedQuestion != null)
        {
            selectedQuestion.imagePath = "";
            selectedQuestion.audioPath = "";
            selectedQuestion.videoPath = "";
        }
    }

    public void RemoveQuestionEditor(QuestionEditor editor)
    {
        RemoveSelectedQuestion();
    }

    public void OpenEditor()
    {
        Debug.Log($"QuizEditorController: Tentando abrir editor. IsTeacher = {isTeacher}");

        // Verificar se é professor antes de abrir o editor
        if (!isTeacher)
        {
            Debug.LogWarning("Acesso negado: Apenas professores podem acessar o editor de fases.");
            return;
        }

        Debug.Log("QuizEditorController: Abrindo editor...");
        gameObject.SetActive(true);
        LoadExistingQuizzes();
    }

    // NOVO: Método público para verificar se é professor
    public bool IsTeacher => isTeacher;

    // NOVO: Método para definir se é professor (útil para ser chamado via código)
    public void SetTeacherMode(bool teacherMode)
    {
        isTeacher = teacherMode;
        Debug.Log($"QuizEditorController: Modo Professor definido como {isTeacher}");
        UpdateEditorButtonVisibility();
    }

    // CORRIGIDO: Método para atualizar visibilidade do botão no QuizGameUI
    private void UpdateEditorButtonVisibility()
    {
        if (quizGameUI != null)
        {
            // CORRIGIDO: Usar UIElements corretamente
            var uiDocument = quizGameUI.GetComponent<UIDocument>();
            if (uiDocument != null)
            {
                var rootElement = uiDocument.rootVisualElement;
                if (rootElement != null)
                {
                    var editorButton = rootElement.Q<UnityEngine.UIElements.Button>("EditorButton");
                    if (editorButton != null)
                    {
                        editorButton.style.display = isTeacher ? DisplayStyle.Flex : DisplayStyle.None;
                        Debug.Log($"QuizEditorController: Botão editor {(isTeacher ? "visível" : "oculto")}");
                    }
                    else
                    {
                        Debug.Log("QuizEditorController: Botão EditorButton não encontrado no QuizGameUI");
                    }
                }
            }
        }
    }

    void Start()
    {
        // CORRIGIDO: Usar o alias
        UnityCursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        // Configurar visibilidade do botão na inicialização
        UpdateEditorButtonVisibility();

        Debug.Log($"QuizEditorController: Inicializado. Modo Professor = {isTeacher}");
    }
}