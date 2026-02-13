using UnityEngine;
using UnityEngine.Events;

public class ArrowButton : MonoBehaviour
{
    public Vector3 pressedOffset = new Vector3(0, -0.1f, 0);
    public float pressDuration = 0.1f;
    public UnityEvent<Vector3> onArrowPressed; // Direction as parameter
    public UnityEvent onArrowReleased;

    private Vector3 originalPosition;
    private bool isPressed = false;

    [Tooltip("Direction this arrow represents (e.g., (0,0,1) for forward)")]
    public Vector3 moveDirection = Vector3.forward;

    private void Awake()
    {
        originalPosition = transform.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPressed)
        {
            isPressed = true;
            StartCoroutine(PressAnimation());
            Debug.Log($"{gameObject.name} pressed, direction: {moveDirection}");
            onArrowPressed.Invoke(moveDirection);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isPressed)
        {
            isPressed = false;
            StartCoroutine(ReleaseAnimation());
            Debug.Log($"{gameObject.name} released");
            onArrowReleased.Invoke();
        }
    }

    private System.Collections.IEnumerator PressAnimation()
    {
        Vector3 target = originalPosition + pressedOffset;
        float t = 0;
        while (t < pressDuration)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, target, t / pressDuration);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = target;
    }

    private System.Collections.IEnumerator ReleaseAnimation()
    {
        Vector3 start = transform.localPosition;
        float t = 0;
        while (t < pressDuration)
        {
            transform.localPosition = Vector3.Lerp(start, originalPosition, t / pressDuration);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
    }
}
