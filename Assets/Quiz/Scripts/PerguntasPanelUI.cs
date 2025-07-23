using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class PerguntasPanelUI : MonoBehaviour
{
    public Transform perguntasContainer;           // Onde os PerguntaUI são instanciados
    public Transform perguntasListContainer;       // Container dos botões de perguntas
    public GameObject perguntaUIPrefab;            // Prefab do PerguntaUI
    public GameObject perguntaBtnPrefab;           // Prefab do botão lateral
    public Button adicionarPerguntaBtn;            // Botão para adicionar pergunta
    public Button salvarPerguntaBtn;               // Botão para salvar pergunta/atualizar lista/json
    public Button finalizarBtn;                    // Botão para finalizar edição e voltar ao menu

    public List<Pergunta> perguntas = new List<Pergunta>();
    public string faseJsonPath;                    // Caminho do JSON da fase

    private List<GameObject> perguntaUIs = new List<GameObject>();

    private void Start()
    {
        adicionarPerguntaBtn.onClick.AddListener(AdicionarPergunta);
        salvarPerguntaBtn.onClick.AddListener(SalvarPerguntas);
        finalizarBtn.onClick.AddListener(FinalizarEdicao);
        AtualizarListaPerguntas();
    }

    void AdicionarPergunta()
    {
        var novaPergunta = new Pergunta();
        perguntas.Add(novaPergunta);

        var perguntaGO = Instantiate(perguntaUIPrefab, perguntasContainer);
        perguntaGO.GetComponent<PerguntaUI>().Inicializar(novaPergunta);
        perguntaUIs.Add(perguntaGO);

        AtualizarListaPerguntas();
        SelecionarPergunta(perguntas.Count - 1);
    }

    public void AtualizarListaPerguntas()
    {
        foreach (Transform t in perguntasListContainer) Destroy(t.gameObject);
        for (int i = 0; i < perguntas.Count; i++)
        {
            int idx = i;
            var btnGO = Instantiate(perguntaBtnPrefab, perguntasListContainer);
            var txt = btnGO.GetComponentInChildren<TMP_Text>();
            txt.text = string.IsNullOrEmpty(perguntas[i].texto) ? $"Pergunta {i + 1}" : perguntas[i].texto;
            btnGO.GetComponent<Button>().onClick.AddListener(() => SelecionarPergunta(idx));
        }
    }

    void SelecionarPergunta(int idx)
    {
        for (int i = 0; i < perguntaUIs.Count; i++)
            perguntaUIs[i].SetActive(i == idx);
    }

    void SalvarPerguntas()
    {
        // Atualiza lista lateral
        AtualizarListaPerguntas();

        // Salva no JSON
        string json = JsonConvert.SerializeObject(perguntas, Formatting.Indented);
        File.WriteAllText(faseJsonPath, json);
        Debug.Log("Perguntas salvas em: " + faseJsonPath);
    }

    void FinalizarEdicao()
    {
        SalvarPerguntas();
        // Aqui você pode chamar o menu de edição de fases, atualizar a lista de fases, etc.
        // Exemplo: MenuFasesUI.Instance.VoltarParaMenuEdicao();
    }
}