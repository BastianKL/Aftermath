using UnityEngine;

public class ButtonInteractable : MonoBehaviour, Interactable
{
    [SerializeField] private Vector3 pressedOffset = new Vector3(0, -0.1f, 0);
    [SerializeField] private float pressDuration = 0.15f;
    [SerializeField] private int buttonIndex = 0; // Set in Inspector or by manager

    private Vector3 originalPosition;
    private bool isPressed = false;

    private void Awake()
    {
        originalPosition = transform.localPosition;
    }

    public void Interact()
    {
        if (!isPressed)
        {
            StartCoroutine(PressAnimation());
            ButtonPuzzleManager.Instance.OnButtonPressed(buttonIndex);
        }
    }

    private System.Collections.IEnumerator PressAnimation()
    {
        isPressed = true;
        Vector3 target = originalPosition + pressedOffset;
        float t = 0;
        while (t < pressDuration)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, target, t / pressDuration);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = target;
        yield return new WaitForSeconds(0.1f);
        t = 0;
        while (t < pressDuration)
        {
            transform.localPosition = Vector3.Lerp(target, originalPosition, t / pressDuration);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
        isPressed = false;
    }
}
