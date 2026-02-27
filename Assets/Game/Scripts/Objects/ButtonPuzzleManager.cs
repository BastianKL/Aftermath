using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class ButtonPuzzleManager : MonoBehaviour
{
    public static ButtonPuzzleManager Instance { get; private set; }
    [SerializeField] private List<int> correctOrder; 
    [SerializeField] private List<ButtonInteractable> buttons; 
    public UnityEvent onPuzzleSolved; 

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
            }
            else
            {
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
