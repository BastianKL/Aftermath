using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Keypad : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshPro codeDisplayText;

    [Header("Keypad Settings")]
    [SerializeField] private string correctCode = "1234";
    [SerializeField] private int codeLength = 4;

    [Header("Events")]
    public UnityEvent onCorrectCodeEntered;

    private string _enteredCode = "";

    public void AddDigit(string digit)
    {
        if (_enteredCode.Length < codeLength)
        {
            _enteredCode += digit;
            UpdateDisplay();
        }
    }

    public void Backspace()
    {
        if (_enteredCode.Length > 0)
        {
            _enteredCode = _enteredCode.Substring(0, _enteredCode.Length - 1);
            UpdateDisplay();
        }
    }

    public void CheckCode()
    {
        if (_enteredCode == correctCode)
        {
            onCorrectCodeEntered.Invoke();
        }
        _enteredCode = "";
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        codeDisplayText.text = _enteredCode.PadRight(codeLength, '_');
    }

    public void SetCode(string code, UnityEvent onCorrect)
    {
        correctCode = code;
        onCorrectCodeEntered = onCorrect;
        _enteredCode = "";
        UpdateDisplay();
    }
}
