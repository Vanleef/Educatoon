// It displays in the console window the labels, questions and answers that the json file contains.
using System.Collections.Generic;
using UnityEngine;

namespace JsonSimpleDemo.Scripts
{
    public class SimpleDemo : MonoBehaviour
    {
        QuestionsFromJson questionsFromJson; // Question and answer list.

        // Start is called before the first frame update
        void Start () {
            ReadJsonQuestionFile ();
        }

        // Read the json file.
        void ReadJsonQuestionFile () {
            string jsonQuestionFolder = null;
            // Folder where the json file containing the questions and answers is located.
            // Depends on the Windows, Linux and Android platform.
            #if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
                jsonQuestionFolder = GlobalDemoVariables.jsonQuestionFolderPC;
            #else
                #if UNITY_ANDROID
                    jsonQuestionFolder = GlobalDemoVariables.jsonQuestionFolderAndroid;
                #endif
            #endif
            // Return the list of questions and answers.
            // The returned information is deserialized.            
            questionsFromJson = new QuestionsFromJson (jsonQuestionFolder, GlobalDemoVariables.jsonQuestionFileName);
            ReadTags ();
        }

        // Read the tags on the questions..
        void ReadTags () {
            List<string> tagList = questionsFromJson.GetTags ();
            for (int indexTag = 0; indexTag < tagList.Count; indexTag++) {
                string tag = tagList [indexTag];
                Debug.Log ("TAG = " + tag);
                ReadQuestions (tag);
            }
        }

        // Read the questions that have the same tag.
        void ReadQuestions (string tag) {
            // Returns the list of hints for the questions that have the same tag.
            List<string> keyQuestions = questionsFromJson.GetQuestionsKeysByTag (tag);
            for (int indexKey = 0; indexKey < keyQuestions.Count; indexKey++) {
                Debug.Log ("-> QUESTION = " + (indexKey + 1));
                string key = keyQuestions [indexKey];
                // Return the question to which the key corresponds.
                OnlyOneQuestion question = questionsFromJson.GetOnlyOneQuestion (key);
                string imageName = question.imageName; // Image file name.
                string questionName = question.questionName; // Question name.
                Debug.Log ("--> Image Name = " + imageName);
                Debug.Log ("--> Question Name = " + questionName);
                ReadAnswers (question);
            }
        }

        // Read the answers to the question.
        // There are two types of answers: "Simple Selection" and "True or False".
        void ReadAnswers (OnlyOneQuestion question) {
			switch (question.typeAnswer) { // Check the type of question.
                case GlobalDemoVariables.singleTypeAnswerID: // Type of answer: Simple selection.
                    Debug.Log ("---> TYPE ANSWER = Single Selection");
                    // Answer list.
                    AllAnswers allAnswers = question.allAnswers;
                    for (int index = 0; index < allAnswers.answers.Count; index++) {
                        string correctAnswer = "";
                        if (allAnswers.selectedAnswers [index]) { // Check if the answer is correct.
                            correctAnswer = " (correct answer)";
                        }
                        // The answer should contain text only.                        
                        if (allAnswers.textOrImage [index] == GlobalDemoVariables.idTextSelection) { 
                            Debug.Log ("----> Text = " + allAnswers.answers [index] + correctAnswer);
                        } 
                        // The answer should contain image only.
                        if (allAnswers.textOrImage [index] == GlobalDemoVariables.idImageSelection) { 
                            Debug.Log ("----> Image = " + allAnswers.answers [index] + correctAnswer);
                        }                                                 
                    }                
                break;
                case GlobalDemoVariables.trueOrFalseTypeAnswerID: // Type of answer: True or False.
                    Debug.Log ("---> TYPE ANSWER = True or False");
                    string trueOrFalse = "";
                    // Check if the answer is true or false.
                    if (question.trueOrFalse) {
                        trueOrFalse = "True";
                    } else {
                        trueOrFalse = "False";
                    }
                    Debug.Log ("----> True or False = " + trueOrFalse + " (correct answer)");
                break;
            }            
        }
    }
}