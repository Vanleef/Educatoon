// This file contains several classes that allow you to read the questions and answers from the json file.
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System.IO;
using System;

public class AllKeys
{
    public List<string> keys = null; // Key list.
}

public class AllTags
{
    public List<string> tags = null; // Tags list.
    public List<AllKeys> allKeys = null; // Key list. 

    // Constructor, initialize list.
    public AllTags()
    {
        tags = new List<string>();
        allKeys = new List<AllKeys>();
    }
}

public class AllAnswers
{
    public List<string> answers = null; // The answers to a question can be text or the name of the image.
    public List<int> textOrImage = null; // Type of content of the answers; 0 = text , 1 = image.
    public List<bool> selectedAnswers = null; // Indicate which is the correct answer (true).       
}

public class AllQuestions
{
    public List<string> imageName = null; // Image list.
    public List<string> questionName = null; // Question list.
    public List<string> tags = null; // Tag list.
    public List<int> typeAnswer = null; // Type of answers.
    public List<AllAnswers> allAnswers = null; // Answer list.
    public List<bool> trueOrFalse = null; // Correct answer, true or false.
    public List<string> key = null; // Key list.

    // Constructor, initialize lists.
    public AllQuestions()
    {
        imageName = new List<string>();
        questionName = new List<string>();
        tags = new List<string>();
        typeAnswer = new List<int>();
        allAnswers = new List<AllAnswers>();
        trueOrFalse = new List<bool>();
        key = new List<string>();
    }
}

public class OnlyOneQuestion
{
    public string imageName; // Image file name.
    public string questionName; // Question.
    public string tags; // Tags separated by commas.
    public int typeAnswer; // Type of answers, 0 = Simple Selection = , 1 = True or False.
    public AllAnswers allAnswers; // Answer list.
    public bool trueOrFalse; // Correct answer, true or false.
    public string key; // Question key.
}


[System.Serializable]
public class DeserializeQuestions
{
    public List<QuestionProperties> Questions = null;
}

[System.Serializable]
public class QuestionProperties
{
    public string imageName = null; // Image file name.
    public string questionName = null; // Question name.
    public string tags = null; // Tags separated by commas.
    public int typeAnswer = 0; // Type of answers, 0 = Simple Selection , 1 =  True or False.
    public List<string> answers = null; // Answer list.
    public List<int> textOrImage = null; // The answers can be text or the name of the image.
    public List<bool> selectedAnswers = null; // List of answers selected as correct.
    public bool trueOrFalse = false; // Answer is true or false.
    public string key = null; // Question key.
}

public class JsonManager
{
    // Returns the list of questions and answers from the json file.
    public List<QuestionProperties> ReadQuestionsJson(string jsonFileFolder, string filePathJson)
    {
        if (filePathJson == null || jsonFileFolder == null)
        {
            return null;
        }
        string filePath = Path.Combine(jsonFileFolder, filePathJson);
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX
			return ReadQuestionsJsonPC (filePath);
#else
#if UNITY_ANDROID
				return ReadQuestionsJsonAndroid (filePath);
#else
        return null;
#endif
#endif
    }

    // Returns the list of questions and answers from the json file.
    // Works only on Editor, Windows y Linux.
    List<QuestionProperties> ReadQuestionsJsonPC(string filePath)
    {
        if (filePath == null)
        {
            return null;
        }
        string filePathJson = Application.dataPath + filePath;
        if (!File.Exists(filePathJson))
        {
            return null;
        }
        System.IO.FileInfo fileInfo = null;
        try
        {
            fileInfo = new System.IO.FileInfo(filePathJson);
        }
        catch (Exception e)
        {
            string error = e.Message;
            return null;
        }
        if ((int)fileInfo.Length < 10)
        {
            return null;
        }
        string jsonStr = null;
        try
        {
            jsonStr = File.ReadAllText(filePathJson);
        }
        catch (Exception e)
        {
            string error = e.Message;
            return null;
        }
        if (jsonStr == null)
        {
            return null;
        }
        if (jsonStr.Length < 10)
        {
            return null;
        }
        DeserializeQuestions deserializeQuestions = null;
        try
        {
            deserializeQuestions = JsonUtility.FromJson<DeserializeQuestions>(jsonStr);
        }
        catch (Exception e)
        {
            string error = e.Message;
            return null;
        }
        if (deserializeQuestions == null)
        {
            return null;
        }
        if (deserializeQuestions.Questions == null)
        {
            return null;
        }
        if (deserializeQuestions.Questions.Count == 0)
        {
            return null;
        }
        return deserializeQuestions.Questions;
    }

    // Returns the list of questions and answers from the json file.
    // Works only on Android.
    List<QuestionProperties> ReadQuestionsJsonAndroid(string filePathJson)
    {
        if (filePathJson == null)
        {
            return null;
        }
        string filePathJsonOnAndroid = Application.streamingAssetsPath + filePathJson;
        string jsonStr = null;
        UnityWebRequest www = UnityWebRequest.Get(filePathJsonOnAndroid);
        UnityWebRequestAsyncOperation request = www.SendWebRequest();
        while (!request.isDone)
        {
        }
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            jsonStr = null;
        }
        else
        {
            jsonStr = www.downloadHandler.text;
            if (jsonStr != null)
            {
                jsonStr = www.downloadHandler.text;
            }
            else
            {
                jsonStr = null;
            }
        }
        if (jsonStr == null)
        {
            return null;
        }
        if (jsonStr.Length < 10)
        {
            return null;
        }
        DeserializeQuestions deserializeQuestions = null;
        try
        {
            deserializeQuestions = JsonUtility.FromJson<DeserializeQuestions>(jsonStr);
        }
        catch (Exception e)
        {
            string error = e.Message;
            return null;
        }
        if (deserializeQuestions == null)
        {
            return null;
        }
        if (deserializeQuestions.Questions == null)
        {
            return null;
        }
        if (deserializeQuestions.Questions.Count == 0)
        {
            return null;
        }
        return deserializeQuestions.Questions;
    }
}

// Main class allows reading the questions and answers from the json file.
public class QuestionsFromJson
{
    AllQuestions allQuestions = null;
    AllTags allTags = null;

    // Constructor, read questions and tags from the json file.
    // string jsonFileFolder = json file folder.
    // string jsonFilePath = json file name.
    public QuestionsFromJson(string jsonFileFolder, string jsonFilePath)
    {
        GetQuestionsFromJson(jsonFileFolder, jsonFilePath);
        GetTagsFromQuestions();
    }

    // Returns the list of questions and answers from the json file.
    void GetQuestionsFromJson(string jsonFileFolder, string jsonFilePath)
    {
        JsonManager jsonManager = new JsonManager();
        List<QuestionProperties> questionProperties = jsonManager.ReadQuestionsJson(jsonFileFolder, jsonFilePath);
        if (questionProperties != null)
        {
            allQuestions = new AllQuestions();
            allTags = new AllTags();
            foreach (QuestionProperties question in questionProperties)
            {
                if (question != null)
                {
                    allQuestions.imageName.Add(question.imageName);
                    allQuestions.questionName.Add(question.questionName);
                    allQuestions.tags.Add(question.tags);
                    allQuestions.typeAnswer.Add(question.typeAnswer);
                    AllAnswers allAnswers = new AllAnswers();
                    allAnswers.answers = new List<string>();
                    foreach (string answer in question.answers)
                    {
                        allAnswers.answers.Add(answer);
                    }
                    allAnswers.textOrImage = new List<int>();
                    foreach (int textOrImage in question.textOrImage)
                    {
                        allAnswers.textOrImage.Add(textOrImage);
                    }
                    allAnswers.selectedAnswers = new List<bool>();
                    foreach (bool selectedAnswers in question.selectedAnswers)
                    {
                        allAnswers.selectedAnswers.Add(selectedAnswers);
                    }
                    allQuestions.allAnswers.Add(allAnswers);
                    allQuestions.trueOrFalse.Add(question.trueOrFalse);
                    allQuestions.key.Add(question.key);
                }
            }
        }
    }

    // Read tags.
    void GetTagsFromQuestions()
    {
        if (allQuestions != null)
        {
            if (allQuestions.tags != null)
            {
                int counter = 0;
                foreach (string tag in allQuestions.tags)
                {
                    string[] tags = tag.Split(',');
                    if (tags.Length > 0)
                    {
                        for (int index = 0; index < tags.Length; index++)
                        {
                            AddTags(tags[index].Trim(), allQuestions.key[counter]);
                        }
                    }
                    else
                    {
                        AddTags(tag.Trim(), allQuestions.key[counter]);
                    }
                    counter++;
                }
            }
        }
    }

    // Add tags to list.
    void AddTags(string tag, string key)
    {
        if (allTags != null)
        {
            bool isTag = false;
            for (int index = 0; index < allTags.tags.Count; index++)
            {
                string outTag = allTags.tags[index];
                if (string.Equals(tag, outTag))
                {
                    allTags.allKeys[index].keys.Add(key);
                    isTag = true;
                    break;
                }
            }
            if (!isTag)
            {
                allTags.tags.Add(tag);
                AllKeys allKeys = new AllKeys();
                allKeys.keys = new List<string>();
                allKeys.keys.Add(key);
                allTags.allKeys.Add(allKeys);
            }
        }
    }

    // Get tag list.
    public List<string> GetTags()
    {
        if (allTags == null)
        {
            return null;
        }
        return allTags.tags;
    }

    // The keys of the questions filtered by tag are obtained.
    public List<string> GetQuestionsKeysByTag(string tag)
    {
        List<string> keys = null;
        for (int index = 0; index < allTags.tags.Count; index++)
        {
            if (string.Equals(tag, allTags.tags[index]))
            {
                return allTags.allKeys[index].keys;
            }
        }
        return keys;
    }

    // Return a question.
    // string key = question key.
    public OnlyOneQuestion GetOnlyOneQuestion(string key)
    {
        OnlyOneQuestion question = null;
        if (allQuestions.key.Count > 0)
        {
            question = new OnlyOneQuestion();
            for (int index = 0; index < allQuestions.key.Count; index++)
            {
                if (string.Equals(key, allQuestions.key[index]))
                {
                    question.imageName = allQuestions.imageName[index];
                    question.questionName = allQuestions.questionName[index];
                    question.tags = allQuestions.tags[index];
                    question.typeAnswer = allQuestions.typeAnswer[index];
                    question.allAnswers = allQuestions.allAnswers[index];
                    question.trueOrFalse = allQuestions.trueOrFalse[index];
                    question.key = allQuestions.key[index];
                    return question;
                }
            }
        }
        return question;
    }
}