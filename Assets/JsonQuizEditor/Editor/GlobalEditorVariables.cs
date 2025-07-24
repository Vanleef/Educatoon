// Global variables.

using UnityEngine;
using System.ComponentModel;

namespace JsonQuizEditor.Scripts 
{
    public class GlobalEditorVariables
    {
		public const float widthEditor = 450.0f;
		public const float heightEditor = 380.0f;
		public const float widthImageGallery = 310.0f;
		public const float heightImageGallery = 500.0f;	
		public const float widthImageView = 150.0f;
		public const float heightImageView = 150.0f;					
		public const string idAssetsFolder = "Assets";
		// Destination folder of the editor's images and configuration files.
		public const string idEditorDefaultResourcesFolder = "/JsonQuizEditor/EditorResources/"; 
		public const string idEditorImagesFolder = "JsonQuizEditor/Images/";
		public const string idStreamingFolder = "/StreamingAssets/";
		// Destination folder of the images used for the questions and answers.
		public const string idImagesFolder = "Images"; 
		// Default location of the json file that contains the questions.
		public const string fileQuestionsJson = "Json/Questions.json";
		// Json configuration file for questions. 
		public const string fileQuestionsEditorSettingsJson = "JsonQuizEditor/Config/Questions.json";
		// Editor configuration json file.
		public const string fileEditorSettingsJson = "JsonQuizEditor/Config/Editor.json";			
		public const string nameDefaultImage = "DefaultImage.png";
		public const string searchNameImage = "Search.png";
		public const string closeNameImage = "Close.png";
		public const string leftNameImage = "Left.png";
		public const string rightNameImage = "Right.png";
		public const string separationNameImage = "Separation.png";
		public const string addImageNameImage = "AddImage.png";
		public const string closeWindowNameImage = "CloseWindow.png";
		public const string infoNameImage = "Info.png";
		public const string inUseNameImage = "InUseImage.png";
		public const string trashNameImage = "Trash.png";
		public const string viewNameImage = "View.png";
		public const string settingNameImage = "Setting.png";
		// Unity editor event names when changing play mode and edit mode.
		public const string enteredEditModeEvent = "EnteredEditMode";
		public const string exitingEditModeEvent = "ExitingEditMode";
		public const string enteredPlayModeEvent = "EnteredPlayMode";
		public const string exitingPlayModeEvent = "ExitingPlayMode";
		public const string titleImageGalleryWindow = "Image gallery";
		public const string titleJsonQuizEditorWindow = "Json Quiz Editor";
		public const string titleImageViewWindow = "Image view";
		public const int idJsonQuizWindow = 1; // Man window ID.
		public const int idImageGalleryWindow = 2; // Image gallery ID.
		public const int idImageViewWindow = 3; // image viewer ID.
		public const int maximumNumberAnswers = 100;
		public const int minimumNumberAnswers = 2;
		public const int idSingleSelectionAnswer = 0; // Type of answer: Simple selection.
		public const int idTrueOrFalseAnswer = 1; // Type of answer: True or False.
		public const int idTextSelection = 0; // Text.
		public const int idImageSelection = 1; // Image.
		// Options for EditorGUI.EnumPopup.	
		public enum rowsOptions {
			[Description("5")]
			_5 = 0,
			[Description("10")]
			_10 = 1,
			[Description("20")]
			_20 = 2,
			[Description("30")]
			_30 = 3
		}
		// Structure that will be used to display the list of added questions.		
		public struct QuestionTextureData {
			public int id; 
			public Texture2D texture;
			public string imageName;			
		}
		// Structure that contains the position of the cell separators where the questions are displayed.
		public struct QuestionEditorConfig {
			public int enumPopup;
			public int xImagesSeparation;
			public int xQuestionsSeparation;
			public int xOptionsSeparation;
			public int xTagsSeparation;
			public int enumPopupImages;		
		}
			
    }

	public static class DataPlatform {
		public static string dataPath = null; // Project path on the platform on which the json editor runs (Window y Linux).
			
		// Initialize variables.
		public static void InitEnvironment () {
			dataPath = Application.dataPath;
		}
	}
}
