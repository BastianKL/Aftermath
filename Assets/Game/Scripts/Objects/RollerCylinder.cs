using UnityEngine;

public class RollerCylinder : MonoBehaviour, Interactable
{
    [SerializeField] private int cylinderIndex = 0; // Set by manager or in Inspector
    [SerializeField] private int numberCount = 4;   // How many positions (e.g., 4 for 0-3)
    [SerializeField] private float rotationDuration = 0.3f; // How long to rotate
    [SerializeField] private Transform numberDisplay; // Assign the mesh/child to rotate
    [SerializeField] private int startRotationIndex = 0; // Set in Inspector

    private int rotationIndex = 0;
    private bool isRotating = false;
    private float currentAngle = 0f;
    private Quaternion baseRotation;

    public int RotationIndex => rotationIndex;
    public int CylinderIndex => cylinderIndex;

    private void Awake()
    {
        if (numberDisplay != null)
        {
            baseRotation = numberDisplay.localRotation;
            rotationIndex = startRotationIndex % numberCount;
            currentAngle = 90f * rotationIndex;
            numberDisplay.localRotation = baseRotation * Quaternion.AngleAxis(-currentAngle, Vector3.up);
        }
    }

    public void Interact()
    {
        if (!isRotating)
            StartCoroutine(RotateToNextPosition());
    }

    private System.Collections.IEnumerator RotateToNextPosition()
    {
        isRotating = true;
        rotationIndex = (rotationIndex + 1) % numberCount;

        float startAngle = currentAngle;
        float endAngle = startAngle + 90f;
        float t = 0f;

        while (t < rotationDuration)
        {
            float angle = Mathf.Lerp(startAngle, endAngle, t / rotationDuration);
            if (numberDisplay != null)
                numberDisplay.localRotation = baseRotation * Quaternion.AngleAxis(-angle, Vector3.up);
            t += Time.deltaTime;
            yield return null;
        }
        currentAngle = endAngle % 360f;
        if (numberDisplay != null)
            numberDisplay.localRotation = baseRotation * Quaternion.AngleAxis(-currentAngle, Vector3.up);

        isRotating = false;
        RollerCodeManager.Instance.OnCylinderChanged(cylinderIndex, rotationIndex);
    }

    public void SetRotationIndex(int index)
    {
        rotationIndex = index % numberCount;
        currentAngle = (90f * rotationIndex) % 360f;
        if (numberDisplay != null)
            numberDisplay.localRotation = baseRotation * Quaternion.AngleAxis(-currentAngle, Vector3.up);
    }
}
