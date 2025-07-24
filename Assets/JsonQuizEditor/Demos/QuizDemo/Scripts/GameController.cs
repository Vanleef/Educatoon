// Quiz Demo.
using System.Collections.Generic;
using UnityEngine;

namespace JsonQuizDemo.Scripts
{
    public class GameController : MonoBehaviour
    {
        UIController uiController;
        QuestionsFromJson questionsFromJson; // List of questions and answers from the json file.
        List<string> keyQuestions = null; // Question key.
        int questionCounter = 0; // Question Counter. 
        int numberQuestions = 0; // Number of questions.
        public int correctAnswers = 0; // The right answer.

        void Awake () {
            uiController = GameObject.Find("UICanvas").GetComponent<UIController> ();
            string jsonQuestionFolder = null;
            #if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
                jsonQuestionFolder = GlobalDemoVariables.jsonQuestionFolderPC;
            #else
                #if UNITY_ANDROID
                    jsonQuestionFolder = GlobalDemoVariables.jsonQuestionFolderAndroid;
                #endif
            #endif            
            questionsFromJson = new QuestionsFromJson (jsonQuestionFolder, GlobalDemoVariables.jsonQuestionFileName);
        }

        // Get the tag list.
        public List<string> GetTagsFromGameController () {
            return questionsFromJson.GetTags ();
        }

        // The hints of the questions filtered by labels are obtained.
        public void SelectedTag (string tag) {
            // It returns the keys to the questions that are associated with a tag.
            keyQuestions = questionsFromJson.GetQuestionsKeysByTag (tag);
            numberQuestions = keyQuestions.Count;
            questionCounter = 0;
            correctAnswers = 0;
            NextQuestion ();
        }

        // Load next question.
        public void NextQuestion () {
            if (keyQuestions == null) {
                return;
            }
            if (questionCounter >= keyQuestions.Count) {
                // Percentage of responses that were answered.
                int percentage = (correctAnswers * 100) / numberQuestions;
                string percentageStr = percentage.ToString () + "%";
                uiController.SetPercentageGameOver (percentageStr);
                // Final score.
                int scoreEarned = correctAnswers * GlobalDemoVariables.score; 
                uiController.SetScoreGameOver (scoreEarned.ToString ());
                questionCounter = 0;
                correctAnswers = 0;
                uiController.EnableMenu (GlobalDemoVariables.gameOverMenuID);
                return;
            }            
            string key = keyQuestions [questionCounter];
            // Get the question by key. 
            OnlyOneQuestion onlyOneQuestion = questionsFromJson.GetOnlyOneQuestion (key);
            questionCounter++;
            if (onlyOneQuestion != null) {
                string score = questionCounter.ToString () + "/" + numberQuestions.ToString ();
                uiController.SetScore (score);                
                uiController.EnableTypesAnswer (onlyOneQuestion.typeAnswer, onlyOneQuestion);
            }
        }
    }
}