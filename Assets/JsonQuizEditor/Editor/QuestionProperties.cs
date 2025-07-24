// Question properties.
using System.Collections.Generic;

namespace JsonQuizEditor.Scripts
{
    [System.Serializable]
    public class QuestionProperties
    {
        public string imageName = null; // Image file name.
        public string questionName = null; // Question name.
        public string tags = null; // Tags separated by commas.
        public int typeAnswer = 0; // Type of answer: 0 = single selection , 1 = true or false.
        public List<string> answers = null; // List of the answers.
        public List<int> textOrImage = null; // Answers can be text or image. 0 = text , 1 = image.
        public List<bool> selectedAnswers = null; // List of correct answers.
        public bool trueOrFalse = false; // The answer is true or false.
        public string key = null; // Question key.
    }
}
