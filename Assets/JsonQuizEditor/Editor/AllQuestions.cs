using System.Collections.Generic;
using UnityEngine;

namespace JsonQuizEditor.Scripts
{
    // Includes all the answers to a question.
    public class AllAnswers
    {
        public List<string> answers = null; // Includes all the answers, text or image name.
        public List<int> textOrImage = null; // Content type of answers, 0 = text , 1 = imagen.
        public List<bool> selectedAnswers = null; // Answer selected as correct, true = is right.       
    }

    // Includes all the questions and answers.
    public class AllQuestions
    {        
        public List<Texture2D> texture = null; // List of question textures.
        public List<bool> isTexture = null; // Indicates whether the texture needs to be loaded, true = yes texture loaded , false = texture was not loaded.
        public List<string> imageName = null; // List of images of the questions.
        public List<string> questionName = null; // List of questions text. 
        public List<string> tags = null; // Tag list.
        public List<int> typeAnswer = null; // List of answer types, 0 = simple selection , 1 = true or false. 
        public List<string> typeAnswerStr = null; 
        public List<AllAnswers> allAnswers = null; // List of answers.
        public List<bool> trueOrFalse = null; // The question is true or false.
        public List<string> key = null; // List of key questions.

        // Initialize variables.
        public AllQuestions () {
            if (texture == null) {
                texture = new List<Texture2D>();
            }
            if (isTexture == null) {
                isTexture = new List<bool>();
            }                         
            if (imageName == null) {
                imageName = new List<string>();
            }
            if (questionName == null) {
                questionName = new List<string>();
            }
            if (tags == null) {
                tags = new List<string>();
            }
            if (typeAnswer == null) {
                typeAnswer = new List<int>();
            }
            if (typeAnswerStr == null) {
                typeAnswerStr = new List<string>();
            }
            if (allAnswers == null) {
                allAnswers = new List<AllAnswers>();
            }
            if (trueOrFalse == null) {
                trueOrFalse = new List<bool>();
            }                                                                         
            if (key == null) {
                key = new List<string>();
            }
         }
    }
}