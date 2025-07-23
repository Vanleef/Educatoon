using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class QuizGameUI : MonoBehaviour
{
#pragma warning disable 649
    [SerializeField] private QuizManager quizManager;               //ref to the QuizManager script
    [SerializeField] private CategoryBtnScript categoryBtnPrefab;
    [SerializeField] private GameObject scrollHolder;
    [SerializeField] private TMP_Text scoreText, timerText;
    [SerializeField] private List<Image> lifeImageList;
    [SerializeField] private GameObject gameOverPanel, mainMenu, gamePanel;
    [SerializeField] private Color correctCol, wrongCol, normalCol; //color of buttons
    [SerializeField] private Image questionImg;                     //image component to show image
    [SerializeField] private UnityEngine.Video.VideoPlayer questionVideo;   //to show video
    [SerializeField] private AudioSource questionAudio;             //audio source for audio clip
    [SerializeField] private TMP_Text questionInfoText;             //text to show question
    [SerializeField] private List<Button> options;                  //options button reference
#pragma warning restore 649

    private float audioLength;          //store audio length
    private Question question;          //store current question data
    private bool answered = false;      //bool to keep track if answered or not
    private Coroutine audioCoroutine;   //reference to running audio coroutine

    public TMP_Text TimerText { get => timerText; }
    public TMP_Text ScoreText { get => scoreText; }
    public GameObject GameOverPanel { get => gameOverPanel; }

    private void Start()
    {
        //add the listener to all the buttons
        for (int i = 0; i < options.Count; i++)
        {
            Button localBtn = options[i];
            localBtn.onClick.AddListener(() => OnClick(localBtn));
        }

        CreateCategoryButtons();
    }

    /// <summary>
    /// Method which populates the question on the screen
    /// </summary>
    public void SetQuestion(Question question)
    {
        this.question = question;

        // Stop any running audio coroutine
        if (audioCoroutine != null)
        {
            StopCoroutine(audioCoroutine);
            audioCoroutine = null;
        }

        //check for questionType
        switch (question.questionType)
        {
            case QuestionType.TEXT:
                questionImg.transform.parent.gameObject.SetActive(false);   //deactivate image holder
                questionVideo.transform.gameObject.SetActive(false);
                questionAudio.transform.gameObject.SetActive(false);
                break;
            case QuestionType.IMAGE:
                questionImg.transform.parent.gameObject.SetActive(true);    //activate image holder
                questionVideo.transform.gameObject.SetActive(false);        //deactivate questionVideo
                questionImg.transform.gameObject.SetActive(true);           //activate questionImg
                questionAudio.transform.gameObject.SetActive(false);        //deactivate questionAudio

                questionImg.sprite = question.questionImage;                //set the image sprite
                break;
            case QuestionType.AUDIO:
                questionVideo.transform.parent.gameObject.SetActive(true);  //activate image holder
                questionVideo.transform.gameObject.SetActive(false);        //deactivate questionVideo
                questionImg.transform.gameObject.SetActive(false);          //deactivate questionImg
                questionAudio.transform.gameObject.SetActive(true);         //activate questionAudio

                if (question.audioClip != null)
                {
                    audioLength = question.audioClip.length;                    //set audio clip
                    audioCoroutine = StartCoroutine(PlayAudio());
                }
                break;
            case QuestionType.VIDEO:
                questionVideo.transform.parent.gameObject.SetActive(true);  //activate image holder
                questionVideo.transform.gameObject.SetActive(true);         //activate questionVideo
                questionImg.transform.gameObject.SetActive(false);          //deactivate questionImg
                questionAudio.transform.gameObject.SetActive(false);        //deactivate questionAudio

                questionVideo.clip = question.videoClip;                    //set video clip
                questionVideo.Play();                                       //play video
                break;
        }

        questionInfoText.text = question.questionInfo;                      //set the question text

        //shuffle the list of options
        List<string> ansOptions = ShuffleList.ShuffleListItems<string>(question.options);

        //assign options to respective option buttons
        for (int i = 0; i < options.Count; i++)
        {
            if (i < ansOptions.Count)
            {
                options[i].GetComponentInChildren<TMP_Text>().text = ansOptions[i];
                options[i].name = ansOptions[i];    //set the name of button
                options[i].image.color = normalCol; //set color of button to normal
                options[i].interactable = true;
            }
            else
            {
                options[i].gameObject.SetActive(false);
            }
        }

        answered = false;
    }

    public void ReduceLife(int remainingLife)
    {
        if (remainingLife >= 0 && remainingLife < lifeImageList.Count)
        {
            lifeImageList[remainingLife].color = Color.red;
        }
    }

    /// <summary>
    /// IEnumerator to repeat the audio after some time
    /// </summary>
    IEnumerator PlayAudio()
    {
        if (question != null && question.questionType == QuestionType.AUDIO && question.audioClip != null)
        {
            questionAudio.PlayOneShot(question.audioClip);
            yield return new WaitForSeconds(audioLength + 0.5f);
            audioCoroutine = StartCoroutine(PlayAudio());
        }
        else
        {
            yield break;
        }
    }

    /// <summary>
    /// Method assigned to the buttons
    /// </summary>
    void OnClick(Button btn)
    {
        if (quizManager.GameStatus == GameStatus.PLAYING && !answered)
        {
            answered = true;
            bool val = quizManager.Answer(btn.name);

            if (val)
            {
                StartCoroutine(BlinkImg(btn.image));
            }
            else
            {
                btn.image.color = wrongCol;
            }
        }
    }

    /// <summary>
    /// Method to create Category Buttons dynamically
    /// </summary>
    void CreateCategoryButtons()
    {
        for (int i = 0; i < quizManager.QuizData.Count; i++)
        {
            CategoryBtnScript categoryBtn = Instantiate(categoryBtnPrefab, scrollHolder.transform);
            categoryBtn.SetButton(quizManager.QuizData[i].categoryName, quizManager.QuizData[i].questions.Count);
            int index = i;
            categoryBtn.Btn.onClick.AddListener(() => CategoryBtn(index, quizManager.QuizData[index].categoryName));
        }
    }

    //Method called by Category Button
    private void CategoryBtn(int index, string category)
    {
        quizManager.StartGame(index, category);
        mainMenu.SetActive(false);
        gamePanel.SetActive(true);
    }

    //this gives blink effect
    IEnumerator BlinkImg(Image img)
    {
        for (int i = 0; i < 2; i++)
        {
            img.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            img.color = correctCol;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void RestryButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
