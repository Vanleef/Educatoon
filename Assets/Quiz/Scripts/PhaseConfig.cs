using System.Collections.Generic;

[System.Serializable]
public class PhaseConfig
{
    public string name; // <-- Adicione esta linha
    public QuizType quizType = QuizType.Texto;
    public List<QuestionConfig> questions = new List<QuestionConfig>();
}