// Create the editor window, image gallery and image viewer.
using UnityEngine;
using UnityEditor;

namespace JsonQuizEditor.Scripts
{
	public class JsonQuizWindow : EditorWindow
	{
		public int idWindow = -1; // Editor window id.
		QuestionAnswerEditor questionAnswerEditor = null; // Reference to the editor where it shows the list of questions.
		bool enableImageGallery = false;
		bool enableImageView = false;
		float destructionCounter = 0.8f;
		bool enableDestroy = false;
		ImageGalleryWindow imageGalleryWindow = null; // Image gallery window.
		public ImageViewWindow imageViewWindow = null; // Image viewer window.
			
		// Add menu "Json Quiz Editor"
		[MenuItem("Window/Json Quiz Editor")]
		static void Init () {
			// Create editor window.
			JsonQuizWindow window = (JsonQuizWindow)EditorWindow.GetWindow (
				typeof(JsonQuizWindow), false, GlobalEditorVariables.titleJsonQuizEditorWindow);
			window.minSize = new Vector2 (GlobalEditorVariables.widthEditor, GlobalEditorVariables.heightEditor);
			window.autoRepaintOnSceneChange = true;
			window.idWindow = GlobalEditorVariables.idJsonQuizWindow; 
			window.Show();
		}

		void OnEnable () {
			DataPlatform.InitEnvironment ();			
			if (questionAnswerEditor == null) {
				CreateQuestionAnswerEditor ();
			}			
		}

		void Update () {
			if (enableDestroy) {
				destructionCounter -= Time.deltaTime;
				if (destructionCounter < 0) {
					enableDestroy = false;
					Close ();
				}
			} 
		}

		// Close the windows.
		void OnDestroy () {	
            JsonQuizWindow [] winEditors = Resources.FindObjectsOfTypeAll<JsonQuizWindow>();
            if (winEditors != null && idWindow == GlobalEditorVariables.idJsonQuizWindow) {
                foreach (JsonQuizWindow winEditor in winEditors) {
					if (winEditor != null) {
						if (winEditor.idWindow == GlobalEditorVariables.idImageGalleryWindow) { 
							winEditor.DestroyObject ();                     
						}
						if (winEditor.idWindow == GlobalEditorVariables.idImageViewWindow) { 
							winEditor.DestroyObject ();                     
						}
					}                                         
                }
            }			
		}

		// Reload question and answer information when switching to play mode.
		void ReloadDataPlayMode () {
			CreateQuestionAnswerEditor ();
		}

		// Reload non-serialized data after exiting play mode.
		void ReloadDataEditMode () {
			CreateQuestionAnswerEditor ();				
		}

		// Delete editor windows created when exiting edit mode.
		void ExitingEditMode () {
			DestroySubWindows ();
		}

		// Delete editor windows created when exiting play mode.
		void ExitingPlayMode () {
			DestroySubWindows ();		
		}

		// Destroy the windows.
		void DestroySubWindows () {
            JsonQuizWindow [] winEditors = Resources.FindObjectsOfTypeAll<JsonQuizWindow>();
            if (winEditors != null) {
                foreach (JsonQuizWindow winEditor in winEditors) {
					if (winEditor != null) {
						if (winEditor.idWindow == GlobalEditorVariables.idImageGalleryWindow) { 
							winEditor.DestroyObject ();					              
						}
						if (winEditor.idWindow == GlobalEditorVariables.idImageViewWindow) { 
							winEditor.DestroyObject ();                     
						}
					}                                         
                }
            }
		}

		// Create the editor window where the list of questions will be displayed.
		void CreateQuestionAnswerEditor () {
			questionAnswerEditor = new QuestionAnswerEditor ();
			questionAnswerEditor.LoadImagesFromEditorFolder ();			
		}

		// Draw UI.
		void OnGUI () {
			Event eventEditor = Event.current;
			if (eventEditor != null) {
				if (eventEditor.commandName == GlobalEditorVariables.enteredEditModeEvent) {
					ReloadDataEditMode ();
					eventEditor.commandName = null;
				}
				if (eventEditor.commandName == GlobalEditorVariables.enteredPlayModeEvent) {
					ReloadDataPlayMode ();
					eventEditor.commandName = null;
				}
				if (eventEditor.commandName == GlobalEditorVariables.exitingEditModeEvent) {
					ExitingEditMode ();
					eventEditor.commandName = null;
				}
				if (eventEditor.commandName == GlobalEditorVariables.exitingPlayModeEvent) {
					ExitingPlayMode ();
					eventEditor.commandName = null;
				}												
			}							
			if (enableImageGallery && imageGalleryWindow != null) {
				ShowCategoryImagesWindow ();
				return;
			}
			if (enableImageView && imageViewWindow != null) {
				ShowImageViewWindow ();
				return;
			}
			if (questionAnswerEditor != null) {
				questionAnswerEditor.QuestionsGUI ();
			}
			ChangeSizeWindow ();										
		}

		// Resize the editor window.
		void ChangeSizeWindow () {					
			if (questionAnswerEditor != null) {
				questionAnswerEditor.ChangeSizeWindow (position);
			}									
		}
 
		// Create the image gallery window.
		public void CreateImageGalleryWindow (QuestionAnswerManager questionAnswerReference) {
			JsonQuizWindow window =  ScriptableObject.CreateInstance <JsonQuizWindow> ();
			window.minSize = new Vector2 (GlobalEditorVariables.widthImageGallery, GlobalEditorVariables.heightImageGallery);
			window.ShowUtility ();
			window.idWindow = GlobalEditorVariables.idImageGalleryWindow;
			window.titleContent = new GUIContent (GlobalEditorVariables.titleImageGalleryWindow);
			window.enableImageGallery = true;
			window.imageGalleryWindow = new ImageGalleryWindow ();
			window.imageGalleryWindow.SetQuestionAnswerManagerObjectReference (questionAnswerReference);
			window.imageGalleryWindow.LoadImagesFromEditorFolder ();
			window.imageGalleryWindow.LoadImagesFromStreamingFolder ();
			window.Show();		
		}	

		// Show image gallery window.
		void ShowCategoryImagesWindow () {
			GUI.BringWindowToFront(1);
			if (imageGalleryWindow != null) {
				imageGalleryWindow.ImageGalleryGUI ();
				imageGalleryWindow.ChangeSizeWindow (position);
			}
		}

		// Create the image viewer window.
		public void CreateImageViewWindow (string folderPath, string nameFile) {
            if (folderPath == null || nameFile == null) {
                return;
            }
			JsonQuizWindow window =  ScriptableObject.CreateInstance <JsonQuizWindow> ();
			window.minSize = new Vector2 (GlobalEditorVariables.widthImageView, GlobalEditorVariables.heightImageView);
			window.maxSize = new Vector2 (GlobalEditorVariables.widthImageView * 3, GlobalEditorVariables.heightImageView * 3);
			window.ShowUtility ();
			window.idWindow = GlobalEditorVariables.idImageViewWindow;
			window.titleContent = new GUIContent (GlobalEditorVariables.titleImageViewWindow);
			window.enableImageView = true;
			window.imageViewWindow = new ImageViewWindow ();
			window.imageViewWindow.LoadImagesFromStreamingFolder (folderPath, nameFile);
			window.Show();		
		}

		// Show image viewer window.
		void ShowImageViewWindow () {
			GUI.BringWindowToFront(1);
			if (imageViewWindow != null) {
				imageViewWindow.ImageViewGUI ();
				imageViewWindow.ChangeSizeWindow (position);
			}
		}

		void DestroyObject () {
			enableDestroy = true;	
		}						
	}
}