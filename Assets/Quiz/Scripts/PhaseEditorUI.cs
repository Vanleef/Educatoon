using UnityEngine;
using UnityEngine.UIElements;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using SimpleFileBrowser; // Mudança: usar SimpleFileBrowser em vez de SFB

public class PhaseEditorUI : MonoBehaviour
{
    [SerializeField] UIDocument uiDoc;
    [SerializeField] VisualTreeAsset questionTemplate;

    private List<PhaseConfig> phases = new List<PhaseConfig>();
    private PhaseConfig currentPhase; // ADICIONADO: Declaração da variável que estava faltando
    private ListView phaseList, questionList;
    private TextField phaseNameField;
    private EnumField quizTypeField;
    private Button addPhaseBtn, addQBtn, saveBtn;

    void OnEnable()
    {
        var root = uiDoc.rootVisualElement;
        phaseList = root.Q<ListView>("phaseList");
        questionList = root.Q<ListView>("questionList");
        phaseNameField = root.Q<TextField>("phaseName");
        quizTypeField = root.Q<EnumField>("quizType");
        addPhaseBtn = root.Q<Button>("addPhase");
        addQBtn = root.Q<Button>("addQuestion");
        saveBtn = root.Q<Button>("saveBtn");

        // Inicializa EnumField
        quizTypeField.Init(QuizType.Texto);
        quizTypeField.value = QuizType.Texto;
        quizTypeField.RegisterValueChangedCallback(evt =>
        {
            if (phaseList.selectedIndex >= 0)
            {
                phases[phaseList.selectedIndex].quizType = (QuizType)evt.newValue;
                questionList.Rebuild();
            }
        });

        // Configura listas
        phaseList.itemsSource = phases;
        phaseList.selectionType = SelectionType.Single;
        phaseList.makeItem = () => new Label();
        phaseList.bindItem = (e, i) => ((Label)e).text = phases[i].name;

        // CORREÇÃO: Usar selectionChanged em vez de onSelectionChange
        phaseList.selectionChanged += (selectedItems) =>
        {
            if (phaseList.selectedIndex >= 0 && phaseList.selectedIndex < phases.Count)
            {
                currentPhase = phases[phaseList.selectedIndex];
                OnPhaseSelected(currentPhase); // ADICIONADO: Chamar método para atualizar UI
            }
            phaseList.Rebuild();
        };

        questionList.makeItem = () => questionTemplate.Instantiate();
        questionList.bindItem = (e, i) =>
        {
            // CORREÇÃO: Verificar se há fase selecionada antes de acessar questions
            if (phaseList.selectedIndex < 0 || phaseList.selectedIndex >= phases.Count)
                return;

            var q = phases[phaseList.selectedIndex].questions[i];

            // Texto da pergunta
            var tf = e.Q<TextField>("questionText");
            tf.value = q.questionText;
            tf.RegisterValueChangedCallback(evt => q.questionText = evt.newValue);

            // Botão de importar mídia
            var importBtn = e.Q<Button>("importMediaBtn");
            var mediaLabel = e.Q<Label>("mediaFileLabel");
            importBtn.text = "Importar Mídia";

            // CORREÇÃO: Limpar listeners anteriores de forma segura
            importBtn.clicked -= null;
            importBtn.clicked += () => ImportMediaForQuestion(q, mediaLabel);

            mediaLabel.text = string.IsNullOrEmpty(q.mediaPath) ? "Nenhum arquivo" : System.IO.Path.GetFileName(q.mediaPath);
            importBtn.style.display = (q.questionType == QuizType.Texto) ? DisplayStyle.None : DisplayStyle.Flex;
            mediaLabel.style.display = importBtn.style.display;

            // Opções de resposta
            var answersContainer = e.Q<VisualElement>("answersContainer");
            answersContainer.Clear();
            for (int j = 0; j < q.answers.Count; j++)
            {
                var answerField = new TextField { value = q.answers[j] };
                int idx = j;
                answerField.RegisterValueChangedCallback(evt => q.answers[idx] = evt.newValue);

                var correctToggle = new Toggle("Correta") { value = (q.correctAnswerIndex == idx) };
                correctToggle.RegisterValueChangedCallback(evt =>
                {
                    if (evt.newValue) q.correctAnswerIndex = idx;
                });

                var answerRow = new VisualElement();
                answerRow.style.flexDirection = FlexDirection.Row;
                answerRow.style.alignItems = Align.Center;
                answerRow.Add(answerField);
                answerRow.Add(correctToggle);
                answersContainer.Add(answerRow);
            }

            // Botão para adicionar nova opção de resposta
            var addAnswerBtn = e.Q<Button>("addAnswerBtn");
            // CORREÇÃO: Melhor forma de gerenciar callbacks
            addAnswerBtn.clicked -= () => AddAnswerToQuestion(q);
            addAnswerBtn.clicked += () => AddAnswerToQuestion(q);
        };

        // Botões
        addPhaseBtn.clicked += () =>
        {
            var newPhase = new PhaseConfig
            {
                name = $"Fase {phases.Count + 1}",
                quizType = QuizType.Texto,
                questions = new List<QuestionConfig>()
            };
            phases.Add(newPhase);
            phaseList.Rebuild();

            // Selecionar a nova fase automaticamente
            phaseList.SetSelection(phases.Count - 1);
        };

        addQBtn.clicked -= OnAddQuestionClicked;
        addQBtn.clicked += OnAddQuestionClicked;

        saveBtn.clicked += () =>
        {
            string path = Path.Combine(Application.persistentDataPath, "phases.json");
            File.WriteAllText(path, JsonConvert.SerializeObject(phases, Formatting.Indented));
            Debug.Log($"JSON salvo em {path}");
        };

        LoadIfExists();
    }

    // NOVO MÉTODO: Para importar mídia usando SimpleFileBrowser
    private void ImportMediaForQuestion(QuestionConfig question, Label mediaLabel)
    {
        // Configurar filtros baseado no tipo de quiz
        SetupFileBrowserForQuizType(question.questionType);

        FileBrowser.ShowLoadDialog(
            (paths) =>
            {
                if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
                {
                    string selectedPath = paths[0];
                    string fileName = System.IO.Path.GetFileName(selectedPath);
                    string destDir = Path.Combine(Application.persistentDataPath, "ImportedMedia");

                    if (!Directory.Exists(destDir))
                        Directory.CreateDirectory(destDir);

                    string destPath = Path.Combine(destDir, fileName);

                    try
                    {
                        File.Copy(selectedPath, destPath, true);
                        question.mediaPath = destPath;
                        mediaLabel.text = fileName;
                        Debug.Log($"Mídia importada: {fileName}");
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Erro ao importar mídia: {e.Message}");
                    }
                }
            },
            () => Debug.Log("Seleção de mídia cancelada"),
            FileBrowser.PickMode.Files,
            false,
            null,
            null,
            "Selecionar Mídia"
        );
    }

    // NOVO MÉTODO: Configurar filtros do FileBrowser
    private void SetupFileBrowserForQuizType(QuizType quizType)
    {
        switch (quizType)
        {
            case QuizType.Imagem:
                FileBrowser.SetFilters(true,
                    new FileBrowser.Filter("Imagens", ".png", ".jpg", ".jpeg", ".gif", ".bmp"));
                break;
            case QuizType.Audio:
                FileBrowser.SetFilters(true,
                    new FileBrowser.Filter("Áudio", ".mp3", ".wav", ".ogg", ".aac"));
                break;
            case QuizType.Video:
                FileBrowser.SetFilters(true,
                    new FileBrowser.Filter("Vídeo", ".mp4", ".mov", ".avi", ".webm", ".mkv"));
                break;
            default:
                FileBrowser.SetFilters(true,
                    new FileBrowser.Filter("Todos os arquivos", ".*"));
                break;
        }
    }

    // NOVO MÉTODO: Adicionar resposta a uma pergunta
    private void AddAnswerToQuestion(QuestionConfig question)
    {
        question.answers.Add($"Opção {question.answers.Count + 1}");
        questionList.Rebuild();
    }

    private void OnAddQuestionClicked()
    {
        int idx = phaseList.selectedIndex;
        if (idx >= 0)
        {
            var newQuestion = new QuestionConfig
            {
                questionText = "Nova pergunta",
                questionType = phases[idx].quizType,
                answers = new List<string> { "Opção 1", "Opção 2" },
                correctAnswerIndex = 0,
                mediaPath = ""
            };

            phases[idx].questions.Add(newQuestion);
            questionList.itemsSource = phases[idx].questions;
            questionList.Rebuild();
        }
    }

    void OnPhaseSelected(PhaseConfig p)
    {
        // Limpar listeners anteriores antes de adicionar novos
        phaseNameField.UnregisterValueChangedCallback(OnPhaseNameChanged);
        quizTypeField.UnregisterValueChangedCallback(OnQuizTypeChanged);

        phaseNameField.SetValueWithoutNotify(p.name);
        phaseNameField.RegisterValueChangedCallback(OnPhaseNameChanged);

        quizTypeField.SetValueWithoutNotify(p.quizType);
        quizTypeField.RegisterValueChangedCallback(OnQuizTypeChanged);

        questionList.itemsSource = p.questions;
        questionList.Rebuild();
    }

    // NOVOS MÉTODOS: Separar callbacks para evitar vazamentos de memória
    private void OnPhaseNameChanged(ChangeEvent<string> evt)
    {
        if (currentPhase != null)
        {
            currentPhase.name = evt.newValue;
            phaseList.Rebuild(); // Atualizar a lista para mostrar o novo nome
        }
    }

    private void OnQuizTypeChanged(ChangeEvent<System.Enum> evt)
    {
        if (currentPhase != null)
        {
            currentPhase.quizType = (QuizType)evt.newValue;
            questionList.Rebuild(); // Atualizar as perguntas para refletir o novo tipo
        }
    }

    void LoadIfExists()
    {
        string path = Path.Combine(Application.persistentDataPath, "phases.json");
        if (File.Exists(path))
        {
            try
            {
                phases = JsonConvert.DeserializeObject<List<PhaseConfig>>(File.ReadAllText(path));
                if (phases == null)
                    phases = new List<PhaseConfig>();

                phaseList.itemsSource = phases;
                phaseList.Rebuild();

                // Selecionar primeira fase se existir
                if (phases.Count > 0)
                {
                    phaseList.SetSelection(0);
                }

                Debug.Log($"Fases carregadas: {phases.Count}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erro ao carregar fases: {e.Message}");
                phases = new List<PhaseConfig>();
            }
        }
    }

    void OnDisable()
    {
        // Limpar listeners ao desabilitar o componente
        if (phaseNameField != null)
            phaseNameField.UnregisterValueChangedCallback(OnPhaseNameChanged);
        if (quizTypeField != null)
            quizTypeField.UnregisterValueChangedCallback(OnQuizTypeChanged);
    }
}