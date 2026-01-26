using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InteractionController : MonoBehaviour
{
    private Transform _cameraTransform;
    private Interactable _currentInteractable;

    [SerializeField] private float interactionDistance = 3;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private string interactionMessage = "Press E to interact";


    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        DetectInteractable();
    }

    private void OnEnable()
    {
        interactAction.action.performed += HandleInteraction;
    }

    private void OnDisable()
    {
        interactAction.action.performed -= HandleInteraction;
    }

    private void HandleInteraction(InputAction.CallbackContext context)
    {
        if (_currentInteractable != null)
        {
            _currentInteractable.Interact();
        }
    }

    private void DetectInteractable()
    {
        var ray = new Ray(_cameraTransform.position, _cameraTransform.forward);
        Interactable detectedInteractable = null;
        if (Physics.Raycast(ray, out var hit, interactionDistance, interactionLayer))
        {
            hit.collider.TryGetComponent(out detectedInteractable);
        }

        if (_currentInteractable != detectedInteractable)
        {
            _currentInteractable = detectedInteractable;
            UpdateInteractionUI();
        }
    }

    private void UpdateInteractionUI()
    {
        if (interactionText == null) return;

        if (_currentInteractable != null)
        {
            interactionText.text = interactionMessage;
            interactionText.gameObject.SetActive(true);
        }
        else
        {
            interactionText.gameObject.SetActive(false);
        }
    }
}