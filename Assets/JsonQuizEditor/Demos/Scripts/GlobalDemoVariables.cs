// Class used by the demos (Simple demo and Quiz demo).
public class GlobalDemoVariables
{
	// Main menu ID.
	public const int mainMenuID = 0; 
	// Category menu ID.
	public const int categoriesMenuID = 1; 
	// Level menu ID.
	public const int levelMenuID = 2; 
	// Game over menu ID.
	public const int gameOverMenuID = 3; 
	// File name json.
	public const string jsonQuestionFileName = "Questions.json";
	// Folder containing json file. Works only on Editor, Windows and Linux.
	// Add trailing slash (/) to start and end.
	public const string jsonQuestionFolderPC = "/StreamingAssets/Demo/Json/";
	// Folder containing json file. Works only on Android.
	// Add trailing slash (/) to start and end.	
	public const string jsonQuestionFolderAndroid = "/Demo/Json/";
	// Folder containing images. Works only on Editor, Windows and Linux.
	// Add trailing slash (/) to start and end.
	public const string imagesFolderPC = "/StreamingAssets/Demo/Images/";
	// Folder containing images. Works only on Android.
	// Add trailing slash (/) to start and end.	
	public const string imagesFolderAndroid = "/Demo/Images/";
	// Simple selection ID.
	public const int singleTypeAnswerID = 0;
	// True or False ID.
	public const int trueOrFalseTypeAnswerID = 1;
	// Text ID. 
	public const int idTextSelection = 0;
	// Imagen ID.
	public const int idImageSelection = 1;
	// Score earned.
	public const int score = 10;		
}