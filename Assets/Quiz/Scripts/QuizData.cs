using System.Collections.Generic;

[System.Serializable]
public class QuizData
{
    public string categoryName;
    public List<Question> questions;

    // Tempo padrão para as perguntas desta categoria (em segundos)
    public float defaultQuestionTime = 30f;
}