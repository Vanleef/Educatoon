using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField answerInput;
    [SerializeField] private Toggle correctToggle;
    [SerializeField] private Button removeBtn;

    private QuestionEditorUI parent;

    public void Init(QuestionEditorUI parentEditor)
    {
        parent = parentEditor;
        removeBtn.onClick.AddListener(() => parent.RemoveAnswer(this));
    }

    public string GetText() => answerInput.text;
    public bool IsCorrect() => correctToggle.isOn;
}