using UnityEngine;
using UnityEngine.Events;

public class SimpleInteract : MonoBehaviour, Interactable
{
    [SerializeField] private UnityEvent onInteract;
    [SerializeField] private bool startsTimer = false;

    [ContextMenu("Interact")]
    public void Interact()
    {
        onInteract.Invoke();

        // Start timer on first door interaction
        if (startsTimer && PlayerTimer.Instance != null)
        {
            PlayerTimer.Instance.StartTimer();
        }
    }
}