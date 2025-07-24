// Create a window where the list of questions will be displayed.
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace JsonQuizEditor.Scripts
{
    public class QuestionAnswerEditor 
    {
        float xCenterArea = 10;
		Texture2D defaultTexture2D = null;
		Texture2D searchTexture2D = null;
		Texture2D closeTexture2D = null;
		Texture2D leftTexture2D = null;
		Texture2D rightTexture2D = null;
		Texture2D settingTexture2D = null;
        EditorTools editorTools = null;
		string searchText = null;
		string searchTextCompare = null;
        float widthSearchConst = 150.0f;
		float widthSearch = 150.0f;                 
		GlobalEditorVariables.rowsOptions enumRowsOptions = GlobalEditorVariables.rowsOptions._5;
		int currentQuestionRow = 0;
		int previousQuestionRow = 0;
		int previousRow = 0;
		int startRows = 0;
		int endRows = 0;
		int questionsPagination = 0;
		int paginationLength = 0;
		int currentQuestionPagination = 0;
		int searchPaginationLength = 0;
		bool mainToggle = false;
		List<bool> togglesQuestions = null;
		Vector2 questionsScrollPosition;
		Texture2D lighterColorTexture2D;
		Texture2D lessLightColorTexture2D;
		Texture2D separationTexture2D;
		int heightItem = 50;
		int heightScroll = 0;
		string [] optionsSubMenu = { "Edit"}; // Submenu options.
    	List<int> indexSubMenu = null;
		float xImagesSeparation = 180;
		float xQuestionsSeparation = 250;
		float xTagsSeparation = 320;
		float xOptionsSeparation = 450;
		float widthMainEditor = 0.0f;
		float heightMainEditor = 0.0f;
		bool enableQuestionSearch = false;
		AllQuestions allQuestions = null; // This class contains all the questions and answers.
		AllQuestions allSearchQuestions = null; // This class contains all the questions and answers included in a search.
		bool enableRowsOptions = false;
		bool lockMainToggle = false;
		JsonManager jsonManager = null; // Read and write json file.
		QuestionEditorSettings questionEditorSettings = null; // Configuration of questions.
		QuestionAnswerManager questionAnswerManager = null; // Add and edit questions.
		bool showCreateQuestion = false;		
        string filePathQuestionsJson = null; // Path of the json file that contains the questions.
        string questionImagesFolder = null; // Folder containing the images that will be used in the questions.
		bool showSettings = false;
		EditorSettings editorSettings = null; // Editor settings.

		// Initialize variables.
		public QuestionAnswerEditor () {
			xCenterArea = 2;
			widthMainEditor = GlobalEditorVariables.widthEditor - 4;
            heightMainEditor = GlobalEditorVariables.heightEditor - 2;
            widthSearch = widthSearchConst;
			startRows = 0;
			endRows = 0;			
			questionsPagination = 0;
			paginationLength = 0;
			currentQuestionPagination = 0;
			searchPaginationLength = 0;
			currentQuestionRow = 0;
			previousRow = 0;
			if (jsonManager == null) {
				jsonManager = new JsonManager ();
				DeserializeEditorConfig desEditorConfig = jsonManager.ReadEditorConfigJson ();
				if (desEditorConfig != null) {
					filePathQuestionsJson = desEditorConfig.filePathQuestionsJson;
					questionImagesFolder = desEditorConfig.questionImagesFolder;				
				}
			}							
            if (editorTools == null) {		            
                editorTools = new EditorTools ();
            }
			if (editorSettings == null) {
				editorSettings = new EditorSettings ();
				editorSettings.SetQuestionEditorObjectReference (this);
			}			
			if (questionEditorSettings == null) {
				questionEditorSettings = new QuestionEditorSettings ();
				questionEditorSettings.InitEnvironment ();				
			}			
			if (searchText == null) {
				searchText = "";
			}
			if (searchTextCompare == null) {
				searchTextCompare = "";
			}
			heightItem = 50;
			heightScroll = heightItem;
			mainToggle = false;
			lockMainToggle = false;				
			if (allQuestions == null) {
				allQuestions = new AllQuestions ();
			}
			if (questionAnswerManager == null) {
				questionAnswerManager = new QuestionAnswerManager ();
				questionAnswerManager.SetQuestionEditorObjectReference (this);
			}									
			LoadColorTexture2D ();
			LoadQuestionsFromStreamingFolder ();
			xImagesSeparation = questionEditorSettings.ReadImagesSeparationSettings ();
			xQuestionsSeparation = questionEditorSettings.ReadQuestionsSeparationSettings ();
			xTagsSeparation = questionEditorSettings.ReadTagsSeparationSettings ();
			xOptionsSeparation = questionEditorSettings.ReadOptionsSeparationSettings ();
			enumRowsOptions = questionEditorSettings.ReadEnumPopupSettings ();			          
        }
		
		public void ResetEnvironment () {
			showCreateQuestion = false;
			showSettings = false;
		}

		// Draw UI.
        public void QuestionsGUI () {
			if (showCreateQuestion) {
				questionAnswerManager.CreateQuestionGUI ();
				return;
			}
			if (showSettings) {
				editorSettings.EditorSettingsGUI ();
				return;
			}
			GUIStyle searchTextStyle = new GUIStyle (EditorStyles.textField);
			searchTextStyle.margin = new RectOffset (0, 0, 0, 0);
			searchTextStyle.padding = new RectOffset (4, 4, 0, 0);
			searchTextStyle.alignment = TextAnchor.MiddleLeft;
            GUIStyle separationFieldStyle = new GUIStyle (EditorStyles.label);
            separationFieldStyle.alignment = TextAnchor.UpperLeft;
			GUIStyle ascendingCounterTextStyle = new GUIStyle (EditorStyles.label);
			ascendingCounterTextStyle.alignment = TextAnchor.MiddleLeft;												
			string itemsStr = "";	
			var editQuestionsStyle = new GUIStyle(GUI.skin.textArea);
			GUILayout.BeginArea(new Rect(xCenterArea, 0, widthMainEditor, heightMainEditor), editQuestionsStyle);
				EditorGUI.BeginDisabledGroup (false);
					GUILayout.BeginHorizontal( GUILayout.Width(widthMainEditor - 6), GUILayout.Height(25));
						bool enableLeftRowOptions = false;
						if (!enableQuestionSearch) {
							if (paginationLength == 0) {
								enableLeftRowOptions = true;
							}
						} else {
							if (searchPaginationLength == 0) {
								enableLeftRowOptions = true;
							}
						}
						GUILayout.BeginHorizontal( GUILayout.Width(350), GUILayout.Height(25));
							EditorGUI.BeginDisabledGroup (enableLeftRowOptions);
								if (GUILayout.Button("Delete", GUILayout.Width(65))) {
									bool isSelect = false;
									if (allQuestions != null) {
										if (allQuestions.questionName != null) {
											for (int loop = 0; loop < togglesQuestions.Count; loop++) {
												if (togglesQuestions [loop]) {
													isSelect = true;
													break;
												}
											}
											if (!isSelect) {
												editorTools.SelectSomeQuestionDialog ();
											} else {
												if (allQuestions != null) {
													if (allQuestions.questionName != null) {
														// Delete the selected question.
														if (allQuestions.questionName.Count > 0 && !enableQuestionSearch) {																						
															if (EditorUtility.DisplayDialog("Delete selected asset?",
															"Are you sure you want to delete the question or questions?\nYou cannot undo this action.", 
																"Yes", "Cancel")) {
																if (editorTools.DeleteQuestions (
																	filePathQuestionsJson, allQuestions, togglesQuestions)) {
																	ReloadQuestions (false);
																	questionsPagination = (int)Mathf.Ceil ((float)paginationLength / currentQuestionRow);
																	if (currentQuestionPagination >= questionsPagination) {
																		currentQuestionPagination = questionsPagination;
																	}									 	
																} else {
																	editorTools.UnexpectedErrorOccurred ();										
																}												
															}
														}
													}
												}
												if (allSearchQuestions != null) {
													if (allSearchQuestions.questionName != null) {
														// Delete the selected question.
														if (searchPaginationLength > 0 && allSearchQuestions.questionName.Count >= 0 && 
															enableQuestionSearch) {										
															if (EditorUtility.DisplayDialog("Delete selected asset?",
															"Are you sure you want to delete the question or questions?\nYou cannot undo this action.", 
																"Yes", "Cancel")) {
																if (editorTools.DeleteSearchQuestions (filePathQuestionsJson, allQuestions, togglesQuestions, 
																	allSearchQuestions)) {
																	string searchTextB = searchText;
																	string searchTextCompareB = searchTextCompare;
																	ReloadQuestions (false);	
																	searchText = searchTextB;
																	searchTextCompare = searchTextCompareB;													
																	allSearchQuestions = editorTools.GetSearchQuestions (allQuestions, searchText);;
																	if (allSearchQuestions != null) {
																		if (allSearchQuestions.questionName != null) {
																			if (allSearchQuestions.questionName.Count > 0) {
																				enableQuestionSearch = true;
																				searchPaginationLength = allSearchQuestions.questionName.Count;
																			} else {
																				enableQuestionSearch = true;
																				searchPaginationLength = 0;								
																			}
																		}																									
																	} else {
																		searchText = "";
																		searchTextCompare = "";	
																	}
																	questionsPagination = (int)Mathf.Ceil ((float)paginationLength / currentQuestionRow);
																	if (currentQuestionPagination >= questionsPagination) {
																		currentQuestionPagination = questionsPagination;
																	}																											 	
																} else {
																	editorTools.UnexpectedErrorOccurred ();										
																}												
															}
														}
													}
												} 
											}
										}
									}
									GUIUtility.ExitGUI ();																
								}
								GUILayout.FlexibleSpace();
								enableLeftRowOptions = false;
								if (paginationLength == 1 || enableQuestionSearch) {
									enableLeftRowOptions = true;
								}					
								EditorGUI.BeginDisabledGroup (enableLeftRowOptions);
									// Move the selected questions up.							 
									if (GUILayout.Button("Up", GUILayout.Width(50))) {
										bool status = false;
										if (togglesQuestions != null && editorTools != null && allQuestions != null) {
											for (int loop = 0; loop < togglesQuestions.Count; loop++) {
												if (togglesQuestions [loop]) {
													status = true;
												}
											}
											if (status) {
												List<bool> newToggleStatus = editorTools.MoveQuestionUp (
													filePathQuestionsJson, allQuestions, togglesQuestions);
												if (newToggleStatus != null) {
													if (newToggleStatus.Count == togglesQuestions.Count) {
														for (int loop = 0; loop < togglesQuestions.Count; loop++) {
															togglesQuestions [loop] = newToggleStatus [loop];
														}
													}
												}
											} else {
												editorTools.SelectSomeQuestionDialog ();
											}
										}
									}
									// Move the selected questions down.
									if (GUILayout.Button("Down", GUILayout.Width(50))) {
										bool status = false;
										if (togglesQuestions != null && editorTools != null && allQuestions != null) {
											for (int loop = 0; loop < togglesQuestions.Count; loop++) {
												if (togglesQuestions [loop]) {
													status = true;
												}
											}
											if (status) {										
												List<bool> newToggleStatus = editorTools.MoveQuestionDown (
														filePathQuestionsJson, allQuestions, togglesQuestions);
												if (newToggleStatus != null) {
													if (newToggleStatus.Count == togglesQuestions.Count) {
														for (int loop = 0; loop < togglesQuestions.Count; loop++) {
															togglesQuestions [loop] = newToggleStatus [loop];
														}
													}
												}
											} else {
												editorTools.SelectSomeQuestionDialog ();
											}
										}
									}
								EditorGUI.EndDisabledGroup();
							EditorGUI.EndDisabledGroup();
							GUILayout.FlexibleSpace();						 
							if (GUILayout.Button("Reload", GUILayout.Width(65))) {
								ReloadQuestions ();
							}
							GUILayout.FlexibleSpace(); 																
							if (GUILayout.Button("Add a Question", GUILayout.Width(105))) {
								ResetTogglesQuestions (false);
								questionAnswerManager.DefaultQuestionData ();
								questionAnswerManager.ResetAnswerType ();
								showCreateQuestion = true;
							}
						GUILayout.EndHorizontal();
						GUILayout.FlexibleSpace();
						if (GUILayout.Button(new GUIContent(settingTexture2D, "Settings"), GUILayout.Width(20), GUILayout.Height(20))) {
							showSettings = true;
						}
					GUILayout.EndHorizontal();
				GUILayout.BeginArea(new Rect(0, 20, widthMainEditor, 7));
					EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
				GUILayout.EndArea();								
				EditorGUI.EndDisabledGroup();

				// Lines
				GUILayout.BeginArea(new Rect(0, 45, widthMainEditor, 7));
					EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
				GUILayout.EndArea();
				GUILayout.BeginArea(new Rect(0, 65, widthMainEditor, 7));
					EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
				GUILayout.EndArea();
				// Lines

				// Pagination
				GUILayout.BeginArea(new Rect(2, 30, widthMainEditor, 26));
					GUILayout.BeginHorizontal(GUILayout.Width(widthSearch), GUILayout.Height(26));
						bool enableSearch = false;
						if (!enableQuestionSearch && paginationLength == 0) {
							enableSearch = true;
						}
						EditorGUI.BeginDisabledGroup (enableSearch);
							if (searchTexture2D != null) {
								GUILayout.Label(new GUIContent(searchTexture2D), GUILayout.Width(16), GUILayout.Height(16));
							}
							if (searchText == null) {
								searchText = "";
							}
							searchText = GUILayout.TextField(searchText, searchTextStyle, GUILayout.Width(widthSearch - 10), 
								GUILayout.Height(19));
							if ( !string.Equals(searchText, searchTextCompare) ) {
								searchTextCompare = searchText;
								if (editorTools != null) {	
									allSearchQuestions = editorTools.GetSearchQuestions (allQuestions, searchText);
								}
								searchPaginationLength = 0;
								currentQuestionPagination = 1;
								questionsScrollPosition = Vector2.zero;
								ResetTogglesQuestions (false);
								lockMainToggle = false;
								mainToggle = false;																	
							}
							if (allSearchQuestions != null) {
								if (allSearchQuestions.questionName != null) {
									if (allSearchQuestions.questionName.Count > 0) {
										enableQuestionSearch = true;
										searchPaginationLength = allSearchQuestions.questionName.Count;
									} else {
										enableQuestionSearch = true;
										searchPaginationLength = 0;								
									}
								}
							}					
							if (searchText != null) {
								if (searchText.Length == 0) {
									enableQuestionSearch = false;
								}
							}
							if (closeTexture2D != null) {
								GUILayout.BeginVertical(GUILayout.Height(18));
								GUILayout.FlexibleSpace();
								// Close the editor.
								if (GUILayout.Button(new GUIContent(closeTexture2D), GUILayout.Width(16), GUILayout.Height(16))) {
									if (searchText.Length > 0) {
										searchText = "";
										searchTextCompare = "";
										allSearchQuestions = null;
										enableQuestionSearch = false;
										questionsScrollPosition = Vector2.zero;
										currentQuestionPagination = 1;
										ResetTogglesQuestions (false);
										lockMainToggle = false;
										mainToggle = false;																			
									}						
								}
								GUILayout.FlexibleSpace();
								GUILayout.EndVertical();
							}
						EditorGUI.EndDisabledGroup();
					GUILayout.EndHorizontal();

					GUILayout.BeginArea(new Rect(widthMainEditor - 305, 0, 300, 35));	
						GUILayout.BeginHorizontal();
							EditorGUI.BeginDisabledGroup (false);
							if (currentQuestionRow != previousRow) {					
								previousRow = currentQuestionRow;
								if (paginationLength == 0) {
									currentQuestionPagination = 0;
								} else {
									currentQuestionPagination = 1;
								}
								questionsScrollPosition = Vector2.zero;														
							}						
							if ( paginationLength == 0 || (enableQuestionSearch && searchPaginationLength == 0) ) {	
								enableRowsOptions = true;
							} else {
								enableRowsOptions = false;
							}
							EditorGUI.BeginDisabledGroup (enableRowsOptions);
								// Select the number of questions (5, 10, 20 and 30) that can be displayed in the window. 						
								enumRowsOptions = (GlobalEditorVariables.rowsOptions)EditorGUI.EnumPopup( new Rect(80, 0, 35, 15), 
									enumRowsOptions);
							EditorGUI.EndDisabledGroup();							
							switch (enumRowsOptions) {
								case GlobalEditorVariables.rowsOptions._5:
									currentQuestionRow = 5;
									if (!enableQuestionSearch) {
										CalculateColumnSize ();
									} else {
										CalculateColumnSizeFomSearch ();
									} 
								break;
								case GlobalEditorVariables.rowsOptions._10:
									currentQuestionRow = 10;
									if (!enableQuestionSearch) {
										CalculateColumnSize ();
									} else {
										CalculateColumnSizeFomSearch ();
									}														
								break;
								case GlobalEditorVariables.rowsOptions._20:
									currentQuestionRow = 20;
									if (!enableQuestionSearch) {
										CalculateColumnSize ();
									} else {
										CalculateColumnSizeFomSearch ();
									}																
								break;
								case GlobalEditorVariables.rowsOptions._30:
									currentQuestionRow = 30;
									if (!enableQuestionSearch) {
										CalculateColumnSize ();
									} else {
										CalculateColumnSizeFomSearch ();
									}																
								break;
								default:							
									currentQuestionRow = 5;
									if (!enableQuestionSearch) {
										CalculateColumnSize ();
									} else {
										CalculateColumnSizeFomSearch ();
									}
								break;
							}											
							GUILayout.FlexibleSpace();
							if (currentQuestionRow != previousQuestionRow) {
								previousQuestionRow = currentQuestionRow;
								ResetTogglesQuestions (false);
								lockMainToggle = false;
								mainToggle = false;
								if (questionEditorSettings != null) {
									questionEditorSettings.SaveEnumPopupSettings (currentQuestionRow);
								}							
							}

							int totalImagesFiles = paginationLength;
							if (enableQuestionSearch) {
								totalImagesFiles = searchPaginationLength;
							}						
							bool enableRightShowPager = false;
							int startRowsTmp = startRows +1;
							if (enableQuestionSearch && searchPaginationLength == 0) {
								itemsStr = "No match found";
								enableRightShowPager = true;
							} else {
								itemsStr = startRowsTmp.ToString ()+"-"+endRows.ToString ()+" of "+totalImagesFiles.ToString ();
								if (enableSearch) {
									itemsStr = "";
								}
							}
							EditorGUI.BeginDisabledGroup (enableSearch);					
								GUILayout.Label(itemsStr, EditorStyles.boldLabel);
							EditorGUI.EndDisabledGroup();
							if (leftTexture2D != null) {
								bool enableLeftButton = false;
								if ( (currentQuestionPagination == 1 || paginationLength == 0) || 
									(enableQuestionSearch && searchPaginationLength == 0) ) {	
									enableLeftButton = true;
								}
								EditorGUI.BeginDisabledGroup (enableLeftButton);
								if (GUILayout.Button(new GUIContent(leftTexture2D), GUILayout.Width(18), GUILayout.Height(18))) {
									currentQuestionPagination--;
									if (currentQuestionPagination <= 0) {
										currentQuestionPagination = 1;
									}
									questionsScrollPosition = Vector2.zero;
									ResetTogglesQuestions (false);
									lockMainToggle = false;
									mainToggle = false;
								}
								EditorGUI.EndDisabledGroup();
							}
							if (enableSearch) {
								enableRightShowPager = true;
							}
							EditorGUI.BeginDisabledGroup (enableRightShowPager);
								GUILayout.Label(currentQuestionPagination.ToString (), EditorStyles.boldLabel);
							EditorGUI.EndDisabledGroup();	
							if (rightTexture2D != null) {
								bool enableRightButton = false;
								if ( (currentQuestionPagination == questionsPagination) || 
									(enableQuestionSearch && searchPaginationLength == 0) ) {	
									enableRightButton = true;
								}
								EditorGUI.BeginDisabledGroup (enableRightButton);							
								if (GUILayout.Button(new GUIContent(rightTexture2D), GUILayout.Width(18), GUILayout.Height(18))) {
									currentQuestionPagination++;
									if (currentQuestionPagination > questionsPagination) {
										currentQuestionPagination = questionsPagination;
									}
									questionsScrollPosition = Vector2.zero;
									ResetTogglesQuestions (false);
									lockMainToggle = false;
									mainToggle = false;
								}
								EditorGUI.EndDisabledGroup();
							}
							EditorGUI.EndDisabledGroup();																													
						GUILayout.EndHorizontal();
					GUILayout.EndArea();
				GUILayout.EndArea();	
				// Pagination

				// Displays the column names.
				EditorGUI.BeginDisabledGroup (enableRowsOptions);
					mainToggle = EditorGUI.Toggle(new Rect(25, 53, 20, 20), mainToggle);
					if (mainToggle && !lockMainToggle) {
						for (int loop = startRows; loop < endRows; loop++) {
							togglesQuestions [loop] = true;
						}
						lockMainToggle = true;
					} 
					if (!mainToggle && lockMainToggle) {
						for (int loop = startRows; loop < endRows; loop++) {
							togglesQuestions [loop] = false;
						}						
						lockMainToggle = false;
					}					
					EditorGUI.LabelField(new Rect((xImagesSeparation / 2) - 5, 53, 100, 20), "Image", separationFieldStyle);					
					EditorGUI.LabelField(new Rect(xImagesSeparation, 53, 100, 20), "Question Name", separationFieldStyle);
					EditorGUI.DrawRect(new Rect(xImagesSeparation - 14, 54, 22, 16), new Color(0.9f, 0.9f, 0.2f, 0.0f));
					EditorGUIUtility.AddCursorRect(new Rect(xImagesSeparation - 14, 54, 22, 16), MouseCursor.ResizeHorizontal);
					EditorGUI.LabelField(new Rect(xImagesSeparation - 10, 54, 6, 16), 
						new GUIContent(separationTexture2D), separationFieldStyle);
					EditorGUI.LabelField(new Rect(xQuestionsSeparation, 53, 100, 20), "Answer types", separationFieldStyle);
					EditorGUI.DrawRect(new Rect(xQuestionsSeparation - 14, 54, 14, 16), new Color(0.9f, 0.9f, 0.2f, 0.0f));
					EditorGUIUtility.AddCursorRect(new Rect(xQuestionsSeparation - 14, 54, 22, 16), MouseCursor.ResizeHorizontal);					
					EditorGUI.LabelField(new Rect(xQuestionsSeparation - 10, 54, 6, 16), 
						new GUIContent(separationTexture2D), separationFieldStyle);
					EditorGUI.LabelField(new Rect(xTagsSeparation, 53, 100, 20), "Tags", separationFieldStyle);
					EditorGUI.DrawRect(new Rect(xTagsSeparation - 14, 54, 14, 16), new Color(0.9f, 0.9f, 0.2f, 0.0f));
					EditorGUIUtility.AddCursorRect(new Rect(xTagsSeparation - 14, 54, 22, 16), MouseCursor.ResizeHorizontal);						
					EditorGUI.LabelField(new Rect(xTagsSeparation - 10, 54, 6, 16), 
						new GUIContent(separationTexture2D), separationFieldStyle);	
					EditorGUI.LabelField(new Rect(xOptionsSeparation, 53, 100, 20), "Options", separationFieldStyle);
					EditorGUI.DrawRect(new Rect(xOptionsSeparation - 14, 54, 14, 16), new Color(0.9f, 0.9f, 0.2f, 0.0f));
					EditorGUIUtility.AddCursorRect(new Rect(xOptionsSeparation - 14, 54, 22, 16), MouseCursor.ResizeHorizontal);						
					EditorGUI.LabelField(new Rect(xOptionsSeparation - 10, 54, 6, 16), 
						new GUIContent(separationTexture2D), separationFieldStyle);	
				EditorGUI.EndDisabledGroup();								
				// Bar

				if (endRows < 0) {
					endRows = 0;
				}
				if (startRows < 0) {
					startRows = 0;
				}				
				heightScroll = (endRows - startRows) * heightItem;
				// Displays the list of questions that have been created.
				questionsScrollPosition = GUI.BeginScrollView(
					new Rect(0, 73, widthMainEditor, heightMainEditor - 73), questionsScrollPosition , 
					new Rect(0, 0, GlobalEditorVariables.widthEditor - 40, heightScroll));
					bool textureUse = true;
					Texture2D tmpTexture2D = lighterColorTexture2D;
					int bLoop = 0;	
					for (int aloop = startRows; aloop < endRows; aloop++) {	
						int yPosItem = heightItem * bLoop++;
						if (textureUse) {
							tmpTexture2D = lighterColorTexture2D;
						} else {
							tmpTexture2D = lessLightColorTexture2D;
						}
						Texture2D texture = defaultTexture2D;
						Texture2D textureTmp;
						if (!enableQuestionSearch) {
							if (allQuestions != null) {
								if (allQuestions.texture != null && editorTools != null && allQuestions.imageName [aloop] != null) {
									if (!allQuestions.isTexture [aloop]) {
										textureTmp = editorTools.GetTexture2DFromStreamingFolder 
											(questionImagesFolder, allQuestions.imageName [aloop]);
										if (textureTmp != null) {
											allQuestions.texture [aloop] = textureTmp;
											allQuestions.isTexture [aloop] = true;
										} else {
											allQuestions.texture [aloop] = defaultTexture2D;
											allQuestions.isTexture [aloop] = true;												
										}																					
									}
									if (allQuestions.isTexture [aloop]) {
										texture = allQuestions.texture [aloop];
									}
								}
							}							
							textureUse = !textureUse;
							if (tmpTexture2D != null) {
								GUI.DrawTexture(new Rect(2, yPosItem, widthMainEditor - 4, heightItem), tmpTexture2D);
							}
							int questionNumber = aloop + 1;
							EditorGUI.LabelField(new Rect(2, yPosItem - 2, 20, 48), questionNumber.ToString (), ascendingCounterTextStyle);							
							togglesQuestions [aloop] = EditorGUI.Toggle(new Rect(25, 15 + yPosItem, 20, 20), togglesQuestions [aloop]);
							if (texture != null) {												
								GUI.DrawTexture(new Rect((xImagesSeparation / 2) - 5, yPosItem, 48, 48), texture, ScaleMode.ScaleAndCrop);
							}
							float widthField = Mathf.Abs (xQuestionsSeparation - xImagesSeparation) - 10;
							if (allQuestions != null) {
								EditorGUI.LabelField(new Rect(xImagesSeparation, 15 + yPosItem, widthField, 20), allQuestions.questionName [aloop]);
							}
							widthField = Mathf.Abs (xTagsSeparation -  xQuestionsSeparation) - 10;
							if (allQuestions != null) {
								EditorGUI.LabelField(new Rect(xQuestionsSeparation, 15 + yPosItem, widthField, 20), allQuestions.typeAnswerStr [aloop]);
							}
							widthField = Mathf.Abs (xOptionsSeparation -  xTagsSeparation) - 10;
							if (allQuestions != null) {
								EditorGUI.LabelField(new Rect(xTagsSeparation, 15 + yPosItem, widthField, 20), allQuestions.tags [aloop]);
							}
							indexSubMenu [aloop] = EditorGUI.Popup(
								new Rect(xOptionsSeparation, 15 + yPosItem, 30, 20), indexSubMenu [aloop], optionsSubMenu);
							if (indexSubMenu [aloop] != -1) {
								if (indexSubMenu [aloop] == 0) {
									if (allQuestions != null) {
										string key = allQuestions.key [aloop];
										questionAnswerManager.QuestionToEditFromEditor (key);
									}
									showCreateQuestion = true;
								}																
								indexSubMenu [aloop] = -1;
							}
						} else {
							if (allSearchQuestions != null) {
								if (allSearchQuestions.texture != null && editorTools != null && allSearchQuestions.imageName [aloop] != null) {
									if (!allSearchQuestions.isTexture [aloop]) {
										textureTmp = editorTools.GetTexture2DFromStreamingFolder 
											(questionImagesFolder, allSearchQuestions.imageName [aloop]);
										if (textureTmp != null) {
											allSearchQuestions.texture [aloop] = textureTmp;
											allSearchQuestions.isTexture [aloop] = true;
										} else {
											allSearchQuestions.texture [aloop] = defaultTexture2D;
											allSearchQuestions.isTexture [aloop] = true;												
										}																					
									}
									if (allSearchQuestions.isTexture [aloop]) {
										texture = allSearchQuestions.texture [aloop];
									}
								}
							}
							textureUse = !textureUse;
							if (tmpTexture2D != null) {
								GUI.DrawTexture(new Rect(2, yPosItem, widthMainEditor, heightItem), tmpTexture2D);
							}							
							int questionNumber = aloop + 1;
							EditorGUI.LabelField(new Rect(2, yPosItem - 2, 20, 48), questionNumber.ToString (), ascendingCounterTextStyle);							
							togglesQuestions [aloop] = EditorGUI.Toggle(new Rect(25, 15 + yPosItem, 20, 20), togglesQuestions [aloop]);
							if (texture != null) {											
								GUI.DrawTexture(new Rect((xImagesSeparation / 2) - 5, yPosItem, 48, 48), texture, ScaleMode.ScaleAndCrop);
							}
							float widthField = Mathf.Abs (xQuestionsSeparation - xImagesSeparation) - 10;
							if (allSearchQuestions != null) {
								EditorGUI.LabelField(new Rect(xImagesSeparation, 15 + yPosItem, widthField, 20), allSearchQuestions.questionName [aloop]);							
							}
							widthField = Mathf.Abs (xTagsSeparation -  xQuestionsSeparation) - 10;
							if (allSearchQuestions != null) {
								EditorGUI.LabelField(new Rect(xQuestionsSeparation, 15 + yPosItem, widthField, 20), allSearchQuestions.typeAnswerStr [aloop]);
							}
							widthField = Mathf.Abs (xOptionsSeparation -  xTagsSeparation) - 10;
							if (allSearchQuestions != null) {
								EditorGUI.LabelField(new Rect(xTagsSeparation, 15 + yPosItem, widthField, 20), allSearchQuestions.tags [aloop]);
							}
							indexSubMenu [aloop] = EditorGUI.Popup(new Rect(xOptionsSeparation, 15 + yPosItem, 30, 20), indexSubMenu [aloop], optionsSubMenu);
							if (indexSubMenu [aloop] != -1) {
								if (indexSubMenu [aloop] == 0) {
									if (allSearchQuestions != null) {
										string key = allSearchQuestions.key [aloop];
										// Update the editor.
										questionAnswerManager.QuestionToEditFromEditor (key);
									}
									showCreateQuestion = true;
								}																							
								indexSubMenu [aloop] = -1;
							}							
						}
					}																																											
				GUI.EndScrollView();
            GUILayout.EndArea();
			MouseEventosEditor ();           
        }

		// Calculates the column size.
		void CalculateColumnSize () {
			questionsPagination = (int)Mathf.Ceil ((float)paginationLength / currentQuestionRow);
			if (questionsPagination > 0 && paginationLength > 0 && currentQuestionPagination == 0) {
				currentQuestionPagination = 1;
			}								
			startRows = 0;
			endRows = currentQuestionPagination * currentQuestionRow;
			startRows = (int)Mathf.Abs (endRows - currentQuestionRow);
			if (endRows > paginationLength) {
				endRows = paginationLength;
			}			
		}

		// Calculates the column size when searching.
		void CalculateColumnSizeFomSearch () {
			if (allSearchQuestions != null) {
				if (searchPaginationLength > 0) {	
					questionsPagination = (int)Mathf.Ceil ((float)searchPaginationLength / currentQuestionRow);
					if (questionsPagination > 0 && searchPaginationLength > 0 && currentQuestionPagination == 0) {
						currentQuestionPagination = 1;
					}								
					startRows = 0;
					endRows = currentQuestionPagination * currentQuestionRow;
					startRows = (int)Mathf.Abs (endRows - currentQuestionRow);
					if (endRows > searchPaginationLength) {
						endRows = searchPaginationLength;
					}
				} else {
					currentQuestionPagination = 0;
					endRows = 0;
					startRows = -1;
				}
			}
		}

		// Detects mouse events.
		void MouseEventosEditor () {
			float minYPosition = 50;
			float maxYPosition = 75;
			float separation = 14;
			if (Event.current.type == EventType.MouseDrag) {
				float xMouse = Event.current.mousePosition.x;
				float yMouse = Event.current.mousePosition.y;
				if (xMouse >= (xImagesSeparation - separation) && 
					xMouse <= (xImagesSeparation + separation) && !enableRowsOptions) {
					if (yMouse >= minYPosition && yMouse <= maxYPosition) {
						xImagesSeparation = xMouse;
					}
				}
				if (xMouse >= (xQuestionsSeparation - separation) && 
					xMouse <= (xQuestionsSeparation + separation) && !enableRowsOptions) {
					if (yMouse >= minYPosition && yMouse <= maxYPosition) {
						xQuestionsSeparation = xMouse;
					}
				}
				if (xMouse >= (xTagsSeparation - separation) && 
					xMouse <= (xTagsSeparation + separation) && !enableRowsOptions) {
					if (yMouse >= minYPosition && yMouse <= maxYPosition) {
						xTagsSeparation = xMouse;
					}
				}
				if (xMouse >= (xOptionsSeparation - separation) && 
					xMouse <= (xOptionsSeparation + separation) && !enableRowsOptions) {
					if (yMouse >= minYPosition && yMouse <= maxYPosition) {
						xOptionsSeparation = xMouse;
					}
				}
			}
			if (Event.current.type == EventType.DragExited || Event.current.type == EventType.MouseUp || 
				Event.current.type == EventType.MouseLeaveWindow) {
				float xMouse = Event.current.mousePosition.x;
				float yMouse = Event.current.mousePosition.y;					
				if (xMouse >= (xImagesSeparation - separation) && 
					xMouse <= (xImagesSeparation + separation) && !enableRowsOptions) {
					if (yMouse >= minYPosition && yMouse <= maxYPosition) {
						questionEditorSettings.SaveImagesSeparationSettings (xImagesSeparation);
					}
				}
				if (xMouse >= (xQuestionsSeparation - separation) && 
					xMouse <= (xQuestionsSeparation + separation) && !enableRowsOptions) {
					if (yMouse >= minYPosition && yMouse <= maxYPosition) {
						questionEditorSettings.SaveQuestionsSeparationSettings (xQuestionsSeparation);
					}
				}
				if (xMouse >= (xTagsSeparation - separation) && 
					xMouse <= (xTagsSeparation + separation) && !enableRowsOptions) {
					if (yMouse >= minYPosition && yMouse <= maxYPosition) {
						questionEditorSettings.SaveTagsSeparationSettings (xTagsSeparation);
					}
				}
				if (xMouse >= (xOptionsSeparation - separation) && 
					xMouse <= (xOptionsSeparation + separation) && !enableRowsOptions) {
					if (yMouse >= minYPosition && yMouse <= maxYPosition) {
						questionEditorSettings.SaveOptionsSeparationSettings (xOptionsSeparation);
					}
				}												
			}			        
		}

		// Change the geometry of the cell elements.
		void ChangeGeometryOfItems () {
			if (xImagesSeparation < 120) {
				xImagesSeparation = 120;
			}				
			if (xQuestionsSeparation < (xImagesSeparation + 100)) {
				xQuestionsSeparation = xImagesSeparation + 100;
			}
			if (xTagsSeparation < (xQuestionsSeparation + 100)) {
				xTagsSeparation = xQuestionsSeparation + 100;
			}
			if (xOptionsSeparation < (xTagsSeparation + 70)) {
				xOptionsSeparation = xTagsSeparation + 70;
			}
			if (xOptionsSeparation > (widthMainEditor - 60)) {
				xOptionsSeparation = widthMainEditor - 60;
				if (xTagsSeparation > (xOptionsSeparation - 70)) {
					xTagsSeparation = xOptionsSeparation - 70;
				}
				if (xQuestionsSeparation > (xTagsSeparation - 100)) {
					xQuestionsSeparation = xTagsSeparation - 100;
				}				
				if (xImagesSeparation > (xQuestionsSeparation - 100)) {
					xImagesSeparation = xQuestionsSeparation - 100;
				}					
			}				
		}

		void ResetTogglesQuestions (bool status) {
			for (int loop = 0; loop < togglesQuestions.Count; loop++) {
				togglesQuestions [loop] = status;
			}
		}

		// Load textures.
		public void LoadImagesFromEditorFolder () {
			searchTexture2D = editorTools.GetTexture2DFromEditorFolder (GlobalEditorVariables.searchNameImage);
			closeTexture2D = editorTools.GetTexture2DFromEditorFolder (GlobalEditorVariables.closeNameImage);
			leftTexture2D = editorTools.GetTexture2DFromEditorFolder (GlobalEditorVariables.leftNameImage);
			rightTexture2D = editorTools.GetTexture2DFromEditorFolder (GlobalEditorVariables.rightNameImage);
			separationTexture2D = editorTools.GetTexture2DFromEditorFolder (GlobalEditorVariables.separationNameImage);
			settingTexture2D = editorTools.GetTexture2DFromEditorFolder (GlobalEditorVariables.settingNameImage);
		}

		void LoadColorTexture2D () {
			lighterColorTexture2D = editorTools.GetColorTexture2D (new Color (1.0f, 1.0f, 1.0f, 1.0f), 2, 2);
			lessLightColorTexture2D = editorTools.GetColorTexture2D (new Color (0.949f, 0.949f, 0.949f, 1.0f), 2, 2);
		}

		// Load textures from StreamingAssets folder.
		void LoadQuestionsFromStreamingFolder () {
			string pathTexture = Application.dataPath + GlobalEditorVariables.idEditorDefaultResourcesFolder + 
				GlobalEditorVariables.idEditorImagesFolder + GlobalEditorVariables.nameDefaultImage;
			var imgBytes = File.ReadAllBytes (pathTexture);
			defaultTexture2D = new Texture2D(2, 2);
			defaultTexture2D.LoadImage(imgBytes);			
			paginationLength = 0;
			List <QuestionProperties> questionProperties = jsonManager.ReadQuestionsJson (filePathQuestionsJson); 
			if (questionProperties != null) {
				foreach (QuestionProperties question in questionProperties) {
					if (question != null) {					
						allQuestions.texture.Add (new Texture2D(2, 2));
						allQuestions.isTexture.Add (false);
						allQuestions.imageName.Add (question.imageName);
						allQuestions.questionName.Add (question.questionName);
						allQuestions.tags.Add (question.tags);
						allQuestions.typeAnswer.Add (question.typeAnswer);
						string typeAnswer = editorTools.GetTypeAnswerToString (question.typeAnswer);
						allQuestions.typeAnswerStr.Add (typeAnswer);
						AllAnswers allAnswers = new AllAnswers ();
						allAnswers.answers = new List<string>();
						foreach (string answer in question.answers) {
							allAnswers.answers.Add (answer);
						}
						allAnswers.textOrImage = new List<int>();
						foreach (int textOrImage in question.textOrImage) {
							allAnswers.textOrImage.Add (textOrImage);
						}
						allAnswers.selectedAnswers = new List<bool>();
						foreach (bool selectedAnswers in question.selectedAnswers) {
							allAnswers.selectedAnswers.Add (selectedAnswers);
						}												
						allQuestions.allAnswers.Add (allAnswers);
						allQuestions.trueOrFalse.Add (question.trueOrFalse);
						allQuestions.key.Add (question.key);					
					}
				}
				if (allQuestions.questionName != null) {
					paginationLength = allQuestions.questionName.Count;
				}
			}
			CreateListsForScrollView (paginationLength);
			searchPaginationLength = 0;
			searchTextCompare = "";
			searchText = "";	
			enableQuestionSearch = false;
			ResetTogglesQuestions (false);
			lockMainToggle = false;
			mainToggle = false;				
		}

		void CreateListsForScrollView (int sizeList) {
			togglesQuestions = new List<bool>();
			indexSubMenu = new List<int>();
			for (int aloop = 0;aloop < sizeList; aloop++) {
				togglesQuestions.Add (false);
				indexSubMenu.Add (-1);
			}
		}

		void ReloadQuestions (bool resetData = true) {
			if (allQuestions != null) {
				allQuestions = new AllQuestions ();
				LoadQuestionsFromStreamingFolder ();
			}
			if (resetData) {
				questionsPagination = (int)Mathf.Ceil ((float)paginationLength / currentQuestionRow);				
				if (questionsPagination > 0) {
					currentQuestionPagination = 1;
				} else {
					currentQuestionPagination = 0;
				}
			}
			enableQuestionSearch = false;
			questionsScrollPosition = Vector2.zero;		
		}

		// Update a question.
		// bool itIsNewItem : true = new question , false = update question. 
		// string key = unique key question.
		public void ReloadFromQuestionAnswerManager (bool itIsNewItem, string key) {
			List <QuestionProperties> questionProperties = jsonManager.ReadQuestionsJson (filePathQuestionsJson);
			if (itIsNewItem) { // New question.
				paginationLength = 0;				
				if (questionProperties != null) {
					allQuestions = new AllQuestions ();
					foreach (QuestionProperties question in questionProperties) {
						if (question != null) {					
							allQuestions.texture.Add (new Texture2D(2, 2));
							allQuestions.isTexture.Add (false);
							allQuestions.imageName.Add (question.imageName);
							allQuestions.questionName.Add (question.questionName);
							allQuestions.tags.Add (question.tags);
							allQuestions.typeAnswer.Add (question.typeAnswer);
							string typeAnswer = editorTools.GetTypeAnswerToString (question.typeAnswer);
							allQuestions.typeAnswerStr.Add (typeAnswer);	
							allQuestions.key.Add (question.key);
							AllAnswers allAnswers = new AllAnswers ();
							allAnswers.answers = new List<string>();
							foreach (string answer in question.answers) {
								allAnswers.answers.Add (answer);
							}
							allAnswers.textOrImage = new List<int>();
							foreach (int textOrImage in question.textOrImage) {
								allAnswers.textOrImage.Add (textOrImage);
							}
							allAnswers.selectedAnswers = new List<bool>();
							foreach (bool selectedAnswers in question.selectedAnswers) {
								allAnswers.selectedAnswers.Add (selectedAnswers);
							}												
							allQuestions.allAnswers.Add (allAnswers);
							allQuestions.trueOrFalse.Add (question.trueOrFalse);
						}
					}
					if (allQuestions.questionName != null) {
						paginationLength = allQuestions.questionName.Count;
					}
				}
				CreateListsForScrollView (paginationLength);
				searchPaginationLength = 0;
				searchTextCompare = "";
				searchText = "";	
				enableQuestionSearch = false;
				ResetTogglesQuestions (false);
				lockMainToggle = false;
				mainToggle = false;
				questionsPagination = (int)Mathf.Ceil ((float)paginationLength / currentQuestionRow);
				currentQuestionPagination = questionsPagination;																						
			} else { // Update question.
				if (questionProperties != null) {
					allQuestions = new AllQuestions ();
					foreach (QuestionProperties question in questionProperties) {
						if (question != null) {					
							allQuestions.texture.Add (new Texture2D(2, 2));
							allQuestions.isTexture.Add (false);
							allQuestions.imageName.Add (question.imageName);
							allQuestions.questionName.Add (question.questionName);
							allQuestions.tags.Add (question.tags);
							allQuestions.typeAnswer.Add (question.typeAnswer);
							string typeAnswer = editorTools.GetTypeAnswerToString (question.typeAnswer);
							allQuestions.typeAnswerStr.Add (typeAnswer);	
							allQuestions.key.Add (question.key);
							AllAnswers allAnswers = new AllAnswers ();
							allAnswers.answers = new List<string>();
							foreach (string answer in question.answers) {
								allAnswers.answers.Add (answer);
							}
							allAnswers.textOrImage = new List<int>();
							foreach (int textOrImage in question.textOrImage) {
								allAnswers.textOrImage.Add (textOrImage);
							}
							allAnswers.selectedAnswers = new List<bool>();
							foreach (bool selectedAnswers in question.selectedAnswers) {
								allAnswers.selectedAnswers.Add (selectedAnswers);
							}												
							allQuestions.allAnswers.Add (allAnswers);
							allQuestions.trueOrFalse.Add (question.trueOrFalse);
						}
					}					
					if (enableQuestionSearch) {
						allSearchQuestions = editorTools.GetSearchQuestions (allQuestions, searchText);
						if (allSearchQuestions != null) {
							if (allSearchQuestions.questionName != null) {
								if (allSearchQuestions.questionName.Count > 0) {
									enableQuestionSearch = true;
									searchPaginationLength = allSearchQuestions.questionName.Count;
								} else {
									enableQuestionSearch = true;
									searchPaginationLength = 0;								
								}
							}
						}													
					}
				}
			}
		}

		// Resize the editor window.
		public void ChangeSizeWindow (Rect position) {
			Rect windowSize = position;			
			if (windowSize.width > GlobalEditorVariables.widthEditor) {
				float subWidth = windowSize.width - GlobalEditorVariables.widthEditor;
				widthMainEditor = windowSize.width - 4;
				float newWidthSearch = subWidth + widthSearchConst;
				if (newWidthSearch > 300) {
					newWidthSearch = 300;
				}
				widthSearch = newWidthSearch; 			
			} else {
				widthMainEditor = GlobalEditorVariables.widthEditor - 4;
				widthSearch = widthSearchConst;
			}
			if (windowSize.height > GlobalEditorVariables.heightEditor) {
				float subHeight = windowSize.height - GlobalEditorVariables.heightEditor;
				heightMainEditor = windowSize.height - 2;
			} else {
				heightMainEditor = GlobalEditorVariables.heightEditor - 2;
			}
			if (widthSearch <= widthSearchConst) {
				widthSearch = widthSearchConst;
			}
			ChangeGeometryOfItems ();
			if (showCreateQuestion) {	
				questionAnswerManager.ChangeSizeWindow (position);
			}
			if (showSettings) {	
				editorSettings.ChangeSizeWindow (position);
			}										
		}
    }
}