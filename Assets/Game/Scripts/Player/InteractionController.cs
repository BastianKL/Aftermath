using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InteractionController : MonoBehaviour
{
    private Transform _cameraTransform;
    private Interactable _currentInteractable;
    private bool _isBoat;

    [SerializeField] private float interactionDistance = 3;
    [SerializeField] private LayerMask interactionLayer;

    [SerializeField] private InputActionReference interactAction; // E
    [SerializeField] private InputActionReference boardAction;    // F

    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private string interactionMessage = "Press E to interact";
    [SerializeField] private string boardMessage = "Press F to board";

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }

    private void Update()
    {
        DetectInteractable();
    }

    private void OnEnable()
    {
        interactAction.action.performed += HandleInteractE;
        boardAction.action.performed += HandleBoardF;
    }

    private void OnDisable()
    {
        interactAction.action.performed -= HandleInteractE;
        boardAction.action.performed -= HandleBoardF;
    }

    private void HandleInteractE(InputAction.CallbackContext context)
    {
        if (_currentInteractable == null) return;
        if (_isBoat) return;
        _currentInteractable.Interact();
    }

    private void HandleBoardF(InputAction.CallbackContext context)
    {
        if (_currentInteractable == null) return;
        if (!_isBoat) return;
        _currentInteractable.Interact();
    }

    private void DetectInteractable()
    {
        var ray = new Ray(_cameraTransform.position, _cameraTransform.forward);

        Interactable detectedInteractable = null;
        bool detectedIsBoat = false;

        if (Physics.Raycast(ray, out var hit, interactionDistance, interactionLayer))
        {
            var pickup = hit.collider.GetComponentInParent<PickupItem>();
            if (pickup != null)
            {
                detectedInteractable = pickup;
                detectedIsBoat = false;
            }
            else
            {
                var boat = hit.collider.GetComponentInParent<RowingBoating>();
                if (boat != null)
                {
                    detectedInteractable = boat;
                    detectedIsBoat = true;
                }
                else
                {
                    hit.collider.TryGetComponent(out detectedInteractable);
                    detectedIsBoat = false;
                }
            }
        }

        if (_currentInteractable != detectedInteractable || _isBoat != detectedIsBoat)
        {
            _currentInteractable = detectedInteractable;
            _isBoat = detectedIsBoat;
            UpdateInteractionUI();
        }
    }

    private void UpdateInteractionUI()
    {
        if (interactionText == null) return;

        if (_currentInteractable != null)
        {
            interactionText.text = _isBoat ? boardMessage : interactionMessage;
            interactionText.gameObject.SetActive(true);
        }
        else
        {
            interactionText.gameObject.SetActive(false);
        }
    }
}