using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class RollerCodeManager : MonoBehaviour
{
    public static RollerCodeManager Instance { get; private set; }

    [SerializeField] private List<RollerCylinder> cylinders; 
    [SerializeField] private List<int> correctCode; 
    public UnityEvent onCorrectCode; 

    private void Awake()
    {
        Instance = this;
    }

    public void OnCylinderChanged(int index, int number)
    {
        for (int i = 0; i < correctCode.Count; i++)
        {
            if (cylinders[i].RotationIndex != correctCode[i])
                return;
        }
        onCorrectCode.Invoke();
    }
}
