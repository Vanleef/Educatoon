// Editor settings.
namespace JsonQuizEditor.Scripts
{    
    [System.Serializable]
    public class DeserializeQuestionEditorConfig
    {
        public int  enumPopup = 0; // Number of questions to be displayed in the editor.
        public int  xImagesSeparation = 0; // X Position on the image element separator.
        public int  xQuestionsSeparation = 0; // X Position on the image questions separator.
        public int  xTagsSeparation = 0; // X Position on the tags element separator.
        public int  xOptionsSeparation = 0; // X Position on the options element separator.
        public int  enumPopupImageGallery = 0; // Number of images to be displayed in the image gallery.
    }
}