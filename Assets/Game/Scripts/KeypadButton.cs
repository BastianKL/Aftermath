using UnityEngine;

public class KeypadButton : MonoBehaviour, Interactable
{
    public enum ButtonType { Digit, Backspace, Submit }
    public ButtonType buttonType;
    public string digitValue; // For digits 0-9

    public Keypad keypad; // Assign in inspector or dynamically

    public void Interact()
    {
        if (keypad == null) return;

        switch (buttonType)
        {
            case ButtonType.Digit:
                keypad.AddDigit(digitValue);
                break;
            case ButtonType.Backspace:
                keypad.Backspace();
                break;
            case ButtonType.Submit:
                keypad.CheckCode();
                break;
        }
    }
}

