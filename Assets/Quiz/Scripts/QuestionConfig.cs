using System.Collections.Generic;

public enum QuizType { Texto, Imagem, Audio, Video }

[System.Serializable]
public class QuestionConfig
{
    public string questionText = "";
    public QuizType questionType = QuizType.Texto;
    public string mediaPath = "";
    public List<string> answers = new List<string>();
    public int correctAnswerIndex = 0;
}