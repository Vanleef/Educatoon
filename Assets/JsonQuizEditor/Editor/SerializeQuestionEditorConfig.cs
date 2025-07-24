// Class used in question editor settings where the list of questions is displayed.
using UnityEngine;

namespace JsonQuizEditor.Scripts
{
    [System.Serializable]
    public class SerializeQuestionEditorConfig : ScriptableObject
    {
        public int  enumPopup = 0; // Number of questions to be displayed in the editor.
        public int  xImagesSeparation = 0; // X position on the image separator.
        public int  xQuestionsSeparation = 0; // X position on the questions separator.
        public int  xTagsSeparation = 0; // X position on the tags separator.
        public int  xOptionsSeparation = 0; // X position on the option separator.
        public int  enumPopupImageGallery = 0; // Number of images to be displayed in the image gallery.      
    }
}