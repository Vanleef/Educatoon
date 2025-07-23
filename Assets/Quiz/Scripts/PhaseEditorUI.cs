using UnityEngine;
using UnityEngine.UIElements;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using SFB;

public class PhaseEditorUI : MonoBehaviour
{
    [SerializeField] UIDocument uiDoc;
    [SerializeField] VisualTreeAsset questionTemplate;

    private List<PhaseConfig> phases = new List<PhaseConfig>();
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
        phaseList.onSelectionChange += _ =>
        {
            var sel = phaseList.selectedItem as PhaseConfig;
            if (sel != null) OnPhaseSelected(sel);
        };

        questionList.makeItem = () => questionTemplate.Instantiate();
        questionList.bindItem = (e, i) =>
        {
            var q = phases[phaseList.selectedIndex].questions[i];

            // Texto da pergunta
            var tf = e.Q<TextField>("questionText");
            tf.value = q.questionText;
            tf.RegisterValueChangedCallback(evt => q.questionText = evt.newValue);

            // Botão de importar mídia
            var importBtn = e.Q<Button>("importMediaBtn");
            var mediaLabel = e.Q<Label>("mediaFileLabel");
            importBtn.text = "Importar Mídia";
            importBtn.clicked -= null;
            importBtn.clicked += () =>
            {
                var extensions = new[] {
                    new ExtensionFilter("Imagens", "png", "jpg", "jpeg"),
                    new ExtensionFilter("Áudio", "mp3", "wav", "ogg"),
                    new ExtensionFilter("Vídeo", "mp4", "mov", "avi")
                };
                string[] paths = StandaloneFileBrowser.OpenFilePanel("Selecione a mídia", "", extensions, false);
                if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
                {
                    string selectedPath = paths[0];
                    string fileName = System.IO.Path.GetFileName(selectedPath);
                    string destDir = Path.Combine(Application.persistentDataPath, "ImportedMedia");
                    if (!Directory.Exists(destDir))
                        Directory.CreateDirectory(destDir);
                    string destPath = Path.Combine(destDir, fileName);
                    File.Copy(selectedPath, destPath, true);
                    q.mediaPath = destPath;
                    mediaLabel.text = fileName;
                }
            };
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
                answerRow.Add(answerField);
                answerRow.Add(correctToggle);
                answersContainer.Add(answerRow);
            }

            // Botão para adicionar nova opção de resposta
            var addAnswerBtn = e.Q<Button>("addAnswerBtn");
            addAnswerBtn.UnregisterCallback<ClickEvent>(addAnswerBtn.userData as EventCallback<ClickEvent>);
            EventCallback<ClickEvent> callback = evt =>
            {
                q.answers.Add("");
                questionList.Rebuild();
            };
            addAnswerBtn.userData = callback;
            addAnswerBtn.RegisterCallback(callback);
        };

        // Botões
        addPhaseBtn.clicked += () =>
        {
            phases.Add(new PhaseConfig());
            phaseList.Rebuild();
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

    private void OnAddQuestionClicked()
    {
        int idx = phaseList.selectedIndex;
        if (idx >= 0)
        {
            phases[idx].questions.Add(new QuestionConfig());
            questionList.itemsSource = phases[idx].questions;
            questionList.Rebuild();
        }
    }

    void OnPhaseSelected(PhaseConfig p)
    {
        phaseNameField.SetValueWithoutNotify(p.name);
        phaseNameField.RegisterValueChangedCallback(evt => p.name = evt.newValue);

        quizTypeField.SetValueWithoutNotify(p.quizType);
        quizTypeField.RegisterValueChangedCallback(evt =>
        {
            p.quizType = (QuizType)evt.newValue;
            questionList.Rebuild();
        });

        questionList.itemsSource = p.questions;
        questionList.Rebuild();
    }

    void LoadIfExists()
    {
        string path = Path.Combine(Application.persistentDataPath, "phases.json");
        if (File.Exists(path))
        {
            phases = JsonConvert.DeserializeObject<List<PhaseConfig>>(File.ReadAllText(path));
            phaseList.itemsSource = phases;
            phaseList.Rebuild();
        }
    }
}