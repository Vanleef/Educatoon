using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string questionText;
        public Sprite image;
        public AudioClip audioClip;
        public VideoClip videoClip;
        public string[] answers;
        public int correctAnswerIndex;
        public string mediaType; // "text", "image", "audio", "video"
    }

    public List<Question> questions;
    public Image imageHolder;
    public AudioSource audioSource;
    public VideoPlayer videoPlayer;
    public TMP_Text questionText;
    public Button[] answerButtons;
    public GameObject feedbackPanel;
    public TMP_Text feedbackText;

    private int currentQuestionIndex = 0;

    void Start()
    {
        LoadQuestion();
    }

    void LoadQuestion()
    {
        if (currentQuestionIndex >= questions.Count)
        {
            Debug.Log("Quiz finished");
            return;
        }

        Question q = questions[currentQuestionIndex];

        // Reset all media
        imageHolder.gameObject.SetActive(false);
        audioSource.Stop();
        videoPlayer.gameObject.SetActive(false);
        questionText.text = "";

        // Show appropriate media
        if (q.mediaType == "text")
        {
            questionText.text = q.questionText;
        }
        else if (q.mediaType == "image")
        {
            imageHolder.gameObject.SetActive(true);
            imageHolder.sprite = q.image;
        }
        else if (q.mediaType == "audio")
        {
            audioSource.clip = q.audioClip;
            audioSource.Play();
        }
        else if (q.mediaType == "video")
        {
            videoPlayer.gameObject.SetActive(true);
            videoPlayer.clip = q.videoClip;
            videoPlayer.Play();
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<TMP_Text>().text = q.answers[i];
            int index = i;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => Answer(index));
        }
    }

    public void Answer(int index)
    {
        bool isCorrect = index == questions[currentQuestionIndex].correctAnswerIndex;
        ShowFeedback(isCorrect);
        currentQuestionIndex++;
        Invoke("LoadQuestion", 2f);
    }

    void ShowFeedback(bool correct)
    {
        feedbackPanel.SetActive(true);
        feedbackText.text = correct ? "Correto!" : "Incorreto.";
        Invoke("HideFeedback", 1.5f);
    }

    void HideFeedback()
    {
        feedbackPanel.SetActive(false);
    }
}