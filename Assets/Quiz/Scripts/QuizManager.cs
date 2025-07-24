using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class QuizManager : MonoBehaviour
{
#pragma warning disable 649
    [SerializeField] private QuizGameUI quizGameUI;
    [SerializeField] private float defaultTimeInSeconds = 30f; // Tempo padrão global
#pragma warning restore 649

    private string currentCategory = "";
    private int correctAnswerCount = 0;
    private List<Question> questions;
    private Question selectedQuestion = new Question();
    private int gameScore;
    private int lifesRemaining;
    private float currentTime;
    private float questionStartTime; // Tempo inicial da pergunta atual
    private GameStatus gameStatus = GameStatus.NEXT;

    // Armazena a fase atualmente carregada (para jogar uma única fase)
    private QuizData loadedQuizData;
    public QuizData QuizData { get { return loadedQuizData; } }

    // Nova propriedade para armazenar todas as fases disponíveis
    public List<QuizData> AllQuizDatas { get; private set; } = new List<QuizData>();

    public GameStatus GameStatus { get { return gameStatus; } }

    void Awake()
    {
        LoadAllQuizDatas();
    }

    // Carrega todos os arquivos JSON da pasta "Quizzes"
    private void LoadAllQuizDatas()
    {
        AllQuizDatas.Clear();
        // Carrega todos os TextAsset (JSON) da pasta "Quizzes" dentro de Resources
        TextAsset[] quizFiles = Resources.LoadAll<TextAsset>("Quizzes");
        if (quizFiles.Length == 0)
        {
            Debug.LogWarning("Nenhum arquivo JSON encontrado na pasta Resources/Quizzes");
            return;
        }
        foreach (TextAsset quizFile in quizFiles)
        {
            QuizData quiz = JsonUtility.FromJson<QuizData>(quizFile.text);
            if (quiz != null)
            {
                // Aplica o tempo padrão da categoria para perguntas que não têm tempo definido
                foreach (var question in quiz.questions)
                {
                    if (question.questionTime <= 0)
                    {
                        question.questionTime = quiz.defaultQuestionTime > 0 ? quiz.defaultQuestionTime : defaultTimeInSeconds;
                    }
                }
                AllQuizDatas.Add(quiz);
            }
            else
            {
                Debug.LogWarning("Falha ao carregar o quiz: " + quizFile.name);
            }
        }
    }

    // Exemplo: para iniciar um jogo a partir de um índice de fase (categoria) escolhido
    public void StartGame(int index, string category)
    {
        if (index < 0 || index >= AllQuizDatas.Count)
        {
            Debug.LogError("Índice de fase inválido.");
            return;
        }
        QuizData data = AllQuizDatas[index];
        InitializeQuiz(data);
    }

    void InitializeQuiz(QuizData data)
    {
        loadedQuizData = data;
        currentCategory = data.categoryName;
        correctAnswerCount = 0;
        gameScore = 0;
        lifesRemaining = 3;
        questions = new List<Question>(data.questions);

        if (questions.Count == 0)
        {
            Debug.LogWarning("QuizManager: Nenhuma pergunta encontrada para esta fase.");
            return;
        }

        SelectQuestion();
        gameStatus = GameStatus.PLAYING;
    }

    private void SelectQuestion()
    {
        if (questions == null || questions.Count == 0)
        {
            Debug.LogWarning("QuizManager: Nenhuma pergunta disponível para seleção.");
            return;
        }
        int val = UnityEngine.Random.Range(0, questions.Count);
        selectedQuestion = questions[val];

        // Define o tempo da pergunta atual
        currentTime = selectedQuestion.questionTime;
        questionStartTime = currentTime;

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
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        quizGameUI.TimerText.text = time.ToString("mm':'ss");
        if (currentTime <= 0)
        {
            // Tempo esgotado - trata como resposta incorreta
            lifesRemaining--;
            quizGameUI.ReduceLife(lifesRemaining);
            if (lifesRemaining == 0)
            {
                GameEnd();
            }
            else if (questions.Count > 0)
            {
                Invoke("SelectQuestion", 0.4f);
            }
            else
            {
                GameEnd();
            }
        }
    }

    public bool Answer(string selectedOption)
    {
        bool correct = false;
        if (selectedQuestion.options[selectedQuestion.correctAnswerIndex] == selectedOption)
        {
            correctAnswerCount++;
            correct = true;
            gameScore += 50;
            quizGameUI.ScoreText.text = "Score:" + gameScore;
        }
        else
        {
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
                Invoke("SelectQuestion", 0.4f);
            else
                GameEnd();
        }
        return correct;
    }

    private void GameEnd()
    {
        gameStatus = GameStatus.NEXT;
        quizGameUI.GameOverPanel.SetActive(true);
        PlayerPrefs.SetInt(currentCategory, correctAnswerCount);
    }

    // NOVO: Método público para recarregar os quizzes
    public void ReloadAllQuizzes()
    {
        LoadAllQuizDatas();
    }

    // NOVO: Método público para obter a lista de quizzes
    public void RefreshQuizList()
    {
        LoadAllQuizDatas();

        // Notifica o QuizGameUI para atualizar os botões
        if (quizGameUI != null)
        {
            quizGameUI.RefreshCategoryButtons();
        }
    }
}

public enum GameStatus
{
    NEXT,
    PLAYING,
    GAMEOVER
}