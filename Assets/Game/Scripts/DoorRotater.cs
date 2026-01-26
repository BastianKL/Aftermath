using System.Collections;
using UnityEngine;

public class DoorRotater : MonoBehaviour, Interactable
{
    [Header("Door Settings")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private bool staysOpen = false;
    [SerializeField] private float autoCloseDelay = 3f;

    [Header("Interaction")]
    [SerializeField] private bool requiresInteraction = false; // Press E to open
    [SerializeField] private bool isLocked = false; // Door is locked until unlocked

    [Header("Collision")]
    [SerializeField] private Collider doorCollider; // Assign the door's collider

    [Header("Audio")]
    [SerializeField] private AudioSource openSound;
    [SerializeField] private AudioSource closeSound;

    private Quaternion _closedRotation;
    private Quaternion _openRotation;
    private Coroutine _currentCoroutine;
    private bool doorRequest;

    private void Start()
    {
        _closedRotation = transform.localRotation;
        _openRotation = Quaternion.Euler(transform.localEulerAngles + new Vector3(0, openAngle, 0));

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

        while (Quaternion.Angle(transform.localRotation, _openRotation) > 0.01f)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, _openRotation, Time.deltaTime * openSpeed);
            yield return null;
        }

        transform.localRotation = _openRotation;
        isOpen = true;

        if (!staysOpen && !requiresInteraction)
        {
            yield return new WaitForSeconds(autoCloseDelay);
            doorRequest = false;
        }
    }

    private IEnumerator CloseDoor()
    {
        if (closeSound != null) closeSound.Play();

        while (Quaternion.Angle(transform.localRotation, _closedRotation) > 0.01f)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, _closedRotation, Time.deltaTime * openSpeed);
            yield return null;
        }

        transform.localRotation = _closedRotation;
        isOpen = false;

        if (doorCollider != null) doorCollider.enabled = true;
    }
}