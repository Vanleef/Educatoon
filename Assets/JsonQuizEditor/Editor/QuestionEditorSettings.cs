// Read and save the editor settings in the json JsonQuizEditor/Config/Questions.json file.
using UnityEngine;

namespace JsonQuizEditor.Scripts
{
    public class QuestionEditorSettings
    {
        JsonManager jsonManager = null;
        public void InitEnvironment () {
            if (jsonManager == null) {
                jsonManager = new JsonManager ();
            }         
        }

        public void SaveEnumPopupSettings (int status) {
            DeserializeQuestionEditorConfig  deserializeQuestionEditorConfig = jsonManager.ReadQuestionsConfigJson ();
            if (deserializeQuestionEditorConfig != null) {
                GlobalEditorVariables.QuestionEditorConfig config = new GlobalEditorVariables.QuestionEditorConfig ();
                config.enumPopup = status;
                config.xImagesSeparation = deserializeQuestionEditorConfig.xImagesSeparation;
                config.xQuestionsSeparation = deserializeQuestionEditorConfig.xQuestionsSeparation;
                config.xTagsSeparation = deserializeQuestionEditorConfig.xTagsSeparation;
                config.xOptionsSeparation = deserializeQuestionEditorConfig.xOptionsSeparation;
                config.enumPopupImages = deserializeQuestionEditorConfig.enumPopupImageGallery;
                SaveJson (config);               
            }             
        }

        public GlobalEditorVariables.rowsOptions ReadEnumPopupSettings () {
            DeserializeQuestionEditorConfig  deserializeQuestionEditorConfig = jsonManager.ReadQuestionsConfigJson ();
            if (deserializeQuestionEditorConfig == null) {
                return GlobalEditorVariables.rowsOptions._5;
            }
            GlobalEditorVariables.rowsOptions savedData = GlobalEditorVariables.rowsOptions._5;
            int status = deserializeQuestionEditorConfig.enumPopup;
            switch (status) {
                case 5:
                    savedData = GlobalEditorVariables.rowsOptions._5;
                break;
                case 10:
                    savedData = GlobalEditorVariables.rowsOptions._10;
                break;
                case 20:
                    savedData = GlobalEditorVariables.rowsOptions._20;
                break;
                case 30:
                    savedData = GlobalEditorVariables.rowsOptions._30;
                break;
                default:
                    savedData = GlobalEditorVariables.rowsOptions._5;
                break;                                                                
            }
            return savedData;
        }

        public void SaveImagesSeparationSettings (float status) {
            DeserializeQuestionEditorConfig  deserializeQuestionEditorConfig = jsonManager.ReadQuestionsConfigJson ();
            if (deserializeQuestionEditorConfig != null) {
                GlobalEditorVariables.QuestionEditorConfig config = new GlobalEditorVariables.QuestionEditorConfig ();
                config.enumPopup = deserializeQuestionEditorConfig.enumPopup;
                config.xImagesSeparation = (int) status;
                config.xQuestionsSeparation = deserializeQuestionEditorConfig.xQuestionsSeparation;
                config.xTagsSeparation = deserializeQuestionEditorConfig.xTagsSeparation;
                config.xOptionsSeparation = deserializeQuestionEditorConfig.xOptionsSeparation;
                config.enumPopupImages = deserializeQuestionEditorConfig.enumPopupImageGallery;
                SaveJson (config);                 
            }              
        }

        public int ReadImagesSeparationSettings () {
            DeserializeQuestionEditorConfig  deserializeQuestionEditorConfig = jsonManager.ReadQuestionsConfigJson ();
            if (deserializeQuestionEditorConfig == null) {
                return 0;
            }
            return  deserializeQuestionEditorConfig.xImagesSeparation;           
        }

        public void SaveQuestionsSeparationSettings (float status) {
            DeserializeQuestionEditorConfig  deserializeQuestionEditorConfig = jsonManager.ReadQuestionsConfigJson ();
            if (deserializeQuestionEditorConfig != null) {
                GlobalEditorVariables.QuestionEditorConfig config = new GlobalEditorVariables.QuestionEditorConfig ();
                config.enumPopup = deserializeQuestionEditorConfig.enumPopup;
                config.xImagesSeparation = deserializeQuestionEditorConfig.xImagesSeparation;
                config.xQuestionsSeparation = (int) status;
                config.xTagsSeparation = deserializeQuestionEditorConfig.xTagsSeparation;
                config.xOptionsSeparation = deserializeQuestionEditorConfig.xOptionsSeparation;
                config.enumPopupImages = deserializeQuestionEditorConfig.enumPopupImageGallery;
                SaveJson (config);                 
            }             
        }

        public int ReadQuestionsSeparationSettings () {
            DeserializeQuestionEditorConfig  deserializeQuestionEditorConfig = jsonManager.ReadQuestionsConfigJson ();
            if (deserializeQuestionEditorConfig == null) {
                return 0;
            }
            return  deserializeQuestionEditorConfig.xQuestionsSeparation;              
        }

        public void SaveTagsSeparationSettings (float status) {
            DeserializeQuestionEditorConfig  deserializeQuestionEditorConfig = jsonManager.ReadQuestionsConfigJson ();
            if (deserializeQuestionEditorConfig != null) {
                GlobalEditorVariables.QuestionEditorConfig config = new GlobalEditorVariables.QuestionEditorConfig ();
                config.enumPopup = deserializeQuestionEditorConfig.enumPopup;
                config.xImagesSeparation = deserializeQuestionEditorConfig.xImagesSeparation;
                config.xQuestionsSeparation = deserializeQuestionEditorConfig.xQuestionsSeparation;
                config.xTagsSeparation = (int) status;
                config.xOptionsSeparation = deserializeQuestionEditorConfig.xOptionsSeparation;
                config.enumPopupImages = deserializeQuestionEditorConfig.enumPopupImageGallery;
                SaveJson (config);                 
            }           
        }

        public int ReadTagsSeparationSettings () {
            DeserializeQuestionEditorConfig  deserializeQuestionEditorConfig = jsonManager.ReadQuestionsConfigJson ();
            if (deserializeQuestionEditorConfig == null) {
                return 0;
            }
            return  deserializeQuestionEditorConfig.xTagsSeparation;              
        }

        public void SaveOptionsSeparationSettings (float status) {
            DeserializeQuestionEditorConfig  deserializeQuestionEditorConfig = jsonManager.ReadQuestionsConfigJson ();
            if (deserializeQuestionEditorConfig != null) {
                GlobalEditorVariables.QuestionEditorConfig config = new GlobalEditorVariables.QuestionEditorConfig ();
                config.enumPopup = deserializeQuestionEditorConfig.enumPopup;
                config.xImagesSeparation = deserializeQuestionEditorConfig.xImagesSeparation;
                config.xQuestionsSeparation = deserializeQuestionEditorConfig.xQuestionsSeparation;
                config.xTagsSeparation = deserializeQuestionEditorConfig.xTagsSeparation;
                config.xOptionsSeparation = (int) status;
                config.enumPopupImages = deserializeQuestionEditorConfig.enumPopupImageGallery;
                SaveJson (config);                 
            }             
        }

        public int ReadOptionsSeparationSettings () {
            DeserializeQuestionEditorConfig  deserializeQuestionEditorConfig = jsonManager.ReadQuestionsConfigJson ();
            if (deserializeQuestionEditorConfig == null) {
                return 0;
            }
            return  deserializeQuestionEditorConfig.xOptionsSeparation;               
        }        

        public void SaveEnumPopupImagesSettings (int status) {
            DeserializeQuestionEditorConfig  deserializeQuestionEditorConfig = jsonManager.ReadQuestionsConfigJson ();
            if (deserializeQuestionEditorConfig != null) {
                GlobalEditorVariables.QuestionEditorConfig config = new GlobalEditorVariables.QuestionEditorConfig ();
                config.enumPopupImages = status;
                config.enumPopup = deserializeQuestionEditorConfig.enumPopup;
                config.xImagesSeparation = deserializeQuestionEditorConfig.xImagesSeparation;
                config.xQuestionsSeparation = deserializeQuestionEditorConfig.xQuestionsSeparation;
                config.xTagsSeparation = deserializeQuestionEditorConfig.xTagsSeparation;
                config.xOptionsSeparation = deserializeQuestionEditorConfig.xOptionsSeparation;
                SaveJson (config);               
            }             
        }

        public GlobalEditorVariables.rowsOptions ReadEnumPopupImagesSettings () {
            DeserializeQuestionEditorConfig  deserializeQuestionEditorConfig = jsonManager.ReadQuestionsConfigJson ();
            if (deserializeQuestionEditorConfig == null) {
                return GlobalEditorVariables.rowsOptions._5;
            }
            GlobalEditorVariables.rowsOptions savedData = GlobalEditorVariables.rowsOptions._5;
            int status = deserializeQuestionEditorConfig.enumPopupImageGallery;
            switch (status) {
                case 5:
                    savedData = GlobalEditorVariables.rowsOptions._5;
                break;
                case 10:
                    savedData = GlobalEditorVariables.rowsOptions._10;
                break;
                case 20:
                    savedData = GlobalEditorVariables.rowsOptions._20;
                break;
                case 30:
                    savedData = GlobalEditorVariables.rowsOptions._30;
                break;
                default:
                    savedData = GlobalEditorVariables.rowsOptions._5;
                break;                                                                
            }
            return savedData;
        }

        void SaveJson (GlobalEditorVariables.QuestionEditorConfig config) {
            SerializeQuestionEditorConfig questionEditorConfig = ScriptableObject.CreateInstance <SerializeQuestionEditorConfig> ();
            questionEditorConfig.enumPopup = config.enumPopup;
            questionEditorConfig.xImagesSeparation = config.xImagesSeparation;
            questionEditorConfig.xQuestionsSeparation = config.xQuestionsSeparation;
            questionEditorConfig.xTagsSeparation = config.xTagsSeparation;
            questionEditorConfig.xOptionsSeparation = config.xOptionsSeparation;
            questionEditorConfig.enumPopupImageGallery = config.enumPopupImages;
            jsonManager.WriteQuestionsConfigJson (questionEditorConfig);
        }                
    }
}