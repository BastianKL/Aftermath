using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class ButtonPuzzleManager : MonoBehaviour
{
    public static ButtonPuzzleManager Instance { get; private set; }
    [SerializeField] private List<int> correctOrder; // Set in Inspector
    [SerializeField] private List<ButtonInteractable> buttons; // Assign in Inspector
    public UnityEvent onPuzzleSolved; // Assign in Inspector

    private List<int> currentOrder = new List<int>();

    private void Awake()
    {
        Instance = this;
    }

    public void OnButtonPressed(int index)
    {
        currentOrder.Add(index);
        buttons[index].LockButton();

        if (currentOrder.Count == correctOrder.Count)
        {
            if (IsCorrect())
            {
                onPuzzleSolved.Invoke();
                // Optionally, keep buttons locked or add more feedback here
            }
            else
            {
                // Reset all buttons
                foreach (var btn in buttons)
                    btn.ResetButton();
                currentOrder.Clear();
            }
        }
    }

    private bool IsCorrect()
    {
        for (int i = 0; i < correctOrder.Count; i++)
            if (currentOrder[i] != correctOrder[i]) return false;
        return true;
    }
}
