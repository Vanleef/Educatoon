// Image viewer.
using UnityEngine;

namespace JsonQuizEditor.Scripts
{
    [System.Serializable]
    public class ImageViewWindow : ISerializationCallbackReceiver
    {
        EditorTools editorTools = null;
        Texture2D answerTexture2D = null;
        float widthTexture = 0;
        float heightTexture = 0;
        public bool doNotShowGUI = false;

        // Initialize variables.
        public ImageViewWindow () {
            widthTexture = GlobalEditorVariables.widthImageView;
            heightTexture = GlobalEditorVariables.heightImageView;            
            if (editorTools == null) {		            
                editorTools = new EditorTools ();
            }
        }

		public void OnBeforeSerialize () {

		}
		
		public void OnAfterDeserialize () {
			doNotShowGUI = true;
		}

        // Draw UI.
        public void ImageViewGUI () {
			if (doNotShowGUI) {
				return;
			}            
            if (answerTexture2D != null) {
                GUI.DrawTexture(new Rect(0, 0, widthTexture, heightTexture), answerTexture2D, ScaleMode.ScaleToFit);
            }
        }

        // Load texture.
        // string folderPath = folder of images.
        // string nameFile = image name.
        public void LoadImagesFromStreamingFolder (string folderPath, string nameFile) {
            if (folderPath == null || nameFile == null) {
                return;
            }
            answerTexture2D = editorTools.GetTexture2DFromStreamingFolder (folderPath, nameFile);
            answerTexture2D.Apply ();
        }

        // Resize the editor window.
        public void ChangeSizeWindow (Rect position) {
            Rect windowSize = position;
            widthTexture = windowSize.width;
            heightTexture = windowSize.height;
        }
    }
}
