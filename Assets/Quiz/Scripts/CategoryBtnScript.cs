using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CategoryBtnScript : MonoBehaviour
{
    [SerializeField] private TMP_Text categoryTitleText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Button btn;

    public Button Btn => btn;

    public void SetButton(string title, int totalQuestion)
    {
        Debug.Log($"SetButton: title={title}, totalQuestion={totalQuestion}, score={PlayerPrefs.GetInt(title, 0)}");
        if (categoryTitleText == null || scoreText == null)
        {
            Debug.LogWarning("CategoryBtnScript: Text references are not set.");
            return;
        }

        categoryTitleText.text = title;
        int score = PlayerPrefs.GetInt(title, 0);
        scoreText.text = $"{score}/{totalQuestion}";
    }
}
