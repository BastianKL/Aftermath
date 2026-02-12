using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private Vector3 pressedOffset = new Vector3(0, -0.1f, 0);
    [SerializeField] private float pressDuration = 0.15f;
    [SerializeField] private float releaseDelay = 2f;

    [Header("Events")]
    public UnityEvent onPlatePressed;
    public UnityEvent onPlateReleased;

    private Vector3 originalPosition;
    private bool isPressed = false;
    private Coroutine releaseCoroutine;

    private void Awake()
    {
        originalPosition = transform.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPressed)
        {
            StartCoroutine(PressAnimation());
            onPlatePressed.Invoke();
            if (releaseCoroutine != null)
                StopCoroutine(releaseCoroutine);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            releaseCoroutine = StartCoroutine(ReleaseAnimation());
            onPlateReleased.Invoke();
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
    }

    private System.Collections.IEnumerator ReleaseAnimation()
    {
        yield return new WaitForSeconds(releaseDelay);
        Vector3 start = transform.localPosition;
        float t = 0;
        while (t < pressDuration)
        {
            transform.localPosition = Vector3.Lerp(start, originalPosition, t / pressDuration);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
        isPressed = false;
    }
}
