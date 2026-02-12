using UnityEngine;
using System.Collections.Generic;

public class ButtonPuzzleManager : MonoBehaviour
{
    public static ButtonPuzzleManager Instance { get; private set; }
    [SerializeField] private List<int> correctOrder; // Set in Inspector
    private List<int> currentOrder = new List<int>();

    private void Awake()
    {
        Instance = this;
    }

    public void OnButtonPressed(int index)
    {
        currentOrder.Add(index);
        if (currentOrder.Count == correctOrder.Count)
        {
            if (IsCorrect())
            {
                // Puzzle solved!
            }
            else
            {
                // Reset or give feedback
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
