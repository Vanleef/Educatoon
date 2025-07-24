// Image gallery.
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace JsonQuizEditor.Scripts
{
	[System.Serializable]
    public class ImageGalleryWindow : ISerializationCallbackReceiver
    {
		Vector2 imagesScrollPosition; 
		int idImageSelectButton = -1; 
		Texture2D defaultTexture2D = null;
		Texture2D questionTexture = null;
		EditorTools editorTools = null;
		float heighRightArea = 0.0f;
		float heighRightAreaConst = 466;
		float heightRightScrollView = 0.0f;
		float heightRightScrollViewConst = 185;
		float widthRightArea = 0.0f;
		float widthRightScrollView = 0.0f;
		float widthRightTextField = 0.0f;
		float widthRightTextFieldConst = 170.0f;
		[System.NonSerialized]
		AllImagesInGallery allImagesInGallery = null; // This class will contain the textures of all images.
		[System.NonSerialized]
		AllImagesInGallery searchImagesInGallery = null; // This class will contain the textures of all images resulting from a search.
		Texture2D previewTexture = null; // Show the selected image.
		string searchText = null;
		string searchTextCompare = null;
		Texture2D searchTexture2D = null;
		Texture2D closeTexture2D = null;
		Texture2D leftTexture2D = null;
		Texture2D rightTexture2D = null;
		Texture2D infoTexture2D = null;
		Texture2D inUseImageTexture2D = null;
		float widthRightSearch = 0.0f;
		GlobalEditorVariables.rowsOptions enumRowsOptions = GlobalEditorVariables.rowsOptions._20;
		int imagesPagination = 0;
		int imagesLenghtPagination = 0;
		int imagesCurrentPagination = 0;
		int imagesSearchLenghtPagination = 0;
		int currentImagesRows = 0;
		int lastCurrentImagesRows = 0;
		int lastImagesRows = 0;
		int startRows = 0;
		int endRows = 0;
		bool enableSearchImages = false;
		float widthMainEditor = 0.0f;
		string infoNameFile = null;
		bool isInUseImage = false;
		[System.NonSerialized]
		List<int> indexImagesInGallery = null; // Image position in gallery.
		string indexImagesGalleryStr = ""; 
		JsonManager jsonManager = null; // Read and save in json file.
        string filePathQuestionsJson = null; // Default location of the json file that contains the questions.
        string questionImagesFolder = null; // Folder containing the images that will be used in the questions.
		QuestionAnswerManager questionAnswerManager = null; // Reference to the class in which the questions are edited.
		QuestionEditorSettings questionEditorSettings = null; // Editor settings.
		bool doNotShowGUI = false;


		// Initialize variables.
        public ImageGalleryWindow () {
			widthMainEditor = GlobalEditorVariables.widthImageGallery;
			heighRightArea = heighRightAreaConst;
			heightRightScrollView = heightRightScrollViewConst;
			widthRightArea = 306;
			widthRightScrollView = 300;
			widthRightTextField = widthRightTextFieldConst;
			widthRightSearch = 200;
			startRows = 0;
			endRows = 0;			
			imagesPagination = 0;
			imagesLenghtPagination = 0;
			imagesCurrentPagination = 0;
			imagesSearchLenghtPagination = 0;
			currentImagesRows = 0;
			lastImagesRows = 0;
			if (jsonManager == null) {
				jsonManager = new JsonManager ();
				// Get editor properties.
				DeserializeEditorConfig desEditorConfig = jsonManager.ReadEditorConfigJson ();
				if (desEditorConfig != null) {
					filePathQuestionsJson = desEditorConfig.filePathQuestionsJson;
					questionImagesFolder = desEditorConfig.questionImagesFolder;				
				}
			}			
            if (editorTools == null) {		            
                editorTools = new EditorTools ();
            }
			if (searchText == null) {
				searchText = "";
			}
			if (searchTextCompare == null) {
				searchTextCompare = "";
			}
			if (infoNameFile == null) {
				infoNameFile = "";
			}
			if (allImagesInGallery == null) {
				allImagesInGallery = new AllImagesInGallery ();
			}			
			if (questionEditorSettings == null) {
				questionEditorSettings = new QuestionEditorSettings ();
				questionEditorSettings.InitEnvironment ();
			}
			enumRowsOptions = questionEditorSettings.ReadEnumPopupImagesSettings ();							
        }

		public void OnBeforeSerialize () {
		}
		
		public void OnAfterDeserialize () {
			doNotShowGUI = true;
		}

		// Draw UI.
        public void ImageGalleryGUI () {
			if (doNotShowGUI) {
				return;
			}
			bool enableRightRowOptions = false;
			if (imagesLenghtPagination == 0) {
				enableRightRowOptions = true;
			}
            GUIStyle searchFieldStyle = new GUIStyle (EditorStyles.textField);
            searchFieldStyle.alignment = TextAnchor.MiddleLeft;			
            string itemsStr;         
				var listImagesStyle = new GUIStyle(GUI.skin.textField);
				GUILayout.BeginArea(new Rect(0, 0, widthRightArea, heighRightArea + 30), listImagesStyle);
					GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						GUILayout.Label("Select an image from gallery", EditorStyles.boldLabel);
						GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal();

					EditorGUI.BeginDisabledGroup (enableRightRowOptions);
					GUILayout.BeginHorizontal();
						GUILayout.FlexibleSpace();
						if (searchTexture2D != null) {
							GUILayout.Label(new GUIContent(searchTexture2D), GUILayout.Width(20), GUILayout.Height(20));
						}
						if (searchText == null) {
							searchText = "";
						}
						searchText = GUILayout.TextField(
							searchText, searchFieldStyle, GUILayout.Width(widthRightSearch), GUILayout.Height(20));
						if ( !string.Equals(searchText, searchTextCompare) ) { // Search images.
							searchTextCompare = searchText;
							searchImagesInGallery = editorTools.GetImageNameSearch (allImagesInGallery.image, searchText);
							imagesSearchLenghtPagination = 0;
							imagesCurrentPagination = 1;
							idImageSelectButton = -1;
							previewTexture = null;
							imagesScrollPosition = Vector2.zero; 
							infoNameFile = null;
							isInUseImage = false;							
						}
						if (searchImagesInGallery != null && searchImagesInGallery.image != null) {
							if (searchImagesInGallery.image.Count > 0) {
								enableSearchImages = true;
								imagesSearchLenghtPagination = searchImagesInGallery.image.Count;
							} else {
								enableSearchImages = true;
								imagesSearchLenghtPagination = 0;								
							}
						}
						if (closeTexture2D != null) {
							if (GUILayout.Button(new GUIContent(closeTexture2D), GUILayout.Width(16), GUILayout.Height(16))) {
								if (searchText.Length > 0) {
									searchText = "";
									searchTextCompare = "";
									searchImagesInGallery = null;
									enableSearchImages = false;
									idImageSelectButton = -1;
									previewTexture = null;
									imagesScrollPosition = Vector2.zero;
									imagesCurrentPagination = 1;
									infoNameFile = null;
									isInUseImage = false;
								}						
							}
						}
					GUILayout.EndHorizontal();
					EditorGUI.EndDisabledGroup();

					GUILayout.BeginHorizontal("box");
						EditorGUI.BeginDisabledGroup (enableRightRowOptions);
						if (GUILayout.Button("Delete")) {
							if (allImagesInGallery != null && allImagesInGallery.image != null) {
								if (idImageSelectButton < 0) {
									editorTools.SelectImageDialog ();
								} else {
									bool statusImgUsed = false;						
									if (allImagesInGallery.image.Count > 0 && idImageSelectButton >= 0 && !enableSearchImages) {
										if (editorTools.CheckIfImageInUseBeforeDeleting (
											filePathQuestionsJson, allImagesInGallery.image [idImageSelectButton])) {
											statusImgUsed = true;										
										}
										// Delete the selected image.
										if (allImagesInGallery.image [idImageSelectButton] != null && !statusImgUsed) {										
											if (EditorUtility.DisplayDialog("Delete selected asset?",
											"Are you sure you want to delete the "+allImagesInGallery.image [idImageSelectButton]+
												" file?\nYou cannot undo this action.", "Yes", "Cancel")) {
												if (editorTools.DeleteFileStreamingFolder (
													questionImagesFolder, allImagesInGallery.image [idImageSelectButton])) {
													allImagesInGallery = new AllImagesInGallery ();													
													LoadImagesFromStreamingFolder ();
													if (allImagesInGallery.image.Count > 0 && idImageSelectButton >= 
														allImagesInGallery.image.Count) {
														idImageSelectButton = allImagesInGallery.image.Count - 1;
													}
													if (idImageSelectButton >= 0 && allImagesInGallery.image.Count > 0) {
														previewTexture = editorTools.GetTexture2DFromStreamingFolder (
															questionImagesFolder, allImagesInGallery.image [idImageSelectButton]);
													}
													if (allImagesInGallery.image.Count == 0) {
														previewTexture = null; 
													}
													imagesPagination = (int)Mathf.Ceil ((float)imagesLenghtPagination / currentImagesRows);
													if (imagesCurrentPagination >= imagesPagination) {
														imagesCurrentPagination = imagesPagination;
													}
													if (previewTexture != null && idImageSelectButton >= 0) {
														CheckIfImageUsed (allImagesInGallery.image [idImageSelectButton]);
													}
												} else {
													editorTools.FileNotExistDialog (allImagesInGallery.image [idImageSelectButton]);										
												}
											}
										}
									}
									// Delete the selected image.
									if (imagesSearchLenghtPagination > 0 && idImageSelectButton >= 0 && enableSearchImages) {
										if (editorTools.CheckIfImageInUseBeforeDeleting (
											filePathQuestionsJson, searchImagesInGallery.image [idImageSelectButton])) {
											statusImgUsed = true;										
										}										
										if (searchImagesInGallery.image [idImageSelectButton] != null && !statusImgUsed) {										
											if (EditorUtility.DisplayDialog("Delete selected asset?",
											"Are you sure you want to delete the "+searchImagesInGallery.image [idImageSelectButton]+
												" file?\nYou cannot undo this action.", "Yes", "Cancel")) {
												if (editorTools.DeleteFileStreamingFolder (
													questionImagesFolder, searchImagesInGallery.image [idImageSelectButton])) {
													string searchTextB = searchText;
													string searchTextCompareB = searchTextCompare;
													allImagesInGallery = new AllImagesInGallery ();																										
													LoadImagesFromStreamingFolder ();
													searchText = searchTextB;
													searchTextCompare = searchTextCompareB;													
													searchImagesInGallery = editorTools.GetImageNameSearch (
														allImagesInGallery.image, searchText);																										
													if (searchImagesInGallery != null && searchImagesInGallery.image != null) {
														if (searchImagesInGallery.image.Count > 0) {
															enableSearchImages = true;
															imagesSearchLenghtPagination = searchImagesInGallery.image.Count;
														} else {
															enableSearchImages = true;
															imagesSearchLenghtPagination = 0;								
														}
													}
													if (searchImagesInGallery.image.Count > 0 && 
														idImageSelectButton >= searchImagesInGallery.image.Count) {
														idImageSelectButton = searchImagesInGallery.image.Count - 1;
													}
													if (idImageSelectButton >= 0 && searchImagesInGallery.image.Count > 0) {
														previewTexture = editorTools.GetTexture2DFromStreamingFolder (
															questionImagesFolder, searchImagesInGallery.image [idImageSelectButton]);
													}
													if (searchImagesInGallery.image.Count == 0) {
														previewTexture = null;
														idImageSelectButton = -1; 
													}
													imagesPagination = (int)Mathf.Ceil ((float)searchImagesInGallery.image.Count / currentImagesRows);
													if (imagesCurrentPagination >= imagesPagination) {
														imagesCurrentPagination = imagesPagination;
													}
													if (previewTexture != null && idImageSelectButton >= 0) {
														CheckIfImageUsed (allImagesInGallery.image [idImageSelectButton]);
													}													
												} else {
													editorTools.FileNotExistDialog (searchImagesInGallery.image [idImageSelectButton]);												
												}
											}
										}
									}								
								}
							}
							GUIUtility.ExitGUI ();
						}
						EditorGUI.EndDisabledGroup();
						GUILayout.FlexibleSpace();
						// Reload the images from gallery.			
						if (GUILayout.Button("Reload")) {
							allImagesInGallery = new AllImagesInGallery ();
							previewTexture = null;
							idImageSelectButton = -1;
							LoadImagesFromStreamingFolder ();
							imagesPagination = (int)Mathf.Ceil ((float)imagesLenghtPagination / currentImagesRows);
							if (imagesPagination > 0) {
								imagesCurrentPagination = 1;
							} else {
								imagesCurrentPagination = 0;
							}
							enableSearchImages = false;
							imagesScrollPosition = Vector2.zero;
							isInUseImage = false;
						}
						GUILayout.FlexibleSpace();
						EditorGUI.BeginDisabledGroup (enableRightRowOptions);
						if (GUILayout.Button("Select an image")) {
							if (allImagesInGallery != null && allImagesInGallery.image != null && !enableSearchImages) {
								if (idImageSelectButton < 0) {
									editorTools.SelectImageDialog ();
								} else if (allImagesInGallery.image.Count > 0 && idImageSelectButton >= 0) {
									questionTexture = editorTools.GetTexture2DFromStreamingFolder (
										questionImagesFolder, allImagesInGallery.image [idImageSelectButton]);
									if (questionTexture == null) {
										editorTools.FileNotExistDialog (allImagesInGallery.image [idImageSelectButton]);
									} else {
										if (questionAnswerManager != null) {
											// Add the image to the editor.
											questionAnswerManager.AddImageToQuestionFromGallery (allImagesInGallery.image [idImageSelectButton]);
										}
									}									
								}
							}
							if (searchImagesInGallery != null && searchImagesInGallery.image != null && enableSearchImages) {
								if (idImageSelectButton < 0) {
									editorTools.SelectImageDialog ();
								} else if (searchImagesInGallery.image.Count > 0 && idImageSelectButton >= 0) {
									questionTexture = editorTools.GetTexture2DFromStreamingFolder (
										questionImagesFolder, searchImagesInGallery.image [idImageSelectButton]);
									if (questionTexture == null) {
										editorTools.FileNotExistDialog (searchImagesInGallery.image [idImageSelectButton]);
									} else {
										if (questionAnswerManager != null) {
											// Add the image to the editor.
											questionAnswerManager.AddImageToQuestionFromGallery (searchImagesInGallery.image [idImageSelectButton]);
										}
									}										
								}
							}														
						}
						EditorGUI.EndDisabledGroup();
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();											
						EditorGUI.BeginDisabledGroup (enableRightRowOptions);
						if (currentImagesRows != lastImagesRows) {					
							lastImagesRows = currentImagesRows;
							if (imagesLenghtPagination == 0) {
								imagesCurrentPagination = 0;
							} else {
								imagesCurrentPagination = 1;
							}
							idImageSelectButton = -1;
							previewTexture = null;
							imagesScrollPosition = Vector2.zero;														
						}
						enumRowsOptions = (GlobalEditorVariables.rowsOptions)EditorGUI.EnumPopup( new Rect(5, 74, 35, 24), enumRowsOptions);							
            			switch (enumRowsOptions) {
                			case GlobalEditorVariables.rowsOptions._5:
								currentImagesRows = 5;
								if (!enableSearchImages) {									
									CalculateColumnSize ();
								} else {
									CalculateColumnSizeFomSearch ();
								} 	
							break;
                			case GlobalEditorVariables.rowsOptions._10:
								currentImagesRows = 10;
								if (!enableSearchImages) {
									CalculateColumnSize ();
								} else {
									CalculateColumnSizeFomSearch ();
								}																
							break;
                			case GlobalEditorVariables.rowsOptions._20:
								currentImagesRows = 20;
								if (!enableSearchImages) {
									CalculateColumnSize ();
								} else {
									CalculateColumnSizeFomSearch ();
								}																
							break;
                			case GlobalEditorVariables.rowsOptions._30:
								currentImagesRows = 30;
								if (!enableSearchImages) {
									CalculateColumnSize ();
								} else {
									CalculateColumnSizeFomSearch ();
								}																
							break;
							default:
								currentImagesRows = 5;
								if (!enableSearchImages) {
									CalculateColumnSize ();
								} else {
									CalculateColumnSizeFomSearch ();
								} 								
							break;
						}											
						GUILayout.FlexibleSpace();
						if (currentImagesRows != lastCurrentImagesRows) {
							lastCurrentImagesRows = currentImagesRows;
							infoNameFile = null;
							isInUseImage = false;
							if (questionEditorSettings != null) {
								questionEditorSettings.SaveEnumPopupImagesSettings (currentImagesRows);
							}							
						}

						int totalImagesFiles = 0;
						if (allImagesInGallery != null && allImagesInGallery.image != null) {
							totalImagesFiles = allImagesInGallery.image.Count;
							if (enableSearchImages) {
								totalImagesFiles = imagesSearchLenghtPagination;
							}
						}
						bool enableRightShowPagination = false;
						int startRowsTmp = startRows +1;
						if (enableSearchImages && imagesSearchLenghtPagination == 0) {
							itemsStr = "No match found";
							enableRightShowPagination = true;
						} else {
							itemsStr = startRowsTmp.ToString ()+"-"+endRows.ToString ()+" of "+totalImagesFiles.ToString ();
						}
						GUILayout.Label(itemsStr, EditorStyles.boldLabel);
						if (leftTexture2D != null) {
							bool enableLeftButton = false;
							if ( (imagesCurrentPagination == 1 || imagesLenghtPagination == 0) || 
								(enableSearchImages && imagesSearchLenghtPagination == 0) ) {	
								enableLeftButton = true;
							}
							EditorGUI.BeginDisabledGroup (enableLeftButton);
							if (GUILayout.Button(new GUIContent(leftTexture2D), GUILayout.Width(20), GUILayout.Height(20))) {
								imagesCurrentPagination--;
								if (imagesCurrentPagination <= 0) {
									imagesCurrentPagination = 1;
								} else {
									idImageSelectButton = -1;
									previewTexture = null;									
								}
								imagesScrollPosition = Vector2.zero;
								infoNameFile = null;
								isInUseImage = false;
							}
							EditorGUI.EndDisabledGroup();
						}
						EditorGUI.BeginDisabledGroup (enableRightShowPagination);
						GUILayout.Label(imagesCurrentPagination.ToString (), EditorStyles.boldLabel);
						EditorGUI.EndDisabledGroup();	
						if (rightTexture2D != null) {
							bool enableRightButton = false;
							if ( (imagesCurrentPagination == imagesPagination) || 
								(enableSearchImages && imagesSearchLenghtPagination == 0) ) {	
								enableRightButton = true;
							}
							EditorGUI.BeginDisabledGroup (enableRightButton);							
							if (GUILayout.Button(new GUIContent(rightTexture2D), GUILayout.Width(20), GUILayout.Height(20))) {
								imagesCurrentPagination++;
								if (imagesCurrentPagination > imagesPagination) {
									imagesCurrentPagination = imagesPagination;
								} else {
									idImageSelectButton = -1;
									previewTexture = null;
								}
								imagesScrollPosition = Vector2.zero;
								infoNameFile = null;
								isInUseImage = false;
							}
							EditorGUI.EndDisabledGroup();
						}
						EditorGUI.EndDisabledGroup();																													
					GUILayout.EndHorizontal();				
					GUILayout.BeginHorizontal("label", GUILayout.Height(200));
						bool enableRightInfoButton = false;
						if (infoNameFile == null) {
							enableRightInfoButton = true;
						} else {
							if (infoNameFile.Length == 0) {
								enableRightInfoButton = true;
							}
						}
						// Show image information.
						EditorGUI.BeginDisabledGroup (enableRightInfoButton);
							if (GUILayout.Button(new GUIContent(infoTexture2D, "Image information"), 
								GUILayout.Width(20), GUILayout.Height(20))) {
								editorTools.InfoFileDialog (questionImagesFolder, infoNameFile);
							}
						EditorGUI.EndDisabledGroup();
						GUILayout.FlexibleSpace();
						if (previewTexture != null) {
							GUILayout.Box(new GUIContent(previewTexture), GUILayout.Width(200), GUILayout.Height(200));
						} else {
							GUILayout.Box(" ", GUILayout.Width(200), GUILayout.Height(200));
						}
						GUILayout.FlexibleSpace();
						bool enableRightInUseImageButton = true;
						string msgText = "Unused image";
						if (isInUseImage && indexImagesInGallery != null) {
							enableRightInUseImageButton = false;							
							msgText = "This image is in use for a question: "+indexImagesGalleryStr;
						}						
						EditorGUI.BeginDisabledGroup (enableRightInUseImageButton);
							GUILayout.Label(new GUIContent(inUseImageTexture2D, msgText), GUILayout.Width(20), GUILayout.Height(20));
						EditorGUI.EndDisabledGroup();						
					GUILayout.EndHorizontal();					
					imagesScrollPosition = GUILayout.BeginScrollView(
						imagesScrollPosition, GUILayout.Width(widthRightScrollView), GUILayout.Height(heightRightScrollView));
					Texture2D texture = defaultTexture2D;
					Texture2D textureTmp;
					// Show image list.						
					if (!enableSearchImages) {
						if (allImagesInGallery != null && allImagesInGallery.image != null) {
							for (int aloop = startRows; aloop < endRows; aloop++) {
								GUIStyle getLabelStyle = new GUIStyle (EditorStyles.label);
								if (aloop == idImageSelectButton) {
									getLabelStyle = new GUIStyle (EditorStyles.helpBox);
								}
								GUILayout.BeginHorizontal (getLabelStyle);
									GUILayout.BeginVertical("label", GUILayout.Height(40));
										GUILayout.FlexibleSpace();
										var labelStyle = GUI.skin.GetStyle("label");
										labelStyle.alignment = TextAnchor.MiddleCenter;
										int bloop = aloop +1;
										GUILayout.Label(bloop.ToString (), labelStyle);
										GUILayout.FlexibleSpace();
									GUILayout.EndVertical();
									if (allImagesInGallery.imageTexture != null && editorTools != null && 
										allImagesInGallery.image [aloop] != null) {
										if (!allImagesInGallery.isTexture [aloop]) {
											textureTmp = editorTools.GetTexture2DFromStreamingFolder (
												questionImagesFolder, allImagesInGallery.image [aloop]);
											if (textureTmp != null) {
												allImagesInGallery.imageTexture [aloop] = textureTmp;
												allImagesInGallery.isTexture [aloop] = true;
											} else {
												allImagesInGallery.imageTexture [aloop] = defaultTexture2D;
												allImagesInGallery.isTexture [aloop] = true;												
											}																					
										}
										if (allImagesInGallery.isTexture [aloop]) {
											texture = allImagesInGallery.imageTexture [aloop];
										}
									}
									GUILayout.Label(new GUIContent(texture), GUILayout.Width(40), GUILayout.Height(40));
									GUI.skin.button.alignment = TextAnchor.MiddleCenter;
									GUILayout.BeginVertical("label", GUILayout.Height(40));
										GUILayout.FlexibleSpace();								
										if (GUILayout.Button(allImagesInGallery.image [aloop], GUILayout.Width(widthRightTextField), 
											GUILayout.Height(30))) {
											idImageSelectButton = aloop;
											infoNameFile = allImagesInGallery.image [aloop];
											previewTexture = editorTools.GetTexture2DFromStreamingFolder (
												questionImagesFolder, allImagesInGallery.image [aloop]);
											if (previewTexture == null) {
												editorTools.FileNotExistDialog (allImagesInGallery.image [aloop]);
												infoNameFile = null;
												isInUseImage = false;
											}
											CheckIfImageUsed (infoNameFile);
										}
										GUILayout.FlexibleSpace();
									GUILayout.EndVertical();								
									GUILayout.Label(" ", EditorStyles.label);
								GUILayout.EndHorizontal();
							}
						}
					} else {
						// Show image list.
						if (searchImagesInGallery != null && searchImagesInGallery.image != null && imagesSearchLenghtPagination > 0) {
							for (int aloop = startRows; aloop < endRows; aloop++) {
								GUIStyle getLabelStyle = new GUIStyle (EditorStyles.label);
								if (aloop == idImageSelectButton) {
									getLabelStyle = new GUIStyle (EditorStyles.helpBox);
								}								
								GUILayout.BeginHorizontal (getLabelStyle);
									GUILayout.BeginVertical("label", GUILayout.Height(40));
										GUILayout.FlexibleSpace();
										var labelStyle = GUI.skin.GetStyle("label");
										labelStyle.alignment = TextAnchor.MiddleCenter;
										int bloop = aloop +1;
										GUILayout.Label(bloop.ToString (), labelStyle);
										GUILayout.FlexibleSpace();
									GUILayout.EndVertical();
									if (searchImagesInGallery.imageTexture != null && editorTools != null && 
										searchImagesInGallery.image [aloop] != null) {
										if (!searchImagesInGallery.isTexture [aloop]) {
											textureTmp = editorTools.GetTexture2DFromStreamingFolder (
												questionImagesFolder, searchImagesInGallery.image [aloop]);
											if (textureTmp != null) {
												searchImagesInGallery.imageTexture [aloop] = textureTmp;
												searchImagesInGallery.isTexture [aloop] = true;
											} else {
												searchImagesInGallery.imageTexture [aloop] = defaultTexture2D;
												searchImagesInGallery.isTexture [aloop] = true;												
											}																					
										}
										if (searchImagesInGallery.isTexture [aloop]) {
											texture = searchImagesInGallery.imageTexture [aloop];
										}
									}
									GUILayout.Label(new GUIContent(texture), GUILayout.Width(40), GUILayout.Height(40));
									GUI.skin.button.alignment = TextAnchor.MiddleCenter;
									GUILayout.BeginVertical("label", GUILayout.Height(40));
										GUILayout.FlexibleSpace();																	
										if (GUILayout.Button(searchImagesInGallery.image [aloop], GUILayout.Width(widthRightTextField), 
											GUILayout.Height(30))) {
											idImageSelectButton = aloop;
											infoNameFile = searchImagesInGallery.image [aloop];
											previewTexture = editorTools.GetTexture2DFromStreamingFolder (
												questionImagesFolder, searchImagesInGallery.image [aloop]);
											if (previewTexture == null) {
												editorTools.FileNotExistDialog (searchImagesInGallery.image [aloop]);
												infoNameFile = null;
												isInUseImage = false;
											}
											CheckIfImageUsed (infoNameFile);
										}
										GUILayout.FlexibleSpace();
									GUILayout.EndVertical();																	
									GUILayout.Label(" ", EditorStyles.label);
								GUILayout.EndHorizontal();
							}
						}
					}
					GUILayout.EndScrollView();
				GUILayout.EndArea();
        }

		// Class reference of the editor.
        public void SetQuestionAnswerManagerObjectReference (QuestionAnswerManager obj) {
            questionAnswerManager = obj;
        } 

		// Load the textures for the editor.
		public void LoadImagesFromEditorFolder () {
			searchTexture2D = editorTools.GetTexture2DFromEditorFolder (GlobalEditorVariables.searchNameImage);
			closeTexture2D = editorTools.GetTexture2DFromEditorFolder (GlobalEditorVariables.closeNameImage);
			leftTexture2D = editorTools.GetTexture2DFromEditorFolder (GlobalEditorVariables.leftNameImage);
			rightTexture2D = editorTools.GetTexture2DFromEditorFolder (GlobalEditorVariables.rightNameImage);
			infoTexture2D = editorTools.GetTexture2DFromEditorFolder (GlobalEditorVariables.infoNameImage);
			inUseImageTexture2D = editorTools.GetTexture2DFromEditorFolder (GlobalEditorVariables.inUseNameImage);
		}

		// Load textures from StreamingAssets folder.
		public void LoadImagesFromStreamingFolder () {
			string pathTexture = Application.dataPath + GlobalEditorVariables.idEditorDefaultResourcesFolder + 
				GlobalEditorVariables.idEditorImagesFolder + GlobalEditorVariables.nameDefaultImage;
			var imgBytes = File.ReadAllBytes (pathTexture);
			defaultTexture2D = new Texture2D(2, 2);
			defaultTexture2D.LoadImage(imgBytes);				
			List<string> nameFiles = editorTools.LoadImagesFromStreamingFolder (questionImagesFolder);
			if (nameFiles != null) {
				foreach (string nameFile in nameFiles) {	
					allImagesInGallery.imageTexture.Add (new Texture2D(2, 2));
					allImagesInGallery.isTexture.Add (false);
					allImagesInGallery.image.Add (nameFile);
				}
			}
			imagesLenghtPagination = 0;
			if (allImagesInGallery != null) {
				imagesLenghtPagination = allImagesInGallery.image.Count;
			}
			imagesSearchLenghtPagination = 0;
			searchTextCompare = "";
			searchText = "";
			searchImagesInGallery = null;
			infoNameFile = null;
			isInUseImage = false;
		}        

		// Resize the editor window.
		public void ChangeSizeWindow (Rect position) {
			Rect windowSize = position;
			if (windowSize.width > GlobalEditorVariables.widthImageGallery) {
				float subWidth = windowSize.width - GlobalEditorVariables.widthImageGallery;
				widthMainEditor = windowSize.width;
				float widthTmpRightArea = widthMainEditor;
				widthRightArea = widthTmpRightArea - 4;
				widthRightScrollView = widthTmpRightArea - 10;
				widthRightTextField = widthTmpRightArea - 140;
				widthRightSearch = widthTmpRightArea - 100;
				if (widthRightSearch > 500) {
					widthRightSearch = 500;
				}				
			} else {
				widthMainEditor = GlobalEditorVariables.widthImageGallery;
				widthRightArea = 306;
				widthRightScrollView = 300;
				widthRightTextField = widthRightTextFieldConst;
				widthRightSearch = 200;
			}
			if (windowSize.height > GlobalEditorVariables.heightImageGallery) {
				float subHeight = (windowSize.height - GlobalEditorVariables.heightImageGallery);
				heighRightArea = heighRightAreaConst + subHeight;
				heightRightScrollView = heightRightScrollViewConst + subHeight;
			} else {
				heighRightArea = heighRightAreaConst;
				heightRightScrollView = heightRightScrollViewConst;
			}			
		}

		// Check if the image is used for a question.
		public void CheckIfImageUsed (string nameFile) {
			isInUseImage = true;
			List<int> questionIndex = editorTools.CheckIfImageUsedForQuestion (filePathQuestionsJson, nameFile);
			if (questionIndex == null) {
				isInUseImage = false;
			} else if (questionIndex.Count == 0) {
				isInUseImage = false;
			}
			indexImagesInGallery = null;
			indexImagesGalleryStr = "";
			if (isInUseImage) {
				indexImagesInGallery = questionIndex;
				if (indexImagesInGallery != null) {
					if (indexImagesInGallery.Count > 0) {
						foreach (int questionImage in indexImagesInGallery) {
							indexImagesGalleryStr += questionImage.ToString () + ",";
						}
						if (indexImagesGalleryStr != null) {
							if (indexImagesGalleryStr.Length > 0) {
								indexImagesGalleryStr = indexImagesGalleryStr.Remove(indexImagesGalleryStr.Length - 1);
							}
						}
					}
				}
			}		
		}

		// Calculate column size.
		void CalculateColumnSize () {
			imagesPagination = (int)Mathf.Ceil ((float)imagesLenghtPagination / currentImagesRows);
			if (imagesPagination > 0 && imagesLenghtPagination > 0 && imagesCurrentPagination == 0) {
				imagesCurrentPagination = 1;
			}								
			startRows = 0;
			endRows = imagesCurrentPagination * currentImagesRows;
			startRows = (int)Mathf.Abs (endRows - currentImagesRows);
			if (endRows > imagesLenghtPagination) {
				endRows = imagesLenghtPagination;
			}			
		}

		// Calculate column size in search.
		void CalculateColumnSizeFomSearch () {
			if (searchImagesInGallery != null && searchImagesInGallery.image != null) {
				if (imagesSearchLenghtPagination > 0) {	
					imagesPagination = (int)Mathf.Ceil ((float)imagesSearchLenghtPagination / currentImagesRows);
					if (imagesPagination > 0 && imagesSearchLenghtPagination > 0 && imagesCurrentPagination == 0) {
						imagesCurrentPagination = 1;
					}								
					startRows = 0;
					endRows = imagesCurrentPagination * currentImagesRows;
					startRows = (int)Mathf.Abs (endRows - currentImagesRows);
					if (endRows > imagesSearchLenghtPagination) {
						endRows = imagesSearchLenghtPagination;
					}
				} else {
					imagesCurrentPagination = 0;
					endRows = 0;
					startRows = -1;
				}
			}
		}
    }
}
