using System.Collections;
using UnityEngine;

public class DoorSlider : MonoBehaviour, Interactable
{
    [Header("Door Settings")]
    [SerializeField] private float openHeight = 3f;
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private bool staysOpen = false;
    [SerializeField] private float autoCloseDelay = 3f;

    [Header("Interaction")]
    [SerializeField] private bool requiresInteraction = false;
    [SerializeField] private bool isLocked = false;

    [Header("Collision")]
    [SerializeField] private Collider doorCollider;

    [Header("Audio")]
    [SerializeField] private AudioSource openSound;
    [SerializeField] private AudioSource closeSound;

    private Vector3 _closedPosition;
    private Vector3 _openPosition;
    private Coroutine _currentCoroutine;
    private bool doorRequest;

    private void Start()
    {
        _closedPosition = transform.position;
        _openPosition = transform.position + Vector3.up * openHeight;

        if (doorCollider != null)
        {
            doorCollider.isTrigger = false;
        }
    }

    // Called by your existing interaction system
    public void Interact()
    {
        if (isLocked)
        {
            Debug.Log("Door is locked!");
            return;
        }

        // Toggle door state
        if (isOpen)
        {
            doorRequest = false;
        }
        else
        {
            doorRequest = true;
        }
    }

    public void RequestOpen()
    {
        if (!isLocked && !requiresInteraction)
        {
            doorRequest = true;
        }
    }

    public void RequestClose()
    {
        if (!staysOpen && !requiresInteraction)
        {
            doorRequest = false;
        }
    }

    public void Unlock()
    {
        isLocked = false;
        Debug.Log("Door unlocked!");
    }

    public void Lock()
    {
        isLocked = true;
    }

    private void Update()
    {
        if (doorRequest && !isOpen)
        {
            if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);

            // Disable collider before starting the coroutine
            if (doorCollider != null) doorCollider.enabled = false;

            _currentCoroutine = StartCoroutine(OpenDoor());
        }
        else if (!doorRequest && isOpen && !staysOpen)
        {
            if (_currentCoroutine != null) StopCoroutine(_currentCoroutine);
            _currentCoroutine = StartCoroutine(CloseDoor());
        }
    }

    private IEnumerator OpenDoor()
    {
        if (openSound != null) openSound.Play();

        if (doorCollider != null) doorCollider.enabled = false;

        while (Vector3.Distance(transform.position, _openPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, _openPosition, Time.deltaTime * openSpeed);
            yield return null;
        }

        transform.position = _openPosition;
        isOpen = true;

        // Enable collider only if the door stays open
        if (staysOpen && doorCollider != null)
        {
            doorCollider.enabled = true;
        }

        if (!staysOpen && !requiresInteraction)
        {
            yield return new WaitForSeconds(autoCloseDelay);
            doorRequest = false;
        }
    }

    private IEnumerator CloseDoor()
    {
        if (closeSound != null) closeSound.Play();

        while (Vector3.Distance(transform.position, _closedPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, _closedPosition, Time.deltaTime * openSpeed);
            yield return null;
        }

        transform.position = _closedPosition;
        isOpen = false;

        if (doorCollider != null) doorCollider.enabled = true;
    }
}