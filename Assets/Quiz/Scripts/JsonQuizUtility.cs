using System.IO;
using UnityEngine;

public static class JsonQuizUtility
{
    // Salva QuizData como JSON
    public static void SaveQuizData(string path, QuizData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    // Carrega QuizData de um arquivo JSON
    public static QuizData LoadQuizData(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("Arquivo JSON não encontrado: " + path);
            return null;
        }
        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<QuizData>(json);
    }
}