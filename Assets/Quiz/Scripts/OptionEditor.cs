// OptionEditor.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionEditor : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField optionTextInput;
    [SerializeField] private Toggle correctToggle;
    [SerializeField] private Button removeBtn;

    private Question question;
    private int optionIndex;
    private QuestionEditor questionEditor;

    public void Initialize(Question q, int index, QuestionEditor qEditor)
    {
        question = q;
        optionIndex = index;
        questionEditor = qEditor;
        
        SetupUI();
        SetupEvents();
    }

    private void SetupUI()
    {
        optionTextInput.text = question.options[optionIndex];
        correctToggle.isOn = question.correctAnswerIndex == optionIndex;
    }

    private void SetupEvents()
    {
        optionTextInput.onValueChanged.AddListener(OnTextChanged);
        correctToggle.onValueChanged.AddListener(OnCorrectToggleChanged);
        removeBtn.onClick.AddListener(RemoveOption);
    }

    private void OnTextChanged(string value)
    {
        if (optionIndex < question.options.Count)
        {
            question.options[optionIndex] = value;
        }
    }

    private void OnCorrectToggleChanged(bool isOn)
    {
        if (isOn)
        {
            questionEditor.SetCorrectAnswer(optionIndex);
        }
    }

    public void SetIndex(int newIndex)
    {
        optionIndex = newIndex;
        correctToggle.isOn = question.correctAnswerIndex == optionIndex;
    }

    public void UpdateCorrectToggle(bool isCorrect)
    {
        correctToggle.SetIsOnWithoutNotify(isCorrect);
    }

    private void RemoveOption()
    {
        if (question.options.Count > 2) // Manter pelo menos 2 opções
        {
            questionEditor.RemoveOption(this);
        }
    }
}