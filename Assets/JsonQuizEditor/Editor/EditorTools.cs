using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System;

namespace JsonQuizEditor.Scripts 
{
    public class EditorTools
    {
        JsonManager jsonManager = new JsonManager ();

        // Returns a list of image names.
        // string folderPath = images folder.
        public List<string> LoadImagesFromStreamingFolder (string folderPath) {
            if (folderPath == null) {
                return null;
            }
            List <string> nameFiles = new List<string> ();
            DirectoryInfo directory = null;
            try {
                directory = new DirectoryInfo(folderPath);
            } catch (Exception e) {
                string error = e.ToString ();
                UnexpectedErrorOccurred ();
                return null;
            }            
            FileInfo [] filesDirectory = null;
            try {
                filesDirectory = directory.GetFiles();    
            } catch (Exception e) {
                string error = e.ToString ();
                UnexpectedErrorOccurred ();
                return null;
            }             
            if (filesDirectory == null) {
                return null;
            }
			foreach (var fileCurrrent in filesDirectory) {
                if (fileCurrrent.Exists) {
                    string valuetxt = fileCurrrent.Extension.ToLower();
                    // png and jpg image formats allowed.
                    if ( string.Equals(valuetxt, ".png") || string.Equals(valuetxt, ".jpg") || 
                        string.Equals(valuetxt, ".jpeg")) {
                        nameFiles.Add (fileCurrrent.Name);
                    }
                }				
			}
            return nameFiles;
        }

        // Get texture from image.
        public Texture2D GetTexture2DFromStreamingFolder (string folderPath, string nameFile) {
            if (folderPath == null || nameFile == null) {
                return null;
            }
            Texture2D nameTexture2D;
            string pathTexture = folderPath + "/" + nameFile;
            if (!File.Exists(pathTexture)) {
                return null;
            }
			var imgBytes = File.ReadAllBytes (pathTexture);
			nameTexture2D = new Texture2D(2, 2);
			nameTexture2D.LoadImage(imgBytes);
            return nameTexture2D;
        }

        // Get image size.
        public int GetFileSizeFromStreamingFolder (string folderPath, string nameFile) {
            if (folderPath == null || nameFile == null) {
                return 0;
            }
            string pathTexture = folderPath + "/" + nameFile;
            if (!File.Exists(pathTexture)) {
                return 0;
            }
            var fileInfo = new System.IO.FileInfo (pathTexture);
            return (int)fileInfo.Length;
        }

        // Returns the texture of an image located in the Editor folder, "Default Resources/JsonQuizEditor/Images".
        public Texture2D GetTexture2DFromEditorFolder (string nameFile) {
            if (nameFile == null) {
                return null;
            }
            Texture2D nameTexture2D;
            string pathTexture = Application.dataPath + GlobalEditorVariables.idEditorDefaultResourcesFolder + 
                GlobalEditorVariables.idEditorImagesFolder + nameFile;
            if (!File.Exists(pathTexture)) {
                return null;
            }
			var imgBytes = File.ReadAllBytes (pathTexture);
			nameTexture2D = new Texture2D(2, 2);
			nameTexture2D.LoadImage(imgBytes);
            return nameTexture2D;
        }        

        public bool DeleteFileStreamingFolder (string folderPath, string nameFile) {
            bool bValue = false;
            if (folderPath == null || nameFile == null) {
                return bValue;
            }             
            string pathTexture = folderPath + "/" + nameFile;
            if (!File.Exists(pathTexture)) {
                return false;
            }            
            bValue = FileUtil.DeleteFileOrDirectory(pathTexture);
            if (bValue) {
                string metaFile = pathTexture + ".meta";
                if (File.Exists(metaFile)) {
                    FileUtil.DeleteFileOrDirectory(metaFile);
                }
            }  
            AssetDatabase.Refresh ();
            return bValue;
        }

        // Search images in list,
        // Returns all matches in an exclusive AllImagesInGallery class.
        // List<string> imagesName = list containing the names of the images.
        // string searchText = search text in list.
        public AllImagesInGallery GetImageNameSearch (List<string> imagesName, string searchText) {            
            if (imagesName == null || searchText == null) {
                return null;
            }
            if (imagesName.Count == 0) {
                return null;
            }
            string searchTextLower = searchText.ToLower ();
            string [] nameSearch = new string [imagesName.Count];
            int bloop = 0;
            for (int aloop = 0;aloop < imagesName.Count; aloop++) {
                string imagesNameLower = imagesName [aloop].ToLower ();
                if (imagesNameLower.IndexOf (searchTextLower) != -1) {
                    nameSearch [bloop++] = imagesName [aloop];
                }
            }
            if (bloop > 0) {
                Array.Resize(ref nameSearch, bloop);
            } else {
                nameSearch = new string [0];
            }

            AllImagesInGallery allImagesInGallery = new AllImagesInGallery ();          
			foreach (string nameFile in nameSearch) {	
				allImagesInGallery.imageTexture.Add (new Texture2D(2, 2));
				allImagesInGallery.isTexture.Add (false);
				allImagesInGallery.image.Add (nameFile);
			}	
            return allImagesInGallery;
        }

        // Search questions.
        // Returns all matches in an exclusive AllQuestions class.
        // AllQuestions questions = class containing all questions.
        // string searchText = search text in list.
        public AllQuestions GetSearchQuestions (AllQuestions questions, string searchText) { 
            if (questions == null || searchText == null) {
                return null;
            }
            if (questions.questionName.Count == 0) {
                return null;
            }            
			AllQuestions allQuestions = new AllQuestions ();
            string searchTextLower = searchText.ToLower ();
            bool status = false;
            string nameLower;
            for (int aloop = 0;aloop < questions.questionName.Count; aloop++) {
                status = false;
                nameLower = questions.imageName [aloop].ToLower ();
                if (nameLower.IndexOf (searchTextLower) != -1) {
                    status = true;
                }
                nameLower = questions.questionName [aloop].ToLower ();
                if (nameLower.IndexOf (searchTextLower) != -1) {
                    status = true;
                }
                nameLower = questions.tags [aloop].ToLower ();
                if (nameLower.IndexOf (searchTextLower) != -1) {
                    status = true;
                }
                if (questions.allAnswers [aloop] != null) {
                    List<string> answers = questions.allAnswers [aloop].answers;
                    foreach (string answer in answers) {
                        nameLower = answer.ToLower ();
                        if (nameLower.IndexOf (searchTextLower) != -1) {
                            status = true;
                        }                            
                    }
                }                
                if (status) {
					allQuestions.texture.Add (new Texture2D(2, 2));
					allQuestions.isTexture.Add (false);
					allQuestions.imageName.Add (questions.imageName [aloop]);
					allQuestions.questionName.Add (questions.questionName [aloop]);
					allQuestions.tags.Add (questions.tags [aloop]);
					allQuestions.typeAnswer.Add (questions.typeAnswer [aloop]);
					string typeAnswer = GetTypeAnswerToString (questions.typeAnswer [aloop]);
					allQuestions.typeAnswerStr.Add (typeAnswer);	
					allQuestions.key.Add (questions.key [aloop]);
                    allQuestions.trueOrFalse.Add (questions.trueOrFalse [aloop]);
                    allQuestions.allAnswers.Add (questions.allAnswers [aloop]);
                }                                                               
            }
            return allQuestions;        
        }        

        // Show dialog window.
        public void SelectImageDialog () {
            if (EditorUtility.DisplayDialog("Error", "Select an image from the gallery.", "Done")) {}
            GUIUtility.ExitGUI ();
        }

        // Show dialog window.
        public void SelectSomeQuestionDialog () {
            if (EditorUtility.DisplayDialog("Error", "Select some question.", "Done")) {}
            GUIUtility.ExitGUI ();
        }

        // Show dialog window.
        public void FileNotExistDialog (string nameFile) {
			string imagesError = "The "+nameFile+" file does not exist.\nUpdate the list of images, click on reload button.";
			if (EditorUtility.DisplayDialog("Error", imagesError, "Done")) {}
            GUIUtility.ExitGUI ();
        }

        // Show image information.
        public void InfoFileDialog (string folderPath, string nameFile) {
            if (folderPath == null || nameFile == null) {
                return;
            }
            Texture2D  infoTexture = GetTexture2DFromStreamingFolder (folderPath, nameFile); 
            if (infoTexture == null) {
                return;
            }
            float fileSize = (float)GetFileSizeFromStreamingFolder (folderPath, nameFile);
            string fileSizeStr = ""; 
            if (fileSize <= 999) {
                fileSizeStr = fileSize.ToString () + " bytes";
            } else if (fileSize >= 1000 && fileSize <= 999999) {
                float fileSizeTmp = fileSize / 1000;
                fileSizeStr = fileSizeTmp.ToString ("F1") + " kb";
            } else if (fileSize >= 1000000) {
                float fileSizeTmp = fileSize / 1000000;
                fileSizeStr = fileSizeTmp.ToString ("F1") + " mb";
            }
			string infoImage = "File Name : "+nameFile+"\nWidth          : "
                +infoTexture.width.ToString ()+" pixels"+"\nHeight        : "+
                infoTexture.height.ToString ()+" pixels"+"\nFile Size     : " + fileSizeStr;
			if (EditorUtility.DisplayDialog("File information", infoImage, "Done")) {}
            GUIUtility.ExitGUI ();
        }    

        // It gets a random key.
        public string GetRandomKey () {
            string key = ""; 
            int rand = UnityEngine.Random.Range (0, 10000);
            int randB = UnityEngine.Random.Range (10001, 100000);
            float timenow = Time.time;
            key = rand.ToString () + timenow.ToString () + randB.ToString ();
            return key;
        }

        public bool DeleteQuestions (string filePathJson, AllQuestions allQuestions, List<bool> toggleStatus) {
            bool status = false;
            if (allQuestions == null || toggleStatus == null) {
                return status;
            }
            if (allQuestions.questionName == null) {
                return status;
            }            
            if (allQuestions.questionName.Count == 0 || allQuestions.questionName.Count != toggleStatus.Count) {
                return status;
            }
            SerializeQuestions serializeQuestions = ScriptableObject.CreateInstance <SerializeQuestions> ();
            for (int aloop = 0; aloop < allQuestions.questionName.Count; aloop++) {
                if (!toggleStatus [aloop]) {    
					QuestionProperties questionProperties = new QuestionProperties ();
                    questionProperties.imageName = allQuestions.imageName [aloop];
                    questionProperties.questionName = allQuestions.questionName [aloop];
                    questionProperties.tags = allQuestions.tags [aloop];
                    questionProperties.typeAnswer = allQuestions.typeAnswer [aloop];
                    questionProperties.trueOrFalse = allQuestions.trueOrFalse [aloop];
                    questionProperties.key = allQuestions.key [aloop];
                    questionProperties.answers = allQuestions.allAnswers [aloop].answers;
                    questionProperties.textOrImage = allQuestions.allAnswers [aloop].textOrImage;
                    questionProperties.selectedAnswers = allQuestions.allAnswers [aloop].selectedAnswers;                    				
					serializeQuestions.AddQuestion (questionProperties);                    
                }
            }
            status = jsonManager.WriteQuestionsJson (filePathJson, serializeQuestions);
            return status;
        }

        // Search and delete questions.
        public bool DeleteSearchQuestions (string filePathJson, AllQuestions allQuestions, 
            List<bool> toggleStatus, AllQuestions searchQuestions) {
            bool status = false;
            if (allQuestions == null || toggleStatus == null || searchQuestions == null) {
                return status;
            }
            if (allQuestions.questionName == null || searchQuestions.questionName == null) {
                return status;
            }            
            if (allQuestions.questionName.Count == 0 || allQuestions.questionName.Count != toggleStatus.Count) {
                return status;
            }
            List<string> keys = new List<string>(); 
            for (int loop = 0; loop < searchQuestions.questionName.Count; loop++) {
                if (toggleStatus [loop]) {
                    keys.Add (searchQuestions.key [loop]);
                }
            }
            SerializeQuestions serializeQuestions = ScriptableObject.CreateInstance <SerializeQuestions> ();
            for (int aloop = 0; aloop < allQuestions.questionName.Count; aloop++) {
                bool statusKey = true;
                for (int loop = 0; loop < keys.Count; loop++) {                
                    if (string.Equals(keys [loop], allQuestions.key [aloop])) {    
                        statusKey = false;
                        break;                    
                    }                   
                }
                if (statusKey) {    
                    QuestionProperties questionProperties = new QuestionProperties ();
                    questionProperties.imageName = allQuestions.imageName [aloop];
                    questionProperties.questionName = allQuestions.questionName [aloop];
                    questionProperties.tags = allQuestions.tags [aloop];
                    questionProperties.typeAnswer = allQuestions.typeAnswer [aloop];
                    questionProperties.trueOrFalse = allQuestions.trueOrFalse [aloop];
                    questionProperties.key = allQuestions.key [aloop];
                    questionProperties.answers = allQuestions.allAnswers [aloop].answers;
                    questionProperties.textOrImage = allQuestions.allAnswers [aloop].textOrImage;
                    questionProperties.selectedAnswers = allQuestions.allAnswers [aloop].selectedAnswers;  
                    serializeQuestions.AddQuestion (questionProperties);                   
                }                 
            }
            status = jsonManager.WriteQuestionsJson (filePathJson, serializeQuestions);
            return status;
        }

        // Move the selected questions up.
        public List<bool> MoveQuestionUp (string filePathJson, AllQuestions allQuestions, List<bool> toggleStatus) {
            int index = -1; // main
            int indexItem = -1;
            int newIndexItem = -1;
            string oneStr, twoStr;
            int oneInt, twoInt;
            bool oneBool, twoBool;
            AllAnswers oneAllAnswers, twoAllAnswers;
            Texture2D oneTexture, twoTexture;           
            if (allQuestions == null || toggleStatus == null) {
                return null;
            }
            if (allQuestions.questionName == null) {
                return null;
            }            
            if (allQuestions.questionName.Count == 0 || allQuestions.questionName.Count == 1 || toggleStatus.Count == 0) {
                return null;
            }
            List<bool> newToggleStatus = new List<bool>();
            for (int loop = 0; loop < toggleStatus.Count; loop++) {
                newToggleStatus.Add (false);
            }
            for (int loop = 0; loop < toggleStatus.Count; loop++) {
                if (toggleStatus [loop]) {
                    index = loop;            
                    indexItem = index;
                    if (index >= allQuestions.questionName.Count) {
                        indexItem = allQuestions.questionName.Count -1 ;
                    }
                    if (indexItem < 0) {
                        return null;
                    }
                    newIndexItem = indexItem - 1;
                    if (newIndexItem < 0 || newIndexItem >= allQuestions.questionName.Count) {
                        return null;
                    }
                    oneStr = allQuestions.imageName [indexItem];
                    twoStr = allQuestions.imageName [newIndexItem];
                    allQuestions.imageName [indexItem] = twoStr;
                    allQuestions.imageName [newIndexItem] = oneStr;

                    oneStr = allQuestions.questionName [indexItem];
                    twoStr = allQuestions.questionName [newIndexItem];
                    allQuestions.questionName [indexItem] = twoStr;
                    allQuestions.questionName [newIndexItem] = oneStr;

                    oneTexture = allQuestions.texture [indexItem];
                    twoTexture = allQuestions.texture [newIndexItem];
                    allQuestions.texture [indexItem] = twoTexture;
                    allQuestions.texture [newIndexItem] = oneTexture;            

                    oneBool = allQuestions.isTexture [indexItem];
                    twoBool = allQuestions.isTexture [newIndexItem];
                    allQuestions.isTexture [indexItem] = twoBool;
                    allQuestions.isTexture [newIndexItem] = oneBool;

                    oneStr = allQuestions.key [indexItem];
                    twoStr = allQuestions.key [newIndexItem];
                    allQuestions.key [indexItem] = twoStr;
                    allQuestions.key [newIndexItem] = oneStr;

                    oneStr = allQuestions.tags [indexItem];
                    twoStr = allQuestions.tags [newIndexItem];
                    allQuestions.tags [indexItem] = twoStr;
                    allQuestions.tags [newIndexItem] = oneStr;                                                                                               

                    oneInt = allQuestions.typeAnswer [indexItem];
                    twoInt = allQuestions.typeAnswer [newIndexItem];
                    allQuestions.typeAnswer [indexItem] = twoInt;
                    allQuestions.typeAnswer [newIndexItem] = oneInt;

                    oneStr = allQuestions.typeAnswerStr [indexItem];
                    twoStr = allQuestions.typeAnswerStr [newIndexItem];
                    allQuestions.typeAnswerStr [indexItem] = twoStr;
                    allQuestions.typeAnswerStr [newIndexItem] = oneStr;

                    oneBool = allQuestions.trueOrFalse [indexItem];
                    twoBool = allQuestions.trueOrFalse [newIndexItem];
                    allQuestions.trueOrFalse [indexItem] = twoBool;
                    allQuestions.trueOrFalse [newIndexItem] = oneBool;

                    oneAllAnswers = allQuestions.allAnswers [indexItem];
                    twoAllAnswers = allQuestions.allAnswers [newIndexItem];
                    allQuestions.allAnswers [indexItem] = twoAllAnswers;
                    allQuestions.allAnswers [newIndexItem] = oneAllAnswers;                    

                    newToggleStatus [newIndexItem] = true; 
                }
            }
			SerializeQuestions serializeQuestions = ScriptableObject.CreateInstance <SerializeQuestions> ();
			for (int aloop = 0; aloop < allQuestions.questionName.Count; aloop++) {
				QuestionProperties questionProperties = new QuestionProperties ();
                questionProperties.imageName = allQuestions.imageName [aloop];
                questionProperties.questionName = allQuestions.questionName [aloop];
                questionProperties.tags = allQuestions.tags [aloop];
                questionProperties.typeAnswer = allQuestions.typeAnswer [aloop];
                questionProperties.trueOrFalse = allQuestions.trueOrFalse [aloop];
                questionProperties.key = allQuestions.key [aloop];
                questionProperties.answers = allQuestions.allAnswers [aloop].answers;
                questionProperties.textOrImage = allQuestions.allAnswers [aloop].textOrImage;
                questionProperties.selectedAnswers = allQuestions.allAnswers [aloop].selectedAnswers; 
			    serializeQuestions.AddQuestion (questionProperties);
			}
			jsonManager.WriteQuestionsJson (filePathJson, serializeQuestions);
            return newToggleStatus;
        }

        // Move the selected questions down.
        public List<bool> MoveQuestionDown (string filePathJson, AllQuestions allQuestions, List<bool> toggleStatus) {
            int index = -1; // main
            int indexItem = -1;
            int newIndexItem = -1;
            string oneStr, twoStr;
            int oneInt, twoInt;
            bool oneBool, twoBool;
            AllAnswers oneAllAnswers, twoAllAnswers;            
            Texture2D oneTexture, twoTexture;           
            if (allQuestions == null || toggleStatus == null) {
                return null;
            }
            if (allQuestions.questionName == null) {
                return null;
            }            
            if (allQuestions.questionName.Count == 0 || allQuestions.questionName.Count == 1 || toggleStatus.Count == 0) {
                return null;
            }
            List<bool> newToggleStatus = new List<bool>();
            for (int loop = 0; loop < toggleStatus.Count; loop++) {
                newToggleStatus.Add (false);
            }
            for (int loop = (toggleStatus.Count - 1); loop >= 0; loop--) {
                if (toggleStatus [loop]) {
                    index = loop;            
                    indexItem = index;
                    if (index >= allQuestions.questionName.Count) {
                        indexItem = allQuestions.questionName.Count -1 ;
                    }
                    if (indexItem < 0) {
                        return null;
                    }
                    newIndexItem = indexItem + 1;
                    if (newIndexItem < 0 || newIndexItem >= allQuestions.questionName.Count) {
                        return null;
                    }
                    oneStr = allQuestions.imageName [indexItem];
                    twoStr = allQuestions.imageName [newIndexItem];
                    allQuestions.imageName [indexItem] = twoStr;
                    allQuestions.imageName [newIndexItem] = oneStr;

                    oneStr = allQuestions.questionName [indexItem];
                    twoStr = allQuestions.questionName [newIndexItem];
                    allQuestions.questionName [indexItem] = twoStr;
                    allQuestions.questionName [newIndexItem] = oneStr;

                    oneTexture = allQuestions.texture [indexItem];
                    twoTexture = allQuestions.texture [newIndexItem];
                    allQuestions.texture [indexItem] = twoTexture;
                    allQuestions.texture [newIndexItem] = oneTexture;            

                    oneBool = allQuestions.isTexture [indexItem];
                    twoBool = allQuestions.isTexture [newIndexItem];
                    allQuestions.isTexture [indexItem] = twoBool;
                    allQuestions.isTexture [newIndexItem] = oneBool;

                    oneStr = allQuestions.key [indexItem];
                    twoStr = allQuestions.key [newIndexItem];
                    allQuestions.key [indexItem] = twoStr;
                    allQuestions.key [newIndexItem] = oneStr;                                                                          

                    oneStr = allQuestions.tags [indexItem];
                    twoStr = allQuestions.tags [newIndexItem];
                    allQuestions.tags [indexItem] = twoStr;
                    allQuestions.tags [newIndexItem] = oneStr;                                                                                               

                    oneInt = allQuestions.typeAnswer [indexItem];
                    twoInt = allQuestions.typeAnswer [newIndexItem];
                    allQuestions.typeAnswer [indexItem] = twoInt;
                    allQuestions.typeAnswer [newIndexItem] = oneInt;

                    oneStr = allQuestions.typeAnswerStr [indexItem];
                    twoStr = allQuestions.typeAnswerStr [newIndexItem];
                    allQuestions.typeAnswerStr [indexItem] = twoStr;
                    allQuestions.typeAnswerStr [newIndexItem] = oneStr;
                                       
                    oneBool = allQuestions.trueOrFalse [indexItem];
                    twoBool = allQuestions.trueOrFalse [newIndexItem];
                    allQuestions.trueOrFalse [indexItem] = twoBool;
                    allQuestions.trueOrFalse [newIndexItem] = oneBool;                    

                    oneAllAnswers = allQuestions.allAnswers [indexItem];
                    twoAllAnswers = allQuestions.allAnswers [newIndexItem];
                    allQuestions.allAnswers [indexItem] = twoAllAnswers;
                    allQuestions.allAnswers [newIndexItem] = oneAllAnswers;  
                    
                    newToggleStatus [newIndexItem] = true; 
                }
            }
			SerializeQuestions serializeQuestions = ScriptableObject.CreateInstance <SerializeQuestions> ();
			for (int aloop = 0; aloop < allQuestions.questionName.Count; aloop++) {
				QuestionProperties questionProperties = new QuestionProperties (); 
                questionProperties.imageName = allQuestions.imageName [aloop];
                questionProperties.questionName = allQuestions.questionName [aloop];
                questionProperties.tags = allQuestions.tags [aloop];
                questionProperties.typeAnswer = allQuestions.typeAnswer [aloop];
                questionProperties.trueOrFalse = allQuestions.trueOrFalse [aloop];
                questionProperties.key = allQuestions.key [aloop];
                questionProperties.answers = allQuestions.allAnswers [aloop].answers;
                questionProperties.textOrImage = allQuestions.allAnswers [aloop].textOrImage;
                questionProperties.selectedAnswers = allQuestions.allAnswers [aloop].selectedAnswers; 
				serializeQuestions.AddQuestion (questionProperties);
			}
			jsonManager.WriteQuestionsJson (filePathJson, serializeQuestions);
            return newToggleStatus;
        }

        // Show dialog window.
        public void UnexpectedErrorOccurred () {
            if (EditorUtility.DisplayDialog("Error", "An unexpected error has occurred.", "Done")) {}
            GUIUtility.ExitGUI ();
        }

        // Returns a custom texture.
        public Texture2D GetColorTexture2D (Color color, int width, int height) {
            Texture2D newTexture2D;
            Color32 [] newColor = new Color32 [width * height];
            for (int aloop = 0; aloop < newColor.Length; aloop++) {
                newColor [aloop] = color;
            }
			newTexture2D = new Texture2D(width, height);
            newTexture2D.SetPixels32 (newColor);
            newTexture2D.Apply ();
            return newTexture2D;
        }  

        // Check if an image is used for one or more questions before it is deleted.
        public bool CheckIfImageInUseBeforeDeleting (string filePathJson, string nameFile) {
            bool status = false;
            if (nameFile == null) {
                return status;
            }
            List <int> questionsIndex = CheckIfImageUsedForQuestion (filePathJson, nameFile);
            if (questionsIndex == null) {
                return status;
            }
            if (questionsIndex.Count == 0) {
                return status;
            } else {
                status = true;
            }
            if (status) {
                if (EditorUtility.DisplayDialog("Error", "The image is currently being used.\nThe image cannot be deleted.", "Done")) {}
                GUIUtility.ExitGUI ();
            }
            return status;
        }

        // Returns a list of questions that use the same image.
        public List<int> CheckIfImageUsedForQuestion (string filePathJson, string nameFile) {
            List<int> questionIndex = null;
            if (filePathJson == null || nameFile == null) {
                return questionIndex;
            }
            List <QuestionProperties> questions = jsonManager.ReadQuestionsJson (filePathJson);
            if (questions == null) {
                return questionIndex;
            }
            questionIndex = new List<int>();
            int aloop = 1;
            foreach (QuestionProperties question in questions) {
                bool isImage = false;
                if ( string.Equals(question.imageName, nameFile) ) {
                    isImage = true;
                }
                foreach (string answer in question.answers) {
                    if ( string.Equals(answer, nameFile) ) {
                        isImage = true;
                    }
                }
                if (isImage) {
                    questionIndex.Add (aloop);
                }
                aloop++;
            }
            return questionIndex;
        }

        public string GetTypeAnswerToString (int type) {
            string answer = "";
            switch (type) {
                case GlobalEditorVariables.idSingleSelectionAnswer:
                    answer = "Single Selection";
                break;
                case GlobalEditorVariables.idTrueOrFalseAnswer:
                    answer = "True or False";
                break;
                default:
                break;                                
            }
            return answer;
        }
    }
}