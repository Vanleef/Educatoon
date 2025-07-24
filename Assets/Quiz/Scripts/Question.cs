using System.Collections.Generic;

// Data structure for storing the questions data
[System.Serializable]
public class Question
{

    public string questionInfo = "";
    public QuestionType questionType = QuestionType.Text;
    public string imagePath = ""; // Caminho do arquivo de imagem
    public string audioPath = ""; // Caminho do arquivo de áudio
    public string videoPath = ""; // Caminho do arquivo de vídeo
    public List<string> options = new List<string>();
    // Remove the old string correctAns and use an integer index
    public int correctAnswerIndex = 0;

    // Nova propriedade para tempo personalizado da pergunta (em segundos)
    [System.NonSerialized]
    public float timeInSeconds = 30f; // Valor padrão de 30 segundos

    // Propriedade que retorna/atribui o caminho de mídia de acordo com o tipo de pergunta.
    public string mediaPath
    {
        get
        {
            switch (questionType)
            {
                case QuestionType.Image:
                    return imagePath;
                case QuestionType.Audio:
                    return audioPath;
                case QuestionType.Video:
                    return videoPath;
                default:
                    return "";
            }
        }
        set
        {
            switch (questionType)
            {
                case QuestionType.Image:
                    imagePath = value;
                    break;
                case QuestionType.Audio:
                    audioPath = value;
                    break;
                case QuestionType.Video:
                    videoPath = value;
                    break;
                default:
                    break;
            }
        }
    }

    // Para serialização JSON
    public float questionTime
    {
        get { return timeInSeconds; }
        set { timeInSeconds = value; }
    }
}

public enum QuestionType
{
    Text,
    Image,
    Audio,
    Video
}