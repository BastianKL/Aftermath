using UnityEngine;
using UnityEngine.Events;

public class ToggleableInteractable : MonoBehaviour, Interactable
{
    [SerializeField] private bool isOpen;
    [SerializeField] private UnityEvent onOpen;
    [SerializeField] private UnityEvent onClose;
    [SerializeField] private bool stopsTimer = false;

    public void Interact()
    {
        if (isOpen)
        {
            onClose.Invoke();
        }
        else
        {
            onOpen.Invoke();

            // Stop timer when this button is pressed
            if (stopsTimer && PlayerTimer.Instance != null)
            {
                PlayerTimer.Instance.StopTimer();
            }
        }
        isOpen = !isOpen;
    }
}