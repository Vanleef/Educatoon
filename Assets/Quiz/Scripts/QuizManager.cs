using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizManager : MonoBehaviour
{
#pragma warning disable 649
    // Reference to the QuizGameUI script
    [SerializeField] private QuizGameUI quizGameUI;
    // Reference to the scriptableobject file
    [SerializeField] private List<QuizDataScriptable> quizDataList;
    [SerializeField] private float timeInSeconds;
#pragma warning restore 649

    private string currentCategory = "";
    private int correctAnswerCount = 0;
    // Questions data
    private List<Question> questions;
    // Current question data
    private Question selectedQuestion = new Question();
    private int gameScore;
    private int lifesRemaining;
    private float currentTime;
    private QuizDataScriptable dataScriptable;

    private GameStatus gameStatus = GameStatus.NEXT;

    public GameStatus GameStatus { get { return gameStatus; } }

    public List<QuizDataScriptable> QuizData { get => quizDataList; }

    public void StartGame(int categoryIndex, string category)
    {
        if (quizGameUI == null)
        {
            Debug.LogError("QuizManager: quizGameUI reference is not set.");
            return;
        }
        if (quizDataList == null || categoryIndex < 0 || categoryIndex >= quizDataList.Count)
        {
            Debug.LogError("QuizManager: Invalid category index or quizDataList is not set.");
            return;
        }

        currentCategory = category;
        correctAnswerCount = 0;
        gameScore = 0;
        lifesRemaining = 3;
        currentTime = timeInSeconds;
        // Set the questions data
        questions = new List<Question>();
        dataScriptable = quizDataList[categoryIndex];
        questions.AddRange(dataScriptable.questions);

        if (questions.Count == 0)
        {
            Debug.LogWarning("QuizManager: No questions found for this category.");
            return;
        }

        // Select the question
        SelectQuestion();
        gameStatus = GameStatus.PLAYING;
    }

    /// <summary>
    /// Method used to randomly select the question from questions data
    /// </summary>
    private void SelectQuestion()
    {
        if (questions == null || questions.Count == 0)
        {
            Debug.LogWarning("QuizManager: No questions available to select.");
            return;
        }
        // Get the random number
        int val = UnityEngine.Random.Range(0, questions.Count);
        // Set the selectedQuestion
        selectedQuestion = questions[val];
        // Send the question to quizGameUI
        quizGameUI.SetQuestion(selectedQuestion);

        questions.RemoveAt(val);
    }

    private void Update()
    {
        if (gameStatus == GameStatus.PLAYING)
        {
            currentTime -= Time.deltaTime;
            SetTime(currentTime);
        }
    }

    void SetTime(float value)
    {
        TimeSpan time = TimeSpan.FromSeconds(currentTime); // Set the time value
        quizGameUI.TimerText.text = time.ToString("mm':'ss"); // Convert time to Time format

        if (currentTime <= 0)
        {
            // Game Over
            GameEnd();
        }
    }

    /// <summary>
    /// Method called to check if the answer is correct or not
    /// </summary>
    /// <param name="selectedOption">answer string</param>
    /// <returns></returns>
    public bool Answer(string selectedOption)
    {
        // Set default to false
        bool correct = false;
        // If selected answer is similar to the correctAns
        if (selectedQuestion.correctAns == selectedOption)
        {
            // Yes, Ans is correct
            correctAnswerCount++;
            correct = true;
            gameScore += 50;
            quizGameUI.ScoreText.text = "Score:" + gameScore;
        }
        else
        {
            // No, Ans is wrong
            // Reduce Life
            lifesRemaining--;
            quizGameUI.ReduceLife(lifesRemaining);

            if (lifesRemaining == 0)
            {
                GameEnd();
            }
        }

        if (gameStatus == GameStatus.PLAYING)
        {
            if (questions.Count > 0)
            {
                // Call SelectQuestion method again after 0.4s
                Invoke("SelectQuestion", 0.4f);
            }
            else
            {
                GameEnd();
            }
        }
        // Return the value of correct bool
        return correct;
    }

    private void GameEnd()
    {
        gameStatus = GameStatus.NEXT;
        quizGameUI.GameOverPanel.SetActive(true);

        // If you want to save only the highest score then compare the current score with saved score and if more save the new score
        // eg:- if correctAnswerCount > PlayerPrefs.GetInt(currentCategory) then call below line

        // Save the score
        PlayerPrefs.SetInt(currentCategory, correctAnswerCount); // Save the score for this category
    }
}

// Data structure for storing the questions data
[System.Serializable]
public class Question
{
    public string questionInfo;         // Question text
    public QuestionType questionType;   // Type
    public Sprite questionImage;        // Image for Image Type
    public AudioClip audioClip;         // Audio for audio type
    public UnityEngine.Video.VideoClip videoClip;   // Video for video type
    public List<string> options;        // Options to select
    public string correctAns;           // Correct option
}

[System.Serializable]
public enum QuestionType
{
    TEXT,
    IMAGE,
    AUDIO,
    VIDEO
}

public enum GameStatus
{
    NEXT,       // Next Question
    PLAYING,    // Playing the game
    GAMEOVER    // Game Over
}