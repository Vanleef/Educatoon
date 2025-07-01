using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.EventSystems;

public class MiniGameConfigurator : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Dropdown tipoMiniGameDropdown;
    public GameObject quizPanel;
    public Button adicionarPerguntaButton;
    public Button exportarJSONButton;
    public Button importarJSONButton;
    public Transform perguntasContainer;
    public GameObject perguntaPrefab;

    [System.Serializable]
    public class Pergunta
    {
        public string texto;
        public string tipoMidia; // "texto", "imagem", "audio", "video"
        public string caminhoMidia;
        public List<string> alternativas;
        public int indiceCorreto;
    }

    [System.Serializable]
    public class QuizData
    {
        public List<Pergunta> perguntas = new List<Pergunta>();
    }

    private QuizData quizAtual = new QuizData();

    void Start()
    {
        tipoMiniGameDropdown.onValueChanged.AddListener(OnTipoMiniGameSelecionado);
        adicionarPerguntaButton.onClick.AddListener(AdicionarPergunta);
        exportarJSONButton.onClick.AddListener(ExportarParaJSON);
        importarJSONButton.onClick.AddListener(ImportarDeJSON);

        OnTipoMiniGameSelecionado(0); // inicializa com valor padrão
    }

    void OnTipoMiniGameSelecionado(int index)
    {
        string tipoSelecionado = tipoMiniGameDropdown.options[index].text.ToLower();
        quizPanel.SetActive(tipoSelecionado == "quiz");
    }

    void AdicionarPergunta()
    {
        GameObject novaPerguntaGO = Instantiate(perguntaPrefab, perguntasContainer);
        novaPerguntaGO.transform.SetAsLastSibling();
        PerguntaUI perguntaUI = novaPerguntaGO.GetComponent<PerguntaUI>();
        perguntaUI.OnSelecionarArquivo += SelecionarArquivoMidia;
    }

    void ExportarParaJSON()
    {
        quizAtual.perguntas.Clear();

        foreach (Transform t in perguntasContainer)
        {
            PerguntaUI perguntaUI = t.GetComponent<PerguntaUI>();
            quizAtual.perguntas.Add(perguntaUI.ObterPergunta());
        }

        string json = JsonUtility.ToJson(quizAtual, true);
        string path = Path.Combine(Application.dataPath, "quiz_exportado.json");
        File.WriteAllText(path, json);
        Debug.Log("JSON exportado para: " + path);
    }

    void ImportarDeJSON()
    {
#if UNITY_EDITOR
        string path = UnityEditor.EditorUtility.OpenFilePanel("Importar JSON", "", "json");
        if (!string.IsNullOrEmpty(path))
        {
            string json = File.ReadAllText(path);
            quizAtual = JsonUtility.FromJson<QuizData>(json);
            RecarregarUI();
        }
#else
        Debug.LogWarning("Importação de arquivos só está disponível no Editor.");
#endif
    }

    void RecarregarUI()
    {
        foreach (Transform t in perguntasContainer)
        {
            Destroy(t.gameObject);
        }

        foreach (Pergunta p in quizAtual.perguntas)
        {
            GameObject novaPerguntaGO = Instantiate(perguntaPrefab, perguntasContainer);
            PerguntaUI perguntaUI = novaPerguntaGO.GetComponent<PerguntaUI>();
            perguntaUI.DefinirPergunta(p);
            perguntaUI.OnSelecionarArquivo += SelecionarArquivoMidia;
        }
    }

    void SelecionarArquivoMidia(PerguntaUI perguntaUI)
    {
#if UNITY_EDITOR
        string caminho = UnityEditor.EditorUtility.OpenFilePanel("Selecionar mídia", "", "jpg,jpeg,png,mp3,mp4,wav");
        if (!string.IsNullOrEmpty(caminho))
        {
            perguntaUI.DefinirCaminhoMidia(caminho);
        }
#else
        Debug.LogWarning("Selecionar arquivos só está disponível no Editor.");
#endif
    }
}
