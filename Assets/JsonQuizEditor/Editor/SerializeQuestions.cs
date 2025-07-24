// Serialize the questions.
using System.Collections.Generic;
using UnityEngine;

namespace JsonQuizEditor.Scripts
{
    [System.Serializable]
    public class SerializeQuestions : ScriptableObject
    {
        public List <QuestionProperties> Questions = null; // Question list.
        
        void OnEnable () {
            if (Questions == null) {
                Questions = new List <QuestionProperties>();
            }            
        }

        public void AddQuestion (QuestionProperties question) {
            if (question == null || Questions == null) {
                return;
            }
            Questions.Add (question);
        }        
    }
}