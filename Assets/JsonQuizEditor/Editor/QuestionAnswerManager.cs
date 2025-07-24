// Add and edit questions.
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace JsonQuizEditor.Scripts
{
    public class QuestionAnswerManager
    {
        float xCenterArea = 10;
		float widthMainEditor = 0.0f;
		float heightMainEditor = 0.0f;
        EditorTools editorTools = null;
        Texture2D lessLightColorTexture2D;
        Texture2D addImageTexture2D;
        Texture2D newTexture2D = null;
        Texture2D separationTexture2D;
        Texture2D closeWindowTexture2D;
        Texture2D trashTexture2D;
        Texture2D viewTexture2D;
        float xPositionColorBackground = 20;       
        float xPositionWorkArea = 2;
        float yPositionWorkArea = 30;
        float xPositionImageConst = 0;
        float xPositionImage = 30;
        float yPositionImage = 50;
        float xPositionImageLabel = 5;
        float yPositionImageLabel = 130;
        float xPositionDeleteImageButton = 0;
        float yPositionDeleteImageButton = 150;
        float xPositionQuestionField = 200;
        float yPositionQuestionField = 30;
        float yPositionQuestionText = 30;
        string questionName; // Question name.
        string tags; // Tags separated by commas.
        string previousQuestionName; 
        string previousTags;
        string imageInformation = null;
        float xPositionQuestionButton = 384;
        float xPositionSeparation = 172;
        float yPositionSeparation = 20;
        float xPositionCloseWinButton = 26;
        float yPositionCloseWinButton = 7;
        float yPositionAnswers = 100;
        QuestionAnswerEditor questionEditor = null; // Reference to the main editor.
        bool aTextureIsAdded = false;
        bool itIsNewQuestion = true;
        string filePathQuestionsJson = null; // Json configuration file for questions.
        string questionImagesFolder = null; // Folder containing the images that will be used in the questions.
        JsonManager jsonManager = null; // Read and write json file.
        string isTheKeyToQuestion; // Question Key.
        string [] typesAnswersSubMenu = { "Single Selection", "True or False" }; 
        string [] textOrImageSubMenu = { "Text", "Image" }; 
        int indexSubMenu = 0;
        int previousIndexSubMenu = 0;
        List<int> indexTextOrImageSubMenu = null;
        List<int> previousIndexTextOrImageSubMenu = null;
        List<bool> togglesAnswers = null; 
        List<string> answers = null; // List of the answers.
        List<bool> answersRemoved = null;
        float separationHeightAnswers = 46;
		int heightItem = 47;
		int heightScroll = 0;
        Vector2 answersScrollPosition; 
        float frameWidth = 550;
        int answerCounter = 2;
        int meter = 0;
        bool enableAddAnswer = true;
        float widthTextAreaConst = 294;
        float widthTextArea = 200;
        float answerTextFieldConst = 270;
        float widthAnswerButton = 160;
        float widthAnswerButtonConst = 200;
        float answerTextField = 200;
        int selectSomeImage = -2;
        GlobalEditorVariables.QuestionTextureData questionTextureData; 
        List<GlobalEditorVariables.QuestionTextureData> answersTextureData; 
       
        // Initialize variables.
        public QuestionAnswerManager () {
			widthMainEditor = GlobalEditorVariables.widthEditor - 2;
            heightMainEditor = GlobalEditorVariables.heightEditor - 2;
            xCenterArea = 2;
            xPositionWorkArea = 6;
            yPositionWorkArea = 30;
            xPositionColorBackground = 4;
            xPositionImage = xPositionImageConst;
            yPositionImage = 2;
            xPositionImageLabel = 9;
            yPositionImageLabel = 125;
            xPositionDeleteImageButton = 52;
            yPositionDeleteImageButton = 148;
            xPositionQuestionField = 140;
            yPositionQuestionField = 0;
            yPositionQuestionText = 96;
            xPositionQuestionButton = 0;
            xPositionSeparation = 134;
            yPositionSeparation = 2;
            xPositionCloseWinButton = 26;
            yPositionCloseWinButton = 7;
            yPositionAnswers = 270;
			heightScroll = heightItem;
            frameWidth = widthMainEditor - 8;
            widthTextArea = widthTextAreaConst;
            answerTextField = answerTextFieldConst;
            widthAnswerButton = widthAnswerButtonConst;
            answerCounter = GlobalEditorVariables.minimumNumberAnswers;
            enableAddAnswer = true;                                      
			if (jsonManager == null) {
				jsonManager = new JsonManager ();
				DeserializeEditorConfig desEditorConfig = jsonManager.ReadEditorConfigJson ();
				if (desEditorConfig != null) {
					filePathQuestionsJson = desEditorConfig.filePathQuestionsJson;
					questionImagesFolder = desEditorConfig.questionImagesFolder;				
				}
			}
            if (togglesAnswers == null) {
                togglesAnswers = new List<bool>();
                for (int index = 0; index < GlobalEditorVariables.maximumNumberAnswers; index++) {
                    togglesAnswers.Add (false);
                }
            }
            if (answers == null) {
                answers = new List<string>();
                for (int index = 0; index < GlobalEditorVariables.maximumNumberAnswers; index++) {
                    answers.Add ("");
                }
            }
            if (indexTextOrImageSubMenu == null) {
                indexTextOrImageSubMenu = new List<int>();
                for (int index = 0; index < GlobalEditorVariables.maximumNumberAnswers; index++) {
                    indexTextOrImageSubMenu.Add (0);
                }
            }
            if (previousIndexTextOrImageSubMenu == null) {
                previousIndexTextOrImageSubMenu = new List<int>();
                for (int index = 0; index < GlobalEditorVariables.maximumNumberAnswers; index++) {
                    previousIndexTextOrImageSubMenu.Add (0);
                }
            }            
            if (answersRemoved == null) {
                answersRemoved = new List<bool>();
                for (int index = 0; index < GlobalEditorVariables.maximumNumberAnswers; index++) {
                    answersRemoved.Add (false);
                }
            }
            if (answersTextureData == null) {
                answersTextureData = new List<GlobalEditorVariables.QuestionTextureData>();                
                for (int index = 0; index < GlobalEditorVariables.maximumNumberAnswers; index++) {
                    GlobalEditorVariables.QuestionTextureData answer;
                    answer.id = -2;
                    answer.texture = null;
                    answer.imageName = "";
                    answersTextureData.Add (answer);
                }
            }                                                
            DefaultQuestionData ();                                                                                      
            if (editorTools == null) {		            
                editorTools = new EditorTools ();
            }            
            LoadColorTexture2D ();
            LoadImagesFromEditorFolder ();        
        }

        // Reference to the editor where the list of questions will be displayed.
        public void SetQuestionEditorObjectReference (QuestionAnswerEditor obj) {
            questionEditor = obj;
        }

        // Draw UI.
        public void CreateQuestionGUI () {
            GUIStyle answersFieldStyle = new GUIStyle (EditorStyles.label);
            answersFieldStyle.alignment = TextAnchor.MiddleRight;
            GUIStyle questionInfoStyle = new GUIStyle (EditorStyles.label);
            questionInfoStyle.alignment = TextAnchor.MiddleLeft;             
			GUIStyle editQuestionsStyle = new GUIStyle (EditorStyles.textArea);
            GUIStyle titleStyle = new GUIStyle (EditorStyles.textField);
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.fontStyle = FontStyle.Normal;
            titleStyle.margin = new RectOffset (0, 0, 0, 0);
            titleStyle.padding = new RectOffset (0, 0, 0, 0);            
            GUIStyle questionFieldStyle = new GUIStyle (EditorStyles.textField);
            questionFieldStyle.alignment = TextAnchor.UpperLeft;
            questionFieldStyle.padding = new RectOffset (5, 5, 5, 5);
            GUIStyle ascendingCounterTextStyle = new GUIStyle (EditorStyles.label);
            ascendingCounterTextStyle.alignment = TextAnchor.MiddleLeft;
            GUIStyle answerFieldStyle = new GUIStyle (EditorStyles.textField);
            answerFieldStyle.alignment = TextAnchor.MiddleLeft;
            answerFieldStyle.padding = new RectOffset (5, 5, 5, 5);
            GUIStyle trueFalseFieldStyle = new GUIStyle (EditorStyles.textField);
            trueFalseFieldStyle.alignment = TextAnchor.MiddleCenter;              
			GUILayout.BeginArea(new Rect(xCenterArea, 0, widthMainEditor, heightMainEditor), editQuestionsStyle);
                GUILayout.BeginHorizontal("label");
                    GUILayout.FlexibleSpace();
                    string titleQuestion = "Create a Question";
                    if (!itIsNewQuestion) {
                        titleQuestion = "Edit Question";
                    }                                     
                    EditorGUILayout.LabelField(titleQuestion, titleStyle, 
                        GUILayout.Width(widthMainEditor - 9), GUILayout.Height(24));
                    GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if (lessLightColorTexture2D != null) {
                    GUI.DrawTexture(new Rect(xPositionWorkArea + xPositionImage - xPositionColorBackground, 
                        yPositionWorkArea, frameWidth, heightMainEditor - 32), lessLightColorTexture2D);
                }
                string titleAddQuestion = "Add question image";
                Texture2D questionTexture2D = addImageTexture2D;
                if (aTextureIsAdded) {
                    if (questionTextureData.id == -1 && questionTextureData.texture != null) {
                        questionTexture2D = questionTextureData.texture;
                    }
                    titleAddQuestion = "";
                }
                GUIStyle uploadFieldStyle = new GUIStyle (EditorStyles.textField);
                int fontSizeDefault = uploadFieldStyle.fontSize;
                if (selectSomeImage == -1) {
                    uploadFieldStyle = new GUIStyle (EditorStyles.helpBox);
                    uploadFieldStyle.fontSize = fontSizeDefault; 
                }
                uploadFieldStyle.alignment = TextAnchor.MiddleCenter;
                GUI.Label (
                    new Rect(xPositionWorkArea + xPositionImage, yPositionWorkArea + yPositionImage, 128, 174), 
                    "", uploadFieldStyle);
                bool addImageButton = GUI.Button (new Rect(xPositionWorkArea + xPositionImage, yPositionWorkArea + 
                    yPositionImage, 128, 128), new GUIContent(questionTexture2D, "Click here to add or update an image"));
                if (addImageButton) {
                    selectSomeImage = -1;
                    CreateQuestionImagesWindow ();
                } 
                GUI.Label (
                    new Rect(xPositionWorkArea + xPositionImage + xPositionImageLabel, yPositionWorkArea + 
                    yPositionImage + yPositionImageLabel, 160, 24), titleAddQuestion, questionInfoStyle);
                EditorGUI.BeginDisabledGroup (!aTextureIsAdded);    
                bool deleteButton = GUI.Button (
                    new Rect(xPositionWorkArea + xPositionImage + xPositionDeleteImageButton, yPositionWorkArea + 
                    yPositionImage + yPositionDeleteImageButton, 24, 24), new GUIContent(trashTexture2D, "Delete image"));
                if (deleteButton) {
                    aTextureIsAdded = false;
                    imageInformation = null;
                    questionTextureData.imageName = "";
                    questionTextureData.texture = null;                 
                }
                EditorGUI.EndDisabledGroup();              
                questionName = GUI.TextArea (
                    new Rect(xPositionWorkArea + xPositionImage + xPositionQuestionField, yPositionWorkArea + 
                    yPositionImage + yPositionQuestionField, widthTextArea, 100), questionName, questionFieldStyle);
                if ( !string.Equals(questionName, previousQuestionName) ) {
                    previousQuestionName = questionName;
                }
                GUI.Label (
                    new Rect(xPositionWorkArea + xPositionImage + xPositionQuestionField, yPositionWorkArea + 
                    yPositionImage + yPositionQuestionField + yPositionQuestionText, 160, 30), 
                    "Question text", questionInfoStyle);
                if (separationTexture2D != null) {    
                    GUI.DrawTexture(new Rect(xPositionWorkArea + xPositionImage + xPositionSeparation, 
                        yPositionWorkArea + yPositionSeparation, 1, 180), separationTexture2D);
                    GUI.DrawTexture(new Rect(xPositionWorkArea + xPositionImage, 
                        yPositionWorkArea + yPositionSeparation + 180, frameWidth - 8, 1), separationTexture2D);
                    GUI.DrawTexture(new Rect(xPositionWorkArea + xPositionImage, 
                        heightMainEditor - 45, frameWidth - 8, 1), separationTexture2D);
                }                    
                bool closeButton = GUI.Button (
                    new Rect(widthMainEditor - xPositionCloseWinButton, yPositionCloseWinButton, 18, 18), 
                    new GUIContent(closeWindowTexture2D, "Close"));
                if (closeButton) {
                    CloseQuestionManager ();       
                }
                GUI.Label (
                    new Rect(xPositionWorkArea + xPositionImage + xPositionQuestionField, yPositionWorkArea + 
                    yPositionImage + yPositionQuestionField + yPositionQuestionText + 20, 250, 30), 
                    "Add Tags, separate tags with commas", questionInfoStyle);
                tags = GUI.TextArea (
                    new Rect(xPositionWorkArea + xPositionImage + xPositionQuestionField, yPositionWorkArea + 
                    yPositionImage + yPositionQuestionField + yPositionQuestionText + 48, widthTextArea, 30), 
                    tags, questionFieldStyle);
                if ( !string.Equals(tags, previousTags) ) {
                    previousTags = tags;
                }
                EditorGUI.BeginDisabledGroup (!aTextureIsAdded);
                EditorGUI.EndDisabledGroup();
                GUI.Label (
                    new Rect(xPositionWorkArea + xPositionImage, 208, 160, 30), 
                    "Select Answer Type", questionInfoStyle);
                if (enableAddAnswer) {                       
                    string answerCounterField = meter.ToString () + " answers";    
                    GUI.Label (
                        new Rect(xPositionWorkArea + xPositionImage + frameWidth - 127, 213, 120, 20), 
                        answerCounterField, answersFieldStyle);                    
                    bool addAnswerButton = GUI.Button (
                        new Rect(xPositionWorkArea + xPositionImage + frameWidth - 92, 237, 85, 24),
                        "Add answer");
                    if (addAnswerButton) {
                        answerCounter++;
                        if (answerCounter > GlobalEditorVariables.maximumNumberAnswers) {  
                            answerCounter = GlobalEditorVariables.maximumNumberAnswers;
                        }
                    }
                }
                indexSubMenu = EditorGUI.Popup(
                    new Rect(xPositionWorkArea + xPositionImage, 238, 114, 20), indexSubMenu, typesAnswersSubMenu);
                if (previousIndexSubMenu != indexSubMenu) {
                    ResetAnswerType ();
                    previousIndexSubMenu = indexSubMenu;                    
                }
                // Select the type of answer.
                switch (indexSubMenu) {
                    case GlobalEditorVariables.idSingleSelectionAnswer: // Simple selection.
                        CreateSelectionAnswers (answerCounter, GlobalEditorVariables.minimumNumberAnswers, false);
                    break;
                    case GlobalEditorVariables.idTrueOrFalseAnswer: // True or False.
                        CreateTrueFalseAnswer ();
                    break;
                    default:
                        CreateSelectionAnswers (answerCounter, GlobalEditorVariables.minimumNumberAnswers, false);
                    break;                                                            
                }
                bool cancelButton = GUI.Button (
                    new Rect(xPositionWorkArea + xPositionImage + frameWidth - 250, heightMainEditor - 30, 80, 24),
                     "Cancel");
                if (cancelButton) {
                    CloseQuestionManager ();
                }
                string titleSaveButton = "Save";
                if (!itIsNewQuestion) {
                    titleSaveButton = "Update";
                }
                bool saveButton = GUI.Button (
                    new Rect(xPositionWorkArea + xPositionImage + frameWidth - 145, heightMainEditor - 30, 80, 24),
                     titleSaveButton);  
                if (saveButton) {
                    if (ValidateFormData ()) {
                        if ( !SaveQuestion (itIsNewQuestion) ) {
                            itIsNewQuestion = true;
                            editorTools.UnexpectedErrorOccurred ();
                        }
                    }
                }
                EditorGUI.BeginDisabledGroup (itIsNewQuestion);    
                bool createQuestionButon = GUI.Button (
                    new Rect(xPositionWorkArea + xPositionImage + xPositionQuestionButton,heightMainEditor - 30, 120, 24),
                     "Create a Question");
                if (createQuestionButon) {
                    DefaultQuestionData ();
                    ResetAnswerType ();
                }     
                EditorGUI.EndDisabledGroup();                                                                                                                                                                                                      
            GUILayout.EndArea();            
        }

        // Create a list of answers, you will need to indicate the correct answer.
        void CreateSelectionAnswers (int numberOfAnswers, int numberOfAnswersNotDeleted, bool singleOrMulti) {
            GUIStyle ascendingCounterTextStyle = new GUIStyle (EditorStyles.label);
            ascendingCounterTextStyle.alignment = TextAnchor.MiddleLeft;
            GUIStyle answerFieldStyle = new GUIStyle (EditorStyles.textField);
            answerFieldStyle.alignment = TextAnchor.MiddleLeft;
            answerFieldStyle.padding = new RectOffset (5, 5, 5, 5);                                   
            if (numberOfAnswersNotDeleted > numberOfAnswers) {
                numberOfAnswersNotDeleted = numberOfAnswers;
            }
            heightScroll = meter * heightItem;
            float heightScrollScreen = (heightMainEditor - 10) - (yPositionWorkArea + yPositionAnswers);
            if (heightScrollScreen < 0) {
                heightScrollScreen = 0;
                heightScroll = 0;
            }				
			answersScrollPosition = GUI.BeginScrollView(
				new Rect(xPositionWorkArea + xPositionImage, yPositionWorkArea + yPositionAnswers - 35, 
                frameWidth - 7, heightScrollScreen), answersScrollPosition, 
				new Rect(xPositionWorkArea + xPositionImage, 0, frameWidth - 37, heightScroll));
                meter = 0;
                for (int index = 0; index < numberOfAnswers; index++) {
                    if (answersRemoved != null) {
                        if (!answersRemoved [index]) {
                            float separationHeight = separationHeightAnswers * meter;
                            int ascendingCounter = meter + 1;                       
                            GUI.Label (
                                new Rect(xPositionWorkArea + xPositionImage, separationHeight + 7, 40, 40), 
                                ascendingCounter.ToString (), ascendingCounterTextStyle);                    
                            togglesAnswers [index] = EditorGUI.Toggle(new Rect(
                                xPositionWorkArea + xPositionImage + 20, separationHeight + 20, 20, 20), togglesAnswers [index]);                            
                            bool enableDeleteAnswer = true;
                            if (index > (numberOfAnswersNotDeleted - 1)) {
                                enableDeleteAnswer = false;
                            }
                            EditorGUI.BeginDisabledGroup (enableDeleteAnswer);
                            bool deleteAnswerButton = GUI.Button (
                                new Rect(xPositionWorkArea + xPositionImage + frameWidth - 54, separationHeight + 17, 24, 24), 
                                new GUIContent(trashTexture2D, "Delete an answer"));
                            if (deleteAnswerButton) {    
                                answersRemoved [index] = true;
                            }
                            EditorGUI.EndDisabledGroup();
                            if (togglesAnswers [index]) {
                                SelectSingleOrMultiToggle (index, togglesAnswers, answersRemoved, numberOfAnswers, singleOrMulti);
                            }                                              
                            indexTextOrImageSubMenu [index] = EditorGUI.Popup(
                                new Rect(xPositionWorkArea + xPositionImage + frameWidth - 115, separationHeight + 22, 56, 20), 
                                indexTextOrImageSubMenu [index], textOrImageSubMenu);
                            if (indexTextOrImageSubMenu [index] != previousIndexTextOrImageSubMenu [index]) {
                                previousIndexTextOrImageSubMenu [index] = indexTextOrImageSubMenu [index];
                                answers [index] = "";
                                if (answersTextureData != null) {
                                    GlobalEditorVariables.QuestionTextureData answer = answersTextureData [index];                                                                           
                                    answer.texture = null;
                                    answer.imageName = "";
                                    answersTextureData [index] = answer;
                                }                            
                            }
                            switch (indexTextOrImageSubMenu [index]) {
                                case GlobalEditorVariables.idTextSelection: // Text                        
                                    answers [index] = GUI.TextField (
                                        new Rect(xPositionWorkArea + xPositionImage + 45, separationHeight + 12, answerTextField, 32), 
                                        answers [index], answerFieldStyle);
                                break;
                                case GlobalEditorVariables.idImageSelection: // Image
                                    string imageName = null;
                                    bool enableImageViewer = true;
                                    Texture2D answerTexture2D = addImageTexture2D;
                                    if (answersTextureData != null) {
                                        GlobalEditorVariables.QuestionTextureData answer = answersTextureData [index];                                     
                                        if (answer.id == index && answer.texture != null) {                                        
                                            answerTexture2D = answer.texture;
                                            enableImageViewer = false;
                                            imageName = answer.imageName;
                                        } 
                                    }                            
                                    GUIStyle uploadFieldStyle = new GUIStyle (EditorStyles.textField);
                                    int fontSizeDefault = uploadFieldStyle.fontSize;
                                    if (selectSomeImage == index) {
                                        uploadFieldStyle = new GUIStyle (EditorStyles.helpBox);
                                        uploadFieldStyle.fontSize = fontSizeDefault; 
                                    }
                                    uploadFieldStyle.alignment = TextAnchor.MiddleCenter;
                                    GUI.Label (
                                        new Rect(xPositionWorkArea + xPositionImage + 45, separationHeight + 6, answerTextField, 44), 
                                        "", uploadFieldStyle);
                                    bool addImageButton = GUI.Button (
                                        new Rect(xPositionWorkArea + xPositionImage + 47, separationHeight + 8, widthAnswerButton, 40), 
                                        new GUIContent(answerTexture2D, "Click here to add or update an image"));
                                    if (addImageButton) {
                                        selectSomeImage = index;
                                        CreateQuestionImagesWindow ();
                                    }
                                    EditorGUI.BeginDisabledGroup (enableImageViewer);
                                    bool viewButton = GUI.Button (
                                        new Rect(xPositionWorkArea + xPositionImage + answerTextField +16, separationHeight + 17, 24, 24), 
                                        new GUIContent(viewTexture2D, "View image"));
                                    if (viewButton) {
                                    CreateImageViewWindow (questionImagesFolder, imageName);                 
                                    }                                
                                    EditorGUI.EndDisabledGroup();                                                                                 
                                break;
                                default:
                                    
                                break;                                                            
                            }
                            meter++;
                        }
                    }
                }
            GUI.EndScrollView();                
        }

        // Select a correct answer.
        void SelectSingleOrMultiToggle (int index, List<bool> toggles, List<bool> removed, int counter, bool singleOrMulti) {
            if (singleOrMulti) {

            } else {
                for (int loop = 0;loop < counter; loop++) {
                    toggles [loop] = false;
                    if (loop == index && !removed [loop]) {
                        toggles [loop] = true;
                    }
                }
            }
        }

        // Create the true or false answer,  you will need to indicate the correct answer.
        void CreateTrueFalseAnswer () {
            GUIStyle trueFalseFieldStyle = new GUIStyle (EditorStyles.textField);
            trueFalseFieldStyle.alignment = TextAnchor.MiddleCenter;
            enableAddAnswer = false;       
            int index = 0;        
            togglesAnswers [index] = EditorGUI.Toggle(new Rect(
                xPositionWorkArea + xPositionImage, yPositionWorkArea + 250, 20, 20), togglesAnswers [index]);                            
            if (togglesAnswers [index]) {
                togglesAnswers [index + 1] = false;
            }
            GUI.Label (new Rect(xPositionWorkArea + xPositionImage + 30, yPositionWorkArea + 245, 100, 25), 
                "TRUE", trueFalseFieldStyle);
            index = 1;
            togglesAnswers [index] = EditorGUI.Toggle(new Rect(
                xPositionWorkArea + xPositionImage, yPositionWorkArea + 277, 20, 20), togglesAnswers [index]);                            
            if (togglesAnswers [index]) {
                togglesAnswers [index - 1] = false;
            }
            GUI.Label (new Rect(xPositionWorkArea + xPositionImage + 30, yPositionWorkArea + 273, 100, 25), 
                "FALSE", trueFalseFieldStyle);                             
        }

        // Reset default variables.
        public void ResetAnswerType () {          
            if (togglesAnswers == null || answersRemoved == null || answersTextureData == null) {
                return;
            }
            enableAddAnswer = true;            
            answerCounter = GlobalEditorVariables.minimumNumberAnswers;
            for (int index = 0; index < togglesAnswers.Count; index++) {
                togglesAnswers [index] = false;
            }
            for (int index = 0; index < answersRemoved.Count; index++) {
                answersRemoved [index] = false;
            }
            for (int index = 0; index < indexTextOrImageSubMenu.Count; index++) {
                indexTextOrImageSubMenu [index] = GlobalEditorVariables.idTextSelection;
                previousIndexTextOrImageSubMenu [index] = GlobalEditorVariables.idTextSelection;
            }
            for (int index = 0; index < answersTextureData.Count; index++) {
                GlobalEditorVariables.QuestionTextureData answer = answersTextureData [index];
                answer.id = -2;
                answer.texture = null;
                answer.imageName = "";
                answersTextureData [index] = answer;
            }
            for (int index = 0; index < answers.Count; index++) {
                answers [index] = "";
            }                       
        }

        public void DefaultQuestionData () {
            questionName = "";
            tags = "";
            previousQuestionName = "";
            previousTags = "";
            aTextureIsAdded = false;
            itIsNewQuestion = true;
            imageInformation = null;
            isTheKeyToQuestion = "";
            selectSomeImage = -2;
            meter = 0;
            answerCounter = GlobalEditorVariables.minimumNumberAnswers;
            indexSubMenu = GlobalEditorVariables.idSingleSelectionAnswer;
            questionTextureData.imageName = "";
            questionTextureData.texture = null;
        }

        // Add a selected image from the image gallery to the question.
        public void AddImageToQuestionFromGallery (string imageName) {
			if (imageName == null || selectSomeImage == -2) {
				return;
			}
			imageInformation = imageName;
            aTextureIsAdded = false;
            newTexture2D = editorTools.GetTexture2DFromStreamingFolder (questionImagesFolder, imageName);
			if (newTexture2D != null) {
                aTextureIsAdded = true;
                if (selectSomeImage == -1) {
                    questionTextureData.id = selectSomeImage;                    
                    questionTextureData.texture = newTexture2D;
                    questionTextureData.imageName = imageName;
                } else {
                    if (selectSomeImage >= 0) {
                        if (answersTextureData != null) {
                            GlobalEditorVariables.QuestionTextureData answer = answersTextureData [selectSomeImage];
                            answer.id = selectSomeImage;
                            answer.texture = newTexture2D;
                            answer.imageName = imageName;
                            answersTextureData [selectSomeImage] = answer;
                        }
                    }
                }
			} else {
				editorTools.FileNotExistDialog (imageName);
			}                       
        }

		bool ValidateFormData () {
			bool status = true;
			bool errors = false;
			int count = 0;
			string errorMsg = "You need to resolve these issues to continue.\n";
            bool statusName = !string.IsNullOrWhiteSpace(questionName);
            if (!statusName && questionTextureData.texture == null) {
                status = false;
                errors = true;
            }
			if (errors) {
				count++;
				errorMsg += count.ToString () + ". "+"Enter a question name or image.\n";
                errors = false;
			}
            if (indexSubMenu == GlobalEditorVariables.idSingleSelectionAnswer) {
                for (int index = 0; index < answerCounter; index++) {
                    if (answersRemoved != null && indexTextOrImageSubMenu != null) {
                        if (!answersRemoved [index]) {
                            if (indexTextOrImageSubMenu [index] == GlobalEditorVariables.idTextSelection) { // text
                                bool statusAnswer = !string.IsNullOrWhiteSpace(answers [index]);
                                if (!statusAnswer) {
                                    status = false;
                                    errors = true;
                                }
                            }
                            if (indexTextOrImageSubMenu [index] == GlobalEditorVariables.idImageSelection) { // texture
                                GlobalEditorVariables.QuestionTextureData answer = answersTextureData [index];
                                if (answer.texture == null) {
                                    status = false;
                                    errors = true;                            
                                }
                            }
                        }
                    }
                }
                if (errors) {
                    count++;
                    errorMsg += count.ToString () + ". "+"Enter your answers.\n";
                    errors = false;
                }
                bool statusTogglesAnswers = false;            
                for (int index = 0; index < answerCounter; index++) {
                    if (answersRemoved != null) {
                        if (!answersRemoved [index]) {
                            if (togglesAnswers [index]) {
                                statusTogglesAnswers = true;
                            }
                        }
                    }
                }
                if (!statusTogglesAnswers) {
                    status = false;
                    errors = true; 
                }
                if (errors) {
                    count++;
                    errorMsg += count.ToString () + ". "+"Select the correct answer.\n";
                    errors = false;
                }
            }
            if (indexSubMenu == GlobalEditorVariables.idTrueOrFalseAnswer) {
                if (togglesAnswers [0] == false && togglesAnswers [1] == false) {
                    count++;
                    status = false;
                    errorMsg += count.ToString () + ". "+"Select the correct answer.\n";                     
                }
            }
			if (!status) {
				if (EditorUtility.DisplayDialog("Errors", errorMsg, "Done")) {}
			}			
			return status;
		}

        bool SaveQuestion (bool isNew) {
            bool status = false;            
            SerializeQuestions serializeQuestions = ScriptableObject.CreateInstance <SerializeQuestions> ();
			List <QuestionProperties> questionProperties = jsonManager.ReadQuestionsJson (filePathQuestionsJson);
            if (isNew) { // New question.   
                if (questionProperties == null) {
                    questionProperties = new List<QuestionProperties> ();
                }
                QuestionProperties question = new QuestionProperties ();
                question.imageName = questionTextureData.imageName;
                question.questionName = questionName;
                question.tags = tags;
                question.typeAnswer = indexSubMenu;
                question.answers = new List<string>();
                question.textOrImage = new List<int>();
                question.selectedAnswers = new List<bool>();
                for (int index = 0; index < answerCounter; index++) {
                    if (answersRemoved != null) {
                        if (!answersRemoved [index]) {
                            if (indexTextOrImageSubMenu [index] == GlobalEditorVariables.idTextSelection) {
                                question.answers.Add (answers [index]);
                            }
                            if (indexTextOrImageSubMenu [index] == GlobalEditorVariables.idImageSelection) {
                                question.answers.Add (answersTextureData [index].imageName);
                            }                        
                            question.textOrImage.Add (indexTextOrImageSubMenu [index]);
                            question.selectedAnswers.Add (togglesAnswers [index]);
                        }
                    }
                }
                question.trueOrFalse = togglesAnswers [0]; 
                isTheKeyToQuestion = editorTools.GetRandomKey ();
                question.key = isTheKeyToQuestion;
                questionProperties.Add (question);
                serializeQuestions.Questions = questionProperties;
                itIsNewQuestion = false;
                status = jsonManager.WriteQuestionsJson (filePathQuestionsJson, serializeQuestions);
            } else { // Update question.
                if (questionProperties != null) {
                    foreach (QuestionProperties question in questionProperties) {
                        if (question != null) {
                            if ( string.Equals(question.key, isTheKeyToQuestion) ) {                
                                question.imageName = questionTextureData.imageName;
                                question.questionName = questionName;
                                question.tags = tags;
                                question.typeAnswer = indexSubMenu;
                                question.answers = new List<string>();
                                question.textOrImage = new List<int>();
                                question.selectedAnswers = new List<bool>();
                                for (int index = 0; index < answerCounter; index++) {
                                    if (answersRemoved != null) {
                                        if (!answersRemoved [index]) {
                                            if (indexTextOrImageSubMenu [index] == GlobalEditorVariables.idTextSelection) {
                                                question.answers.Add (answers [index]);
                                            }
                                            if (indexTextOrImageSubMenu [index] == GlobalEditorVariables.idImageSelection) {
                                                question.answers.Add (answersTextureData [index].imageName);
                                            }                                        
                                            question.textOrImage.Add (indexTextOrImageSubMenu [index]);
                                            question.selectedAnswers.Add (togglesAnswers [index]);
                                        }
                                    }
                                }
                                question.trueOrFalse = togglesAnswers [0]; 
                                serializeQuestions.Questions = questionProperties;
                                status = jsonManager.WriteQuestionsJson (filePathQuestionsJson, serializeQuestions);
                                break;
                            }                            
                        }
                    }
                }
            }
            if (status && questionEditor != null) {
                questionEditor.ReloadFromQuestionAnswerManager (isNew, isTheKeyToQuestion);
            }
            return status;
        }

        // Load the information for a question in the editor.
        public void QuestionToEditFromEditor (string key) {
            DefaultQuestionData ();
            ResetAnswerType ();
            isTheKeyToQuestion = key;
            List <QuestionProperties> questionProperties = jsonManager.ReadQuestionsJson (filePathQuestionsJson);
            foreach (QuestionProperties question in questionProperties) {
                if (question != null) {
                    if ( string.Equals(question.key, isTheKeyToQuestion) ) { // Select the question by your key.
                        itIsNewQuestion = false;
                        questionName = question.questionName;
                        tags = question.tags;
                        indexSubMenu = question.typeAnswer;
                        previousIndexSubMenu = indexSubMenu;  
                        answerCounter = question.answers.Count;
                        meter = answerCounter;
                        for (int index = 0; index < answerCounter; index++) {
                            answers [index] = question.answers [index];
                            indexTextOrImageSubMenu [index] = question.textOrImage [index];
                            previousIndexTextOrImageSubMenu [index] = indexTextOrImageSubMenu [index];
                            togglesAnswers [index] = question.selectedAnswers [index];
                            if (answersTextureData != null && indexTextOrImageSubMenu [index] == GlobalEditorVariables.idImageSelection) {
                                GlobalEditorVariables.QuestionTextureData answer = answersTextureData [index];
                                answer.id = index;
                                answer.texture = editorTools.GetTexture2DFromStreamingFolder (
                                    questionImagesFolder, answers [index]);
                                answer.imageName = answers [index];
                                answersTextureData [index] = answer;
                            }
                        }                         
                        questionTextureData.id = -1;
                        questionTextureData.imageName = question.imageName;
                        questionTextureData.texture = null;
                        if (questionTextureData.imageName != null) {
                            if (questionTextureData.imageName.Length > 0) {
                                questionTextureData.texture = 
                                    editorTools.GetTexture2DFromStreamingFolder (
                                    questionImagesFolder, questionTextureData.imageName);
                                if (questionTextureData.texture != null) {
                                    aTextureIsAdded = true;
                                }    
                            }
                        }
                        if (indexSubMenu == GlobalEditorVariables.idTrueOrFalseAnswer) {
                            if (question.trueOrFalse) {
                                togglesAnswers [0] = true;
                                togglesAnswers [1] = false; 
                            } else {
                                togglesAnswers [0] = false;
                                togglesAnswers [1] = true;
                            }
                        }
                    }                            
                }
            }          
        }

		void LoadColorTexture2D () {
            lessLightColorTexture2D = editorTools.GetColorTexture2D (new Color (0.91f, 0.91f, 0.91f, 1.0f), 2, 2);
		}

        // Load textures.
		void LoadImagesFromEditorFolder () {
			addImageTexture2D = editorTools.GetTexture2DFromEditorFolder (GlobalEditorVariables.addImageNameImage);
            separationTexture2D = editorTools.GetTexture2DFromEditorFolder (GlobalEditorVariables.separationNameImage);
            closeWindowTexture2D = editorTools.GetTexture2DFromEditorFolder (GlobalEditorVariables.closeWindowNameImage);
            trashTexture2D = editorTools.GetTexture2DFromEditorFolder (GlobalEditorVariables.trashNameImage);
            viewTexture2D = editorTools.GetTexture2DFromEditorFolder (GlobalEditorVariables.viewNameImage);
		}

        // Close editor.
        void CloseQuestionManager () {
            if (questionEditor != null) {
                questionEditor.ResetEnvironment ();
            }
        }

        // Create the image gallery window.
        void CreateQuestionImagesWindow () {
            JsonQuizWindow winCurrentEditor = null;
            JsonQuizWindow [] winEditors = Resources.FindObjectsOfTypeAll<JsonQuizWindow>();
            if (winEditors != null) {
                foreach (JsonQuizWindow winEditor in winEditors) {
                    if (winEditor != null) {
                        if (winEditor.idWindow == GlobalEditorVariables.idJsonQuizWindow) {
                            winCurrentEditor = winEditor;                        
                        }
                        if (winEditor.idWindow == GlobalEditorVariables.idImageGalleryWindow) {
                            winCurrentEditor = null;
                            break;                        
                        }
                    }                                         
                }
            }
            if (winCurrentEditor != null) {            
                winCurrentEditor.CreateImageGalleryWindow (this);
            }                            
        }      

        // Creates the image viewer window.
        void CreateImageViewWindow (string folderPath, string nameFile) {
            if (folderPath == null || nameFile == null) {
                return;
            }            
            JsonQuizWindow winCurrentEditor = null;
            JsonQuizWindow [] winEditors = Resources.FindObjectsOfTypeAll<JsonQuizWindow>();
            if (winEditors != null) {
                foreach (JsonQuizWindow winEditor in winEditors) {
                    if (winEditor != null) {
                        if (winEditor.idWindow == GlobalEditorVariables.idJsonQuizWindow) {
                            winCurrentEditor = winEditor;                        
                        }
                        if (winEditor.idWindow == GlobalEditorVariables.idImageViewWindow) {
                            winCurrentEditor = null;
                            if (winEditor.imageViewWindow != null) {
                                winEditor.imageViewWindow.LoadImagesFromStreamingFolder (folderPath, nameFile);
                                winEditor.Repaint ();
                            }
                            break;                        
                        }                        
                    }                                         
                }
            }
            if (winCurrentEditor != null) {           
                winCurrentEditor.CreateImageViewWindow (folderPath, nameFile);
            }                            
        }  

        // Resize the editor window.
        public void ChangeSizeWindow (Rect position) {
			Rect windowSize = position;
            float maxWidthResize = 150.0f;		
			if (windowSize.width > GlobalEditorVariables.widthEditor) {
				float subWidth = windowSize.width - GlobalEditorVariables.widthEditor;
                if (subWidth > maxWidthResize) {
				    widthMainEditor = windowSize.width - 4;
                    xPositionImage = (widthMainEditor / 2) - (frameWidth / 2) - 4; 
                    if (frameWidth <= GlobalEditorVariables.widthEditor) {
                        widthMainEditor = GlobalEditorVariables.widthEditor + maxWidthResize - 6;
                        frameWidth = widthMainEditor - 4;
                        widthTextArea = widthTextAreaConst + maxWidthResize - 2;
                        answerTextField = answerTextFieldConst + maxWidthResize - 4;
                        widthAnswerButton = widthAnswerButtonConst + maxWidthResize;
                    }    
                } else {
                    widthMainEditor = GlobalEditorVariables.widthEditor + subWidth - 6;
                    frameWidth = widthMainEditor - 4;
                    widthTextArea = widthTextAreaConst + subWidth - 2;
                    answerTextField = answerTextFieldConst + subWidth - 4;
                    widthAnswerButton = widthAnswerButtonConst + subWidth; 
                }
			} else {
				widthMainEditor = GlobalEditorVariables.widthEditor - 4;
                xPositionImage = xPositionImageConst;
                frameWidth = widthMainEditor - 4;
                widthTextArea = widthTextAreaConst;
                answerTextField = answerTextFieldConst;
                widthAnswerButton = widthAnswerButtonConst; 
			}
			if (windowSize.height > GlobalEditorVariables.heightEditor) {
				float subHeight = windowSize.height - GlobalEditorVariables.heightEditor;
				heightMainEditor = windowSize.height - 2;
			} else {
				heightMainEditor = GlobalEditorVariables.heightEditor - 2;
			}            
        }        
    }
}