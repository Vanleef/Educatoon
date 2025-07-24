using UnityEngine;

namespace JsonQuizEditor.Scripts
{
    [System.Serializable]
    public class SerializeEditorConfig : ScriptableObject
    {
        public string filePathQuestionsJson; // Path of the json file that contains the questions.
        public string questionImagesFolder; // Folder containing the images that will be used in the questions.       
    }
}
