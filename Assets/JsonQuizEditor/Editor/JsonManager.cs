// Read and write json file.
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace JsonQuizEditor.Scripts
{
    public class JsonManager
    {
        // Returns the list of questions and answers from the json file.
        public List <QuestionProperties> ReadQuestionsJson (string filePathJson) {
            if (filePathJson == null) {
                return null;
            }
            if (!File.Exists(filePathJson)) {
                return null;
            }
            System.IO.FileInfo fileInfo = null;
            try {
                fileInfo = new System.IO.FileInfo (filePathJson);
            } catch (Exception e) {
                string error = e.Message;
                return null;
            }             
            if ((int)fileInfo.Length < 10) {
                return null;
            }            
			string jsonStr = null;
            try {
                jsonStr = File.ReadAllText (filePathJson);    
            } catch (Exception e) {
                string error = e.Message;
                return null;
            }            
            if (jsonStr == null) {
                return null;
            }
            if (jsonStr.Length < 10) {
                return null;
            }
            DeserializeQuestions deserializeQuestions = null;
            try {
                deserializeQuestions = JsonUtility.FromJson <DeserializeQuestions> (jsonStr);
            }
            catch (Exception e) {
                string error = e.Message;
                return null;
            }
            if (deserializeQuestions == null) {
                return null;
            }
            if (deserializeQuestions.Questions == null) {
                return null;
            }            
            if (deserializeQuestions.Questions.Count == 0) {
                return null;
            }
            return deserializeQuestions.Questions;
        } 

        // Write the questions in json file.
        public bool WriteQuestionsJson (string filePathJson, SerializeQuestions objData) { 
            if (objData == null || filePathJson == null) {
                return false;
            }
		    string jsonStr = null;
            try {
                jsonStr = JsonUtility.ToJson (objData);
                File.WriteAllText (filePathJson, jsonStr);
            } catch (Exception e) {
                string error = e.Message;
                return false;
            }            
            return true;                             
        }

        // Read the settings from "Editor Default Resources/JsonQuizEditor/Config/Questions.json" file.
        public DeserializeQuestionEditorConfig ReadQuestionsConfigJson () {
			string filePathJson = DataPlatform.dataPath + GlobalEditorVariables.idEditorDefaultResourcesFolder + 
                GlobalEditorVariables.fileQuestionsEditorSettingsJson;            
            if (!File.Exists(filePathJson)) {
                return null;
            }
            System.IO.FileInfo fileInfo = null;
            try {
                fileInfo = new System.IO.FileInfo (filePathJson);
            } catch (Exception e) {
                string error = e.Message;
                return null;
            } 
            if ((int)fileInfo.Length < 10) {
                return null;
            }            
			string jsonStr = null;
            try {
                jsonStr = File.ReadAllText (filePathJson);    
            } catch (Exception e) {
                string error = e.Message;
                return null;
            }
            if (jsonStr == null) {
                return null;
            }
            if (jsonStr.Length < 10) {
                return null;
            }
            DeserializeQuestionEditorConfig deserializeQuestionEditorConfig = null;
            try {
                deserializeQuestionEditorConfig = JsonUtility.FromJson <DeserializeQuestionEditorConfig> (jsonStr);
            } catch (Exception e) {
                string error = e.Message;
                return null;
            }
            if (deserializeQuestionEditorConfig == null) {
                return null;
            }
            return deserializeQuestionEditorConfig;
        } 

        // Write the settings in "Editor Default Resources/JsonQuizEditor/Config/Questions.json" file.
        public bool WriteQuestionsConfigJson (SerializeQuestionEditorConfig objData) { 
            if (objData == null) {
                return false;
            }
		    string jsonStr = null;
            try {
                jsonStr = JsonUtility.ToJson (objData);   
            } catch (Exception e) {
                string error = e.Message;
                return false;
            }
            string filePathJson = DataPlatform.dataPath + GlobalEditorVariables.idEditorDefaultResourcesFolder + 
                GlobalEditorVariables.fileQuestionsEditorSettingsJson;              
            try {
                File.WriteAllText (filePathJson, jsonStr);  
            } catch (Exception e) {
                string error = e.Message;
                return false;
            }            
            return true;                             
        }

        // Read the settings from "Editor Default Resources/JsonQuizEditor/Config/Editor.json" file.
        public DeserializeEditorConfig ReadEditorConfigJson () {
			string filePathJson = DataPlatform.dataPath + GlobalEditorVariables.idEditorDefaultResourcesFolder + 
                GlobalEditorVariables.fileEditorSettingsJson;            
            if (!File.Exists(filePathJson)) {
                return null;
            }
            System.IO.FileInfo fileInfo = null;
            try {
                fileInfo = new System.IO.FileInfo (filePathJson);   
            } catch (Exception e) {
                string error = e.Message;
                return null;
            }
            if ((int)fileInfo.Length < 10) {
                return null;
            }            
			string jsonStr = null;
            try {
                jsonStr = File.ReadAllText (filePathJson);    
            } catch (Exception e) {
                string error = e.Message;
                return null;
            }
            if (jsonStr == null) {
                return null;
            }
            if (jsonStr.Length < 10) {
                return null;
            }
            DeserializeEditorConfig deserializeEditorConfig = null;
            try {
                deserializeEditorConfig = JsonUtility.FromJson <DeserializeEditorConfig> (jsonStr);
            } catch (Exception e) {
                string error = e.Message;
                return null;
            }
            if (deserializeEditorConfig == null) {
                return null;
            }
            return deserializeEditorConfig;
        }

        // Write the settings in "Editor Default Resources/JsonQuizEditor/Config/Editor.json" file.
        public bool WriteEditorConfigJson (SerializeEditorConfig objData) { 
            if (objData == null) {
                return false;
            }
		    string jsonStr = null;
            try {
                jsonStr = JsonUtility.ToJson (objData);    
            } catch (Exception e) {
                string error = e.Message;
                return false;
            }
            string filePathJson = DataPlatform.dataPath + GlobalEditorVariables.idEditorDefaultResourcesFolder + 
                GlobalEditorVariables.fileEditorSettingsJson;            
            try {
                File.WriteAllText (filePathJson, jsonStr);
            } catch (Exception e) {
                string error = e.Message;
                return false;
            }            
            return true;                             
        }        
    }
}