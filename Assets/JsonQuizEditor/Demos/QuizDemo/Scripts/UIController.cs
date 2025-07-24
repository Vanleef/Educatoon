// Quiz Demo.
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;

namespace JsonQuizDemo.Scripts
{
    public class UIController : MonoBehaviour
    {
        public GameObject mainMenu; // Main menu.
        public GameObject categoriesMenu; // Categories menu or tags menu.
        public GameObject levelMenu; // Level menu.
        public GameObject gameOverMenu; // Game over menu.
        public GameObject playButton;
        public GameObject[] homeButtons;
        public GameObject categoriesButton;
        public GameObject contentCategories; // Parent object where the category buttons will be added.
        public GameObject categoryButtom; // Button that will be used to create different instances, each button will be a category.
        public GameObject contentAnswers; // Parent object where the answer buttons will be added.
        public GameObject answerButtom; // Button that will be used to create different instances, each button will be a answer.
        public GameObject questionImage; // Main image of a question.
        public GameObject questionName; // Object where the question will be added.
        public Text scoreEarned; // Score earned.
        public Text scoreGame; // Total score. 
        public Text scoreGameOver; // Final score obtained in each category.
        public Text percentageAnswers; // Percentage of answers that were answered.
        public GameObject[] typesAnswer; // 0 = Simple selection , 1 = True or False
        GameController gameController = null;
        int selectAnswer = 0; // Correct answer.
        bool enableOnClick = false;
        public Button trueButton;
        public Button falseButton;
        bool trueOrFalse = false; // Correct answer, true or false.
        List<Button> answerButtons = null;

        void Awake()
        {
            gameController = GameObject.Find("Scripts").GetComponent<GameController>();
        }

        // Start is called before the first frame update
        void Start()
        {
            EnableMenu(GlobalDemoVariables.mainMenuID);
            SetUpButtons();
            CreateButtonsForTags();
        }

        // Enable menus.
        public void EnableMenu(int idMenu)
        {
            switch (idMenu)
            {
                case GlobalDemoVariables.mainMenuID: // Main menu. 
                    mainMenu.SetActive(true);
                    categoriesMenu.SetActive(false);
                    levelMenu.SetActive(false);
                    gameOverMenu.SetActive(false);
                    break;
                case GlobalDemoVariables.categoriesMenuID: // Categories menu or tags menu.
                    mainMenu.SetActive(false);
                    categoriesMenu.SetActive(true);
                    levelMenu.SetActive(false);
                    gameOverMenu.SetActive(false);
                    break;
                case GlobalDemoVariables.levelMenuID: // Level menu.
                    mainMenu.SetActive(false);
                    categoriesMenu.SetActive(false);
                    levelMenu.SetActive(true);
                    gameOverMenu.SetActive(false);
                    break;
                case GlobalDemoVariables.gameOverMenuID: // Game over menu.
                    mainMenu.SetActive(false);
                    categoriesMenu.SetActive(false);
                    levelMenu.SetActive(false);
                    gameOverMenu.SetActive(true);
                    break;
                default:
                    mainMenu.SetActive(true);
                    categoriesMenu.SetActive(false);
                    levelMenu.SetActive(false);
                    break;
            }
        }

        // Enable answer type.
        // int idType: 0 = simple selection , 1 = true or false.
        public void EnableTypesAnswer(int idType, OnlyOneQuestion question)
        {
            RemoveScrollViewButtons(contentAnswers);
            answerButtom.SetActive(false);
            Text nameText = questionName.GetComponentInChildren<Text>();
            nameText.text = question.questionName; // Question.
            Texture2D texture2d = null;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
                texture2d = GetTexture2DFromStreamingFolder (GlobalDemoVariables.imagesFolderPC, question.imageName, -2);
#else
#if UNITY_ANDROID
                    texture2d = GetTexture2DFromStreamingFolder (GlobalDemoVariables.imagesFolderAndroid, question.imageName, -1);
#endif
#endif
            if (texture2d != null)
            {
                Image image = questionImage.GetComponent<Image>();
                image.type = Image.Type.Simple;
                image.preserveAspect = true;
                image.sprite = Sprite.Create(
                    texture2d, new Rect(0.0f, 0.0f, texture2d.width, texture2d.height), new Vector2(0.5f, 0.5f), 100.0f);
                questionImage.SetActive(true);
            }
            else
            {
                Image image = questionImage.GetComponent<Image>();
                image.sprite = null;
                questionImage.SetActive(false);
            }
            switch (idType)
            {
                case GlobalDemoVariables.singleTypeAnswerID: // Simple selection.
                    typesAnswer[0].SetActive(true);
                    typesAnswer[1].SetActive(false);
                    answerButtons = new List<Button>();
                    AllAnswers allAnswers = question.allAnswers;
                    // Create dynamic buttons, each button represents a answer.
                    for (int index = 0; index < allAnswers.answers.Count; index++)
                    {
                        GameObject objectInstantiate = Instantiate(
                            answerButtom, answerButtom.transform.position, answerButtom.transform.rotation) as GameObject;
                        if (objectInstantiate != null)
                        {
                            objectInstantiate.transform.localScale = transform.root.localScale;
                            objectInstantiate.transform.SetParent(contentAnswers.transform);
                            Button button = objectInstantiate.GetComponent<Button>();
                            answerButtons.Add(button);
                            int countButton = index + 1;
                            button.name = countButton.ToString() + "- Answer";
                            button.gameObject.SetActive(true);
                            int indexAnswer = index;
                            button.onClick.AddListener(delegate { SelectAnswerOnClick(indexAnswer); });
                            if (allAnswers.textOrImage[index] == GlobalDemoVariables.idTextSelection)
                            {
                                Text text = button.GetComponentInChildren<Text>();
                                text.text = allAnswers.answers[index];
                            }
                            if (allAnswers.textOrImage[index] == GlobalDemoVariables.idImageSelection)
                            {
                                Texture2D questionTexture2d = null;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
                                questionTexture2d = GetTexture2DFromStreamingFolder 
                                    (GlobalDemoVariables.imagesFolderPC, allAnswers.answers [index], -2);
#else
#if UNITY_ANDROID
                                    questionTexture2d = GetTexture2DFromStreamingFolder 
                                        (GlobalDemoVariables.imagesFolderAndroid, allAnswers.answers [index], index);
#endif
#endif
                                if (questionTexture2d != null)
                                {
                                    Image buttonImage = button.GetComponent<Image>();
                                    buttonImage.type = Image.Type.Simple;
                                    buttonImage.preserveAspect = true;
                                    buttonImage.sprite = Sprite.Create(
                                        questionTexture2d, new Rect(
                                        0.0f, 0.0f, questionTexture2d.width, questionTexture2d.height), new Vector2(0.5f, 0.5f), 100.0f);
                                    Text text = button.GetComponentInChildren<Text>();
                                    text.text = "";
                                }
                            }
                        }
                        // What is the correct answer is verified.
                        if (question.allAnswers.selectedAnswers[index])
                        {
                            selectAnswer = index;
                        }
                    }
                    break;
                case GlobalDemoVariables.trueOrFalseTypeAnswerID: // True or False.
                                                                  // Set the true and false buttons.
                    typesAnswer[0].SetActive(false);
                    typesAnswer[1].SetActive(true);
                    trueOrFalse = question.trueOrFalse;
                    Image image = trueButton.GetComponentInChildren<Image>();
                    Color color = image.color;
                    color.a = 1;
                    image.color = color;
                    Text textButton = trueButton.GetComponentInChildren<Text>();
                    Color colorTxt = textButton.color;
                    colorTxt.a = 1;
                    textButton.color = colorTxt;
                    image = falseButton.GetComponentInChildren<Image>();
                    color = image.color;
                    color.a = 1;
                    image.color = color;
                    textButton = falseButton.GetComponentInChildren<Text>();
                    colorTxt = textButton.color;
                    colorTxt.a = 1;
                    textButton.color = colorTxt;
                    break;
                default:
                    typesAnswer[0].SetActive(true);
                    typesAnswer[1].SetActive(false);
                    break;
            }
        }

        void SetUpButtons()
        {
            // Play button.
            var playButtonObject = playButton.GetComponent<Button>();
            playButtonObject.onClick.AddListener(EnableSelectCategoryMenuOnClick);
            // Home button.
            for (int index = 0; index < homeButtons.Length; index++)
            {
                var homeButtonObject = homeButtons[index].GetComponent<Button>();
                homeButtonObject.onClick.AddListener(EnableSelectCategoryMenuOnClick);
                homeButtonObject.onClick.AddListener(EnableMainMenuOnClick);
            }
            // Back button.
            var categoriesButtonObject = categoriesButton.GetComponent<Button>();
            categoriesButtonObject.onClick.AddListener(EnableSelectCategoryMenuOnClick);
            trueButton.onClick.AddListener(delegate { TrueFalseOnClick(true); });
            falseButton.onClick.AddListener(delegate { TrueFalseOnClick(false); });
        }

        // Select an answer by clicking on the dynamic buttons.
        void SelectAnswerOnClick(int indexAnswer)
        {
            if (enableOnClick)
            {
                return;
            }
            Button[] button = contentAnswers.GetComponentsInChildren<Button>();
            if (button != null)
            {
                if (button.Length > 0)
                {
                    for (int index = 0; index < button.Length; index++)
                    {
                        Image image = button[index].GetComponentInChildren<Image>();
                        Color color = image.color;
                        color.a = 0;
                        image.color = color;
                        Text text = button[index].GetComponentInChildren<Text>();
                        Color colorTxt = text.color;
                        colorTxt.a = 0;
                        text.color = colorTxt;
                    }
                    if (selectAnswer == indexAnswer)
                    { // Correct answer.
                        Image image = button[indexAnswer].GetComponentInChildren<Image>();
                        Color color = image.color;
                        color.a = 1;
                        image.color = color;
                        Text text = button[indexAnswer].GetComponentInChildren<Text>();
                        Color colorTxt = text.color;
                        colorTxt.a = 1;
                        text.color = colorTxt;
                        gameController.correctAnswers++;
                        int score = gameController.correctAnswers * GlobalDemoVariables.score;
                        SetScoreEarned(score.ToString());
                    }
                    else
                    { // Wrong answer.
                        Image image = button[selectAnswer].GetComponentInChildren<Image>();
                        Color color = image.color;
                        color.a = 1;
                        image.color = color;
                        Text text = button[selectAnswer].GetComponentInChildren<Text>();
                        Color colorTxt = text.color;
                        colorTxt.a = 1;
                        text.color = colorTxt;
                    }
                    StartCoroutine("CoroutineLoadNextQuestion");
                }
            }
        }

        // Select the correct answer.
        void TrueFalseOnClick(bool answer)
        {
            if (enableOnClick)
            {
                return;
            }
            Image image = trueButton.GetComponentInChildren<Image>();
            Color color = image.color;
            color.a = 0;
            image.color = color;
            Text text = trueButton.GetComponentInChildren<Text>();
            Color colorTxt = text.color;
            colorTxt.a = 0;
            text.color = colorTxt;
            image = falseButton.GetComponentInChildren<Image>();
            color = image.color;
            color.a = 0;
            image.color = color;
            text = falseButton.GetComponentInChildren<Text>();
            colorTxt = text.color;
            colorTxt.a = 0;
            text.color = colorTxt;
            if (trueOrFalse == answer)
            { // Correct answer.
                Button button = null;
                if (answer)
                {
                    button = trueButton;
                }
                else
                {
                    button = falseButton;
                }
                image = button.GetComponentInChildren<Image>();
                color = image.color;
                color.a = 1;
                image.color = color;
                text = button.GetComponentInChildren<Text>();
                colorTxt = text.color;
                colorTxt.a = 1;
                text.color = colorTxt;
                gameController.correctAnswers++;
                int score = gameController.correctAnswers * GlobalDemoVariables.score;
                SetScoreEarned(score.ToString());
            }
            else
            { // Wrong answer.
                if (trueOrFalse)
                {
                    image = trueButton.GetComponentInChildren<Image>();
                    color = image.color;
                    color.a = 1;
                    image.color = color;
                    text = trueButton.GetComponentInChildren<Text>();
                    colorTxt = text.color;
                    colorTxt.a = 1;
                    text.color = colorTxt;
                }
                else
                {
                    image = falseButton.GetComponentInChildren<Image>();
                    color = image.color;
                    color.a = 1;
                    image.color = color;
                    text = falseButton.GetComponentInChildren<Text>();
                    colorTxt = text.color;
                    colorTxt.a = 1;
                    text.color = colorTxt;
                }
            }
            StartCoroutine("CoroutineLoadNextQuestion");
        }

        // Coroutine, create the next question after 3 seconds.
        IEnumerator CoroutineLoadNextQuestion()
        {
            enableOnClick = true;
            yield return new WaitForSeconds(3.0f);
            gameController.NextQuestion();
            enableOnClick = false;
        }

        // Enable category menu.
        void EnableSelectCategoryMenuOnClick()
        {
            StopCoroutine("CoroutineLoadNextQuestion");
            enableOnClick = false;
            EnableMenu(GlobalDemoVariables.categoriesMenuID);
        }

        // Enable level menu.
        void EnableLevelMenuOnClick(string tag)
        {
            StopCoroutine("CoroutineLoadNextQuestion");
            enableOnClick = false;
            SetScoreEarned("0");
            gameController.SelectedTag(tag);
            EnableMenu(GlobalDemoVariables.levelMenuID);
        }

        // Enable main menu.
        void EnableMainMenuOnClick()
        {
            EnableMenu(GlobalDemoVariables.mainMenuID);
        }

        // Create dynamic buttons, each button represents a category.
        void CreateButtonsForTags()
        {
            categoryButtom.SetActive(false);
            if (gameController == null)
            {
                return;
            }
            // Get tags list.
            List<string> tags = gameController.GetTagsFromGameController();
            if (tags == null || tags.Count == 0)
            {
                return;
            }
            for (int index = 0; index < tags.Count; index++)
            {
                GameObject objectInstantiate = Instantiate(
                    categoryButtom, categoryButtom.transform.position, categoryButtom.transform.rotation) as GameObject;
                if (objectInstantiate != null)
                {
                    objectInstantiate.transform.localScale = transform.root.localScale;
                    objectInstantiate.transform.SetParent(contentCategories.transform);
                    Button button = objectInstantiate.GetComponent<Button>();
                    button.gameObject.SetActive(true);
                    string tag = tags[index];
                    button.onClick.AddListener(delegate { EnableLevelMenuOnClick(tag); });
                    Text text = button.GetComponentInChildren<Text>();
                    text.text = tags[index];
                }
            }
        }

        // Delete buttons.
        void RemoveScrollViewButtons(GameObject content)
        {
            Button[] buttons = content.GetComponentsInChildren<Button>();
            if (buttons != null)
            {
                for (int index = 0; index < buttons.Length; index++)
                {
                    Destroy(buttons[index].gameObject);
                }
            }
        }

        // The total number of questions answered is displayed.
        public void SetScore(string score)
        {
            scoreGame.text = score;
        }

        // The score obtained is shown.
        public void SetScoreGameOver(string score)
        {
            scoreGameOver.text = score;
        }

        // The percentage of questions answered is displayed.
        public void SetPercentageGameOver(string percentage)
        {
            percentageAnswers.text = percentage;
        }

        // The total number of questions and the number of questions answered are displayed.
        void SetScoreEarned(string score)
        {
            scoreEarned.text = score;
        }

        // Get image texture.
        // string imagesFolder = folder where the images are located.
        // string nameFile = image name.
        // int index = position of the image in the list (only Android).
        Texture2D GetTexture2DFromStreamingFolder(string imagesFolder, string nameFile, int index)
        {
            if (imagesFolder == null || nameFile == null)
            {
                return null;
            }
            string filePath = Path.Combine(imagesFolder, nameFile);
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
                return GetTexture2DFromStreamingFolderPC (filePath);
#else
#if UNITY_ANDROID
                    StartCoroutine (GetTexture2DFromStreamingFolderAndroid (filePath, index));
                    return null;
#else
            return null;
#endif
#endif
        }

        // Get image texture from Editor, Window and Linux.
        // string filePath = image path.
        Texture2D GetTexture2DFromStreamingFolderPC(string filePath)
        {
            string pathTexture = Application.dataPath + filePath;
            Texture2D texture2D = null;
            if (!File.Exists(pathTexture))
            {
                return null;
            }
            var imgBytes = File.ReadAllBytes(pathTexture);
            texture2D = new Texture2D(2, 2);
            texture2D.LoadImage(imgBytes);
            return texture2D;
        }

        // Get image texture from Android.
        // string filePath = image path.
        // int index = position of the image in the list.
        IEnumerator GetTexture2DFromStreamingFolderAndroid(string filePath, int index)
        {
            string filePathOnAndroid = Application.streamingAssetsPath + filePath;
            Texture2D texture2D = null;
            using (UnityWebRequest wReq = UnityWebRequestTexture.GetTexture(filePathOnAndroid))
            {
                yield return wReq.SendWebRequest();
                if (wReq.result == UnityWebRequest.Result.ConnectionError || wReq.result == UnityWebRequest.Result.ProtocolError)
                {
                    string error = wReq.error;
                    texture2D = null;
                }
                else
                {
                    texture2D = DownloadHandlerTexture.GetContent(wReq);
                    SetTextureOnSpriteAsynchronousTask(texture2D, index);
                }
            }
        }

        // Add texture asynchronously in question and answer sprites.
        // Texture2D texture2d = sprite texture.
        // int index = position of the texture in the list. 
        void SetTextureOnSpriteAsynchronousTask(Texture2D texture2d, int index)
        {
            if (texture2d == null)
            {
                return;
            }
            if (index == -1)
            { // Question image.
                Image image = questionImage.GetComponent<Image>();
                image.type = Image.Type.Simple;
                image.preserveAspect = true;
                image.sprite = Sprite.Create(
                    texture2d, new Rect(0.0f, 0.0f, texture2d.width, texture2d.height), new Vector2(0.5f, 0.5f), 100.0f);
                questionImage.SetActive(true);
            }
            else
            { // Buttons.
                if (answerButtons == null)
                {
                    return;
                }
                if (answerButtons.Count == 0)
                {
                    return;
                }
                Button button = answerButtons[index];
                if (button != null)
                {
                    Image buttonImage = button.GetComponent<Image>();
                    buttonImage.type = Image.Type.Simple;
                    buttonImage.preserveAspect = true;
                    buttonImage.sprite = Sprite.Create(
                        texture2d, new Rect(
                            0.0f, 0.0f, texture2d.width, texture2d.height), new Vector2(0.5f, 0.5f), 100.0f);
                    Text text = button.GetComponentInChildren<Text>();
                    text.text = "";
                }
            }
        }
    }
}