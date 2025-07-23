using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using SFB;
using System.IO;

public class PerguntaUI : MonoBehaviour
{
    public TMP_InputField textoPerguntaInput;
    public TMP_Dropdown tipoPerguntaDropdown;
    public Button importarMidiaBtn;
    public TMP_Text nomeMidiaText;
    public Transform respostasContainer;
    public GameObject respostaPrefab;
    public Button adicionarRespostaBtn;

    private Pergunta pergunta;

    public void Inicializar(Pergunta pergunta)
    {
        this.pergunta = pergunta;
        textoPerguntaInput.text = pergunta.texto;
        tipoPerguntaDropdown.value = (int)pergunta.tipoMidia;
        nomeMidiaText.text = string.IsNullOrEmpty(pergunta.caminhoMidia) ? "Nenhum arquivo" : Path.GetFileName(pergunta.caminhoMidia);

        textoPerguntaInput.onValueChanged.AddListener(val => pergunta.texto = val);

        tipoPerguntaDropdown.onValueChanged.AddListener(val =>
        {
            pergunta.tipoMidia = (TipoMidia)val;
            AtualizarMidiaUI();
        });

        importarMidiaBtn.onClick.AddListener(() =>
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
                string fileName = Path.GetFileName(selectedPath);
                string destDir = Path.Combine(Application.persistentDataPath, "ImportedMedia");
                if (!Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);
                string destPath = Path.Combine(destDir, fileName);
                File.Copy(selectedPath, destPath, true);
                pergunta.caminhoMidia = destPath;
                nomeMidiaText.text = fileName;
            }
        });

        AtualizarMidiaUI();

        AtualizarRespostas();

        adicionarRespostaBtn.onClick.AddListener(() =>
        {
            pergunta.alternativas.Add("");
            AtualizarRespostas();
        });
    }

    private void AtualizarMidiaUI()
    {
        bool mostrar = pergunta.tipoMidia == TipoMidia.Imagem ||
                       pergunta.tipoMidia == TipoMidia.Audio ||
                       pergunta.tipoMidia == TipoMidia.Video;
        importarMidiaBtn.gameObject.SetActive(mostrar);
        nomeMidiaText.gameObject.SetActive(mostrar);
    }

    private void AtualizarRespostas()
    {
        foreach (Transform t in respostasContainer) Destroy(t.gameObject);
        for (int i = 0; i < pergunta.alternativas.Count; i++)
        {
            var respostaGO = Instantiate(respostaPrefab, respostasContainer);
            var respostaInput = respostaGO.GetComponentInChildren<TMP_InputField>();
            var toggleCorreta = respostaGO.GetComponentInChildren<Toggle>();
            respostaInput.text = pergunta.alternativas[i];
            int idx = i;
            respostaInput.onValueChanged.AddListener(val => pergunta.alternativas[idx] = val);
            toggleCorreta.isOn = pergunta.indiceCorreto == idx;
            toggleCorreta.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    pergunta.indiceCorreto = idx;
                    // Atualiza todos os toggles para garantir que só um fique marcado
                    AtualizarRespostas();
                }
            });
        }
    }
}