using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class MenuFasesUI : MonoBehaviour
{
    public Transform fasesListContainer;
    public GameObject faseBtnPrefab;
    public Button adicionarFaseBtn;
    public GameObject painelEdicaoFase;
    public PerguntasPanelUI perguntasPanelUI;

    public List<Fase> fases = new List<Fase>();
    public string fasesJsonPath;

    private void Start()
    {
        adicionarFaseBtn.onClick.AddListener(AdicionarFase);
        CarregarFases();
        AtualizarListaFases();
    }

    void CarregarFases()
    {
        if (File.Exists(fasesJsonPath))
        {
            string json = File.ReadAllText(fasesJsonPath);
            fases = JsonConvert.DeserializeObject<List<Fase>>(json);
        }
    }

    public void AtualizarListaFases()
    {
        foreach (Transform t in fasesListContainer) Destroy(t.gameObject);
        for (int i = 0; i < fases.Count; i++)
        {
            int idx = i;
            var btnGO = Instantiate(faseBtnPrefab, fasesListContainer);
            var txt = btnGO.GetComponentInChildren<TMP_Text>();
            txt.text = fases[i].nome;
            btnGO.GetComponent<Button>().onClick.AddListener(() =>
            {
                // Carregar cena correspondente
                UnityEngine.SceneManagement.SceneManager.LoadScene(fases[idx].cena);
            });
        }
    }

    void AdicionarFase()
    {
        var novaFase = new Fase { nome = "Nova Fase", perguntas = new List<Pergunta>() };
        fases.Add(novaFase);
        AtualizarListaFases();
        EditarFase(fases.Count - 1);
    }

    void EditarFase(int idx)
    {
        painelEdicaoFase.SetActive(true);
        perguntasPanelUI.perguntas = fases[idx].perguntas;
        perguntasPanelUI.faseJsonPath = Path.Combine(Application.persistentDataPath, $"fase_{fases[idx].nome}.json");
        perguntasPanelUI.AtualizarListaPerguntas();
        // Aqui você pode carregar a cena correspondente se quiser
        // SceneManager.LoadScene(fases[idx].nome);
    }

    public void VoltarParaMenuEdicao()
    {
        painelEdicaoFase.SetActive(false);
        AtualizarListaFases();
        // Salva lista de fases
        string json = JsonConvert.SerializeObject(fases, Formatting.Indented);
        File.WriteAllText(fasesJsonPath, json);
    }
}
