// Set the location of the json file and the destination folder of the images.
using UnityEngine;
using UnityEditor;

namespace JsonQuizEditor.Scripts
{
    public class EditorSettings 
    {
        float xCenterArea = 10;
		float widthMainEditor = 0.0f;
		float heightMainEditor = 0.0f;
        float xPositionCloseWinButton = 26;
        float yPositionCloseWinButton = 7;
        Texture2D closeWindowTexture2D;
        EditorTools editorTools = null;
        QuestionAnswerEditor questionEditor = null; // Reference to the list of questions.               
        JsonManager jsonManager = null; // Reading and writing of the json file.
        string filePathQuestionsJson; // Path of the json file that contains the questions.
        string questionsJsonFromBrowser = null;
        string questionImagesFolder; // Folder containing the images used by the questions.
        string  imagesInFolderFromBrowser = null;
        string titleEditorSettings = "Settings";
        string titleQuestionFile = "Open or save the json question file.";
   

        // Initialize variables.
        public EditorSettings () {
			widthMainEditor = GlobalEditorVariables.widthEditor - 2;
            heightMainEditor = GlobalEditorVariables.heightEditor - 2;
            xCenterArea = 2;
            xPositionCloseWinButton = 26;
            yPositionCloseWinButton = 7;
            if (editorTools == null) {		            
                editorTools = new EditorTools ();
            }                        
			if (jsonManager == null) {
				jsonManager = new JsonManager ();
			}            
            CheckConfiguration ();
            LoadImagesFromEditorFolder ();
        }

        // Draw UI.
        public void EditorSettingsGUI () {
            GUIStyle questionInfoStyle = new GUIStyle (EditorStyles.label);
            questionInfoStyle.alignment = TextAnchor.MiddleLeft;             
			GUIStyle editQuestionsStyle = new GUIStyle (EditorStyles.textArea);
            GUIStyle titleStyle = new GUIStyle (EditorStyles.textField);
            titleStyle.alignment = TextAnchor.MiddleCenter;
            titleStyle.fontStyle = FontStyle.Normal;
            titleStyle.margin = new RectOffset (0, 0, 0, 0);
            titleStyle.padding = new RectOffset (0, 0, 0, 0);
            GUIStyle fileQuestionStyle = new GUIStyle (EditorStyles.textField);
            fileQuestionStyle.alignment = TextAnchor.MiddleLeft;
            fileQuestionStyle.padding = new RectOffset (5, 5, 0, 0);
            GUIStyle infoHelpBoxStyle = new GUIStyle (EditorStyles.helpBox);
            infoHelpBoxStyle.alignment = TextAnchor.MiddleLeft;            
			GUILayout.BeginArea(new Rect(xCenterArea, 0, widthMainEditor, heightMainEditor), editQuestionsStyle);
                GUILayout.BeginHorizontal("label");
                    GUILayout.FlexibleSpace();                                                      
                    EditorGUILayout.LabelField(titleEditorSettings, titleStyle, 
                        GUILayout.Width(widthMainEditor - 9), GUILayout.Height(24));
                    GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                bool closeButton = GUI.Button (
                    new Rect(widthMainEditor - xPositionCloseWinButton, yPositionCloseWinButton, 18, 18), 
                    new GUIContent(closeWindowTexture2D, "Close"));
                if (closeButton) {
                    CloseEditorSetting ();      
                }
                EditorGUILayout.LabelField(titleQuestionFile);                
                GUILayout.BeginHorizontal();
                // Open the json file.                
                filePathQuestionsJson = EditorGUILayout.TextField (filePathQuestionsJson, fileQuestionStyle, GUILayout.Height(24));
                if (GUILayout.Button("File Browser", GUILayout.Width(110), GUILayout.Height(24))) {
                    questionsJsonFromBrowser = EditorUtility.SaveFilePanelInProject(titleQuestionFile, "Questions.json", "json",
                    "Please enter a file name to open or save the questions.");
                    if (!string.IsNullOrWhiteSpace (questionsJsonFromBrowser)) {
                        filePathQuestionsJson = questionsJsonFromBrowser;
                    }                                
                }                
                GUILayout.EndHorizontal();
                EditorGUILayout.LabelField ("Default file location: Assets/StreamingAssets/Json/Questions.json",infoHelpBoxStyle);
                EditorGUILayout.LabelField("");
                EditorGUILayout.LabelField("Open the location of the images.");
                GUILayout.BeginHorizontal();                
                    questionImagesFolder = EditorGUILayout.TextField (questionImagesFolder, fileQuestionStyle, GUILayout.Height(24));
                    if (GUILayout.Button("Folder Browser", GUILayout.Width(110), GUILayout.Height(24))) {
                        imagesInFolderFromBrowser = EditorUtility.SaveFolderPanel("Open the location of the images.", "", "");
                        if (!string.IsNullOrWhiteSpace (imagesInFolderFromBrowser)) {
                            string projectDataPath = Application.dataPath;
                            if (imagesInFolderFromBrowser.Length > projectDataPath.Length) {
                                string splitStr = imagesInFolderFromBrowser.Substring (projectDataPath.Length);
                                questionImagesFolder = GlobalEditorVariables.idAssetsFolder + splitStr.Clone ();
                            }
                        }                                
                    }                
                GUILayout.EndHorizontal();
                EditorGUILayout.LabelField ("Default folder location: Assets/StreamingAssets/Images",infoHelpBoxStyle);
                EditorGUILayout.LabelField("");
                EditorGUILayout.LabelField ("Save and Restart the Json Editor for the changes to take effect.", questionInfoStyle);                                   
                GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button ("Cancel", GUILayout.Width(80), GUILayout.Height(24))) {
                        CloseEditorSetting ();
                    }
                    EditorGUILayout.LabelField("", GUILayout.Width(20));                           
                    if (GUILayout.Button ("Save", GUILayout.Width(80), GUILayout.Height(24))) {
                        if (string.IsNullOrWhiteSpace (filePathQuestionsJson) || string.IsNullOrWhiteSpace (questionImagesFolder)) {
                            editorTools.UnexpectedErrorOccurred ();
                        } else {
                            SerializeEditorConfig editorConfig = ScriptableObject.CreateInstance <SerializeEditorConfig> ();
                            editorConfig.filePathQuestionsJson = filePathQuestionsJson;
                            editorConfig.questionImagesFolder = questionImagesFolder;
                            jsonManager.WriteEditorConfigJson (editorConfig);
                        }
                    }
                GUILayout.EndHorizontal();                                                                            
            GUILayout.EndArea();                                                                          
        }

        // Load default settings.
        void CheckConfiguration () {
            DeserializeEditorConfig desEditorConfig = jsonManager.ReadEditorConfigJson ();
            if (desEditorConfig == null) {
                // Save the default values.
                SerializeEditorConfig editorConfig = ScriptableObject.CreateInstance <SerializeEditorConfig> ();
                editorConfig.filePathQuestionsJson = Application.dataPath + GlobalEditorVariables.idStreamingFolder + 
                    GlobalEditorVariables.fileQuestionsJson;
                filePathQuestionsJson = editorConfig.filePathQuestionsJson;
                editorConfig.questionImagesFolder = Application.dataPath + GlobalEditorVariables.idStreamingFolder + 
                    GlobalEditorVariables.idImagesFolder;
                questionImagesFolder = editorConfig.questionImagesFolder;
                jsonManager.WriteEditorConfigJson (editorConfig);
            } else {
                // Load json file path.
                filePathQuestionsJson = desEditorConfig.filePathQuestionsJson;
                // Load the images folder.
                questionImagesFolder = desEditorConfig.questionImagesFolder;                
            }
        }

		public void LoadImagesFromEditorFolder () {
            closeWindowTexture2D = editorTools.GetTexture2DFromEditorFolder (GlobalEditorVariables.closeWindowNameImage);
		}

        // Reference to the question editor, in order to access the methods.
        public void SetQuestionEditorObjectReference (QuestionAnswerEditor obj) {
            questionEditor = obj;
        }

        // Close the editor.
        void CloseEditorSetting () {
            if (questionEditor != null) {
                questionEditor.ResetEnvironment ();
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
                } else {
                    widthMainEditor = GlobalEditorVariables.widthEditor + subWidth - 6;
                }
			} else {
				widthMainEditor = GlobalEditorVariables.widthEditor - 4;
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
