using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking; // ADICIONADO: Para UnityWebRequest
using SFB;

[RequireComponent(typeof(Button))]
public class CanvasSampleOpenFileTextMultiple : MonoBehaviour, IPointerDownHandler
{
    public Text output;

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void OnPointerDown(PointerEventData eventData) {
        UploadFile(gameObject.name, "OnFileUpload", ".txt", true);
    }

    // Called from browser
    public void OnFileUpload(string urls) {
        StartCoroutine(OutputRoutine(urls.Split(',')));
    }
#else
    //
    // Standalone platforms & editor
    //
    public void OnPointerDown(PointerEventData eventData) { }

    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        // var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "txt", true);
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", true);
        if (paths.Length > 0)
        {
            var urlArr = new List<string>(paths.Length);
            for (int i = 0; i < paths.Length; i++)
            {
                urlArr.Add(new System.Uri(paths[i]).AbsoluteUri);
            }
            StartCoroutine(OutputRoutine(urlArr.ToArray()));
        }
    }
#endif

    // CORREÇÃO: Substituir WWW por UnityWebRequest
    private IEnumerator OutputRoutine(string[] urlArr)
    {
        var outputText = "";

        for (int i = 0; i < urlArr.Length; i++)
        {
            using (UnityWebRequest loader = UnityWebRequest.Get(urlArr[i]))
            {
                yield return loader.SendWebRequest();

                if (loader.result == UnityWebRequest.Result.Success)
                {
                    outputText += loader.downloadHandler.text;
                }
                else
                {
                    outputText += "Error loading file " + (i + 1) + ": " + loader.error + "\n";
                }
            }
        }

        output.text = outputText;
    }
}