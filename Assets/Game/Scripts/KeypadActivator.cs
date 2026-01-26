using UnityEngine;
using UnityEngine.Events;

public class KeypadActivator : MonoBehaviour, Interactable
{
    [SerializeField] private GameObject keypadUI; // Assign your keypad UI GameObject in the inspector
    [SerializeField] private string puzzleCode = "1234"; // Set the code for this puzzle
    [SerializeField] private UnityEvent onCorrectCode;    // Set the event for this puzzle

    public void Interact()
    {
        var keypad = keypadUI.GetComponent<Keypad>();
        if (keypad != null)
        {
            keypad.SetCode(puzzleCode, onCorrectCode);
            keypadUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
