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

            // CORREÇÃO: Mostrar "Pergunta 1", "Pergunta 2", etc.
            string displayText = $"Pergunta {i + 1}";
            if (!string.IsNullOrEmpty(perguntas[i].texto) && perguntas[i].texto.Trim() != "")
            {
                string preview = perguntas[i].texto.Length > 20 ?
                    perguntas[i].texto.Substring(0, 20) + "..." :
                    perguntas[i].texto;
                displayText += $" - {preview}";
            }

            txt.text = displayText;
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
        // NOVO: Validar antes de salvar
        List<string> errors = new List<string>();

        for (int i = 0; i < perguntas.Count; i++)
        {
            var pergunta = perguntas[i];

            // Validar texto da pergunta
            if (string.IsNullOrEmpty(pergunta.texto) || pergunta.texto.Trim() == "")
            {
                errors.Add($"• Pergunta {i + 1}: Texto não pode estar vazio");
            }

            // Validar opções de resposta
            if (pergunta.alternativas.Count < 2)
            {
                errors.Add($"• Pergunta {i + 1}: Deve ter pelo menos 2 alternativas");
            }
            else
            {
                for (int j = 0; j < pergunta.alternativas.Count; j++)
                {
                    if (string.IsNullOrEmpty(pergunta.alternativas[j]) || pergunta.alternativas[j].Trim() == "")
                    {
                        errors.Add($"• Pergunta {i + 1}, Alternativa {j + 1}: Não pode estar vazia");
                    }
                }
            }

            // Validar índice da resposta correta
            if (pergunta.indiceCorreto < 0 || pergunta.indiceCorreto >= pergunta.alternativas.Count)
            {
                errors.Add($"• Pergunta {i + 1}: Resposta correta inválida");
            }

            // Validar mídia se necessário
            if (pergunta.tipoMidia != TipoMidia.Texto && string.IsNullOrEmpty(pergunta.caminhoMidia))
            {
                errors.Add($"• Pergunta {i + 1}: Mídia é obrigatória para o tipo {pergunta.tipoMidia}");
            }
        }

        if (errors.Count > 0)
        {
            string errorMessage = "Não é possível salvar. Corrija os seguintes erros:\n\n" + string.Join("\n", errors);
            Debug.LogWarning(errorMessage);
            return;
        }

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