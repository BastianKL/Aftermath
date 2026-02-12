using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class RollerCodeManager : MonoBehaviour
{
    public static RollerCodeManager Instance { get; private set; }

    [SerializeField] private List<RollerCylinder> cylinders; // Assign in Inspector
    [SerializeField] private List<int> correctCode; // e.g., [2, 1, 3, 0]
    public UnityEvent onCorrectCode; // Assign in Inspector

    private void Awake()
    {
        Instance = this;
    }

    public void OnCylinderChanged(int index, int number)
    {
        // Only open if all cylinders match the correct code in order
        for (int i = 0; i < correctCode.Count; i++)
        {
            if (cylinders[i].RotationIndex != correctCode[i])
                return;
        }
        // All correct!
        onCorrectCode.Invoke();
    }
}
