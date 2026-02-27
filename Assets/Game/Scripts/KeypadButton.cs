using UnityEngine;

public class KeypadButton : MonoBehaviour, Interactable
{
    public enum ButtonType { Digit, Backspace, Submit }
    public ButtonType buttonType;
    public string digitValue;

    public Keypad keypad; 

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