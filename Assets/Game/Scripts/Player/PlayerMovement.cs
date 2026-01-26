using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float walkSpeed = 5;
    [SerializeField] private float runSpeed = 10;
    [SerializeField] private float crouchSpeed = 2.5f;

    [Header("Jump and Fall")]
    [SerializeField] private float jumpForce = 7;
    [SerializeField] private float gravity = -12;
    [SerializeField] private float initialFallVelocity = -2;

    [Header("Stamina")]
    [SerializeField] private PlayerStamina playerStamina;

    [Header("Double and Triple Jump")]
    [SerializeField] private int maxJumps = 0;
    private int jumpCount = 0;
    public bool doubleJumpUnlocked = false;
    public bool unlocksTripleJump = false;

    [Header("Crouching")]
    [SerializeField] private float standingHeight = 2;
    [SerializeField] private float crouchingHeight = 1;
    [SerializeField] private float crouchTransitionSpeed = 10;
    [SerializeField] private float cameraOffset = 0.5f;

    [Header("Wall Climb")]
    [SerializeField] private string wallTag = "Climbable";
    [SerializeField] private float wallClimbSpeed = 4f;
    private bool isClimbingWall = false;
    private bool isTouchingWall = false;
    private Vector3 wallNormal;
    [SerializeField] private float wallStickTime = 0.5f;
    [SerializeField] private float wallJumpForce = 16f;
    private float wallStickTimer = 0f;
    [SerializeField] private string stickyWallTag = "Sticky";
    [SerializeField] private float stickySlideSpeed = 1.5f;
    private bool isOnStickyWall = false;

    [Header("Slide")]
    [SerializeField] private float slideSpeed = 16f;
    [SerializeField] private float slideDuration = 0.7f;
    private bool isSliding = false;
    private float slideTimer = 0f;

    [Header("Space Movement")]
    [SerializeField] private bool isInSpaceMode = false;
    [SerializeField] private float spaceSpeed = 8f;
    [SerializeField] private float spaceBoostSpeed = 15f;
    [SerializeField] private float spaceDrag = 2f;
    [SerializeField] private float spaceAcceleration = 10f;
    [SerializeField] private float spaceDamagePerSecond = 3f;
    [SerializeField] private float spaceDamageInterval = 1f;
    private Vector3 spaceVelocity = Vector3.zero;
    private float spaceDamageTimer = 0f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference crouchAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Hands")]
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;
    private PickupItem heldLeftItem;
    private PickupItem heldRightItem;
    [SerializeField] private Transform bothHands;
    [SerializeField] private Transform dropPoint;
    [SerializeField] private float throwPower = 2f;

    [Header("Item UI")]
    [SerializeField] private TextMeshProUGUI dropInstructionText;
    [SerializeField] private string dropMessage = "Hold G + Left/Right Click to drop item";

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Camera Look")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float clampAngle = 80f;
    private float xRotation = 0f;

    [Header("Viewbobbing")]
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float walkBobSpeed = 12f;
    [SerializeField] private float landBobAmount = 0.15f;
    [SerializeField] private float landBobDuration = 0.1f;
    [SerializeField] private float jumpBobAmount = 0.08f;
    [SerializeField] private float jumpBobDuration = 0.08f;
    private Vector3 cameraDefaultLocalPos;
    private float bobTimer = 0f;
    private bool wasGroundedLastFrame = true;
    private bool isLanding = false;
    private float landBobTimer = 0f;
    private bool isJumping = false;
    private float jumpBobTimer = 0f;

    private CharacterController _characterController;
    private Vector2 _moveInput;
    private bool _isCrouching;
    private float _verticalVelocity;
    private bool _isGrounded;
    private bool _isRunning;
    private float _targetHeight;
    private bool viewBobbingEnabled = true;

    [Header("Rolling")]
    [SerializeField] private float rollSpeed = 12f;
    [SerializeField] private float rollDuration = 0.4f;
    private bool isRolling = false;
    private float rollTimer = 0f;
    [SerializeField] private InputActionReference rollAction;

    [SerializeField] private InputActionReference dropAction;
    [SerializeField] private InputActionReference leftMouseAction;
    [SerializeField] private InputActionReference rightMouseAction;
    private bool isDropHeld = false;

    private bool _controlsEnabled = true;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _targetHeight = standingHeight;
        cameraDefaultLocalPos = cameraTransform.localPosition;

        // Try to find PlayerHealth if not assigned
        if (playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHealth>();
            if (playerHealth == null)
            {
                playerHealth = GetComponentInParent<PlayerHealth>();
            }
            if (playerHealth == null)
            {
                playerHealth = GetComponentInChildren<PlayerHealth>();
            }
        }

        // Hide drop instruction at start
        if (dropInstructionText != null)
        {
            dropInstructionText.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        moveAction.action.performed += StoreMovementInput;
        moveAction.action.canceled += StoreMovementInput;
        jumpAction.action.performed += Jump;
        sprintAction.action.performed += Sprint;
        sprintAction.action.canceled += Sprint;
        crouchAction.action.performed += Crouch;
        dropAction.action.performed += OnDropPressed;
        dropAction.action.canceled += OnDropReleased;
        leftMouseAction.action.performed += OnLeftMouse;
        rightMouseAction.action.performed += OnRightMouse;
        rollAction.action.performed += OnRoll;
    }

    private void OnDisable()
    {
        moveAction.action.performed -= StoreMovementInput;
        moveAction.action.canceled -= StoreMovementInput;
        jumpAction.action.performed -= Jump;
        sprintAction.action.performed -= Sprint;
        sprintAction.action.canceled -= Sprint;
        crouchAction.action.performed -= Crouch;
        dropAction.action.performed -= OnDropPressed;
        dropAction.action.canceled -= OnDropReleased;
        leftMouseAction.action.performed -= OnLeftMouse;
        rightMouseAction.action.performed -= OnRightMouse;
        rollAction.action.performed -= OnRoll;
    }

    private void Update()
    {
        if (!_controlsEnabled) return;
        if (isInSpaceMode)
        {
            HandleSpaceMovement();
            HandleSpaceDamage();
            HandleCameraLook();
        }
        else
        {
            _isGrounded = _characterController.isGrounded;
            if (_isGrounded) jumpCount = 0;
            HandleGravity();
            HandleRoll();
            if (!isRolling)
                HandleMovement();
            HandleCrouchTransition();
            HandleAnimation();
            HandleRotation();
            HandleCameraLook();
            HandleViewbobbing();
            HandleWallClimb();
        }

        UpdateDropInstructionUI();
    }

    private void UpdateDropInstructionUI()
    {
        if (dropInstructionText == null) return;

        bool hasItem = heldLeftItem != null || heldRightItem != null;

        if (hasItem && !isInSpaceMode)
        {
            dropInstructionText.text = dropMessage;
            dropInstructionText.gameObject.SetActive(true);
        }
        else
        {
            dropInstructionText.gameObject.SetActive(false);
        }
    }

    // Public method to toggle space mode (call from trigger or script)
    public void SetSpaceMode(bool enabled)
    {
        isInSpaceMode = enabled;
        if (enabled)
        {
            spaceVelocity = Vector3.zero;
            _verticalVelocity = 0;
            spaceDamageTimer = 0f;
        }
    }

    private void HandleSpaceDamage()
    {
        if (playerHealth == null) return;

        spaceDamageTimer += Time.deltaTime;
        if (spaceDamageTimer >= spaceDamageInterval)
        {
            float damage = spaceDamagePerSecond * spaceDamageInterval;
            playerHealth.TakeDamage(damage);
            spaceDamageTimer = 0f;
        }
    }

    private void HandleSpaceMovement()
    {
        // Check exhaustion in space mode
        if (playerStamina != null && playerStamina.IsExhausted())
        {
            _isRunning = false;
            // Drift to a stop
            spaceVelocity = Vector3.Lerp(spaceVelocity, Vector3.zero, spaceDrag * 2f * Time.deltaTime);
            _characterController.Move(spaceVelocity * Time.deltaTime);
            return;
        }

        Vector3 inputDir = Vector3.zero;

        inputDir += cameraTransform.forward * _moveInput.y;
        inputDir += cameraTransform.right * _moveInput.x;

        if (jumpAction.action.IsPressed())
        {
            inputDir += Vector3.up;
        }
        if (crouchAction.action.IsPressed())
        {
            inputDir += Vector3.down;
        }

        inputDir.Normalize();

        float currentSpeed = spaceSpeed;

        // Handle boost in space
        if (_isRunning && inputDir.magnitude > 0.1f)
        {
            if (playerStamina != null)
            {
                if (playerStamina.HasStamina(1f))
                {
                    currentSpeed = spaceBoostSpeed;
                    playerStamina.DrainStamina(playerStamina.spaceBoostDrainRate);
                }
                else
                {
                    _isRunning = false;
                }
            }
            else
            {
                currentSpeed = spaceBoostSpeed;
            }
        }

        Vector3 targetVelocity = inputDir * currentSpeed;
        spaceVelocity = Vector3.Lerp(spaceVelocity, targetVelocity, spaceAcceleration * Time.deltaTime);

        if (inputDir.magnitude < 0.1f)
        {
            spaceVelocity = Vector3.Lerp(spaceVelocity, Vector3.zero, spaceDrag * Time.deltaTime);
        }

        _characterController.Move(spaceVelocity * Time.deltaTime);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isInSpaceMode) return;
        if (playerStamina != null && playerStamina.IsExhausted()) return;

        if (_isGrounded && jumpCount == 0)
        {
            if (playerStamina != null && !playerStamina.UseStamina(playerStamina.jumpCost))
            {
                return; // Not enough stamina
            }
            _verticalVelocity = jumpForce;
            jumpCount++;
        }
        else if (isClimbingWall)
        {
            if (playerStamina != null && !playerStamina.UseStamina(playerStamina.jumpCost))
            {
                return;
            }
            Vector3 jumpDir = (wallNormal * 0.7f + Vector3.up * 0.3f).normalized;
            _characterController.Move(jumpDir * wallJumpForce * Time.deltaTime);
            _verticalVelocity = wallJumpForce * 0.3f;
            isClimbingWall = false;
            jumpCount++;
        }
        else if (jumpCount < maxJumps)
        {
            if (playerStamina != null && !playerStamina.UseStamina(playerStamina.jumpCost))
            {
                return;
            }
            _verticalVelocity = jumpForce;
            jumpCount++;
        }
    }

    public void UnlockDoubleJump()
    {
        doubleJumpUnlocked = true;
        maxJumps = 1;
    }

    public void UnlockTripleJump()
    {
        doubleJumpUnlocked = true;
        maxJumps = 2;
    }

    private void Crouch(InputAction.CallbackContext context)
    {
        if (isInSpaceMode) return; // Handled in HandleSpaceMovement

        if (_isCrouching)
        {
            if (!CanStandUp())
            {
                return;
            }
            _targetHeight = standingHeight;
        }
        else
        {
            _targetHeight = crouchingHeight;
        }
        _isCrouching = !_isCrouching;
    }

    private bool CanStandUp()
    {
        // Calculate the height difference between standing and crouching
        float heightDifference = standingHeight - crouchingHeight;

        // Get current center position (which adjusts with crouch height)
        Vector3 currentCenter = transform.position + _characterController.center;

        // Bottom of capsule cast should be at current center
        Vector3 bottom = currentCenter - Vector3.up * (_characterController.height / 2f);

        // Top should be at the top of the current crouch capsule
        Vector3 top = currentCenter + Vector3.up * (_characterController.height / 2f);

        // Check if there's an obstacle in the space above where we would expand to
        bool hasObstacle = Physics.SphereCast(
            top,
            _characterController.radius,
            Vector3.up,
            out RaycastHit hit,
            heightDifference - 0.1f); // Small buffer to prevent edge cases

        return !hasObstacle;
    }

    private void Sprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Only allow sprint if we have stamina
            if (playerStamina != null && playerStamina.HasStamina(1f))
            {
                _isRunning = true;
            }
        }
        else
        {
            _isRunning = false;
        }
    }

    private void HandleGravity()
    {
        if (_isGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = initialFallVelocity;
        }

        _verticalVelocity += gravity * Time.deltaTime;
    }

    private void HandleMovement()
    {
        var move = cameraTransform.TransformDirection(new Vector3(_moveInput.x, 0, _moveInput.y)).normalized;
        float currentSpeed; // Declare once at the top
        Vector3 finalMove; // Declare once at the top

        // Check exhaustion
        if (playerStamina != null && playerStamina.IsExhausted())
        {
            _isRunning = false;
            // Can still walk while exhausted
            currentSpeed = _isCrouching ? crouchSpeed : walkSpeed;
            finalMove = move * currentSpeed;
            finalMove.y = _verticalVelocity;
            var collisions = _characterController.Move(finalMove * Time.deltaTime);
            if ((collisions & CollisionFlags.Above) != 0)
            {
                _verticalVelocity = 0;
            }
            return;
        }

        currentSpeed = _isCrouching ? crouchSpeed : _isRunning ? runSpeed : walkSpeed;

        // Drain stamina while running
        if (_isRunning && _moveInput.magnitude > 0.1f && _isGrounded)
        {
            if (playerStamina != null)
            {
                if (playerStamina.HasStamina(1f))
                {
                    playerStamina.DrainStamina(playerStamina.sprintDrainRate);
                }
                else
                {
                    _isRunning = false; // Stop running if out of stamina
                }
            }
        }

        finalMove = move * currentSpeed;
        finalMove.y = _verticalVelocity;

        var collisions2 = _characterController.Move(finalMove * Time.deltaTime);
        if ((collisions2 & CollisionFlags.Above) != 0)
        {
            _verticalVelocity = 0;
        }
    }

    private void HandleCrouchTransition()
    {
        var currentHeight = _characterController.height;
        if (Mathf.Abs(currentHeight - _targetHeight) < 0.01f)
        {
            _characterController.height = _targetHeight;
        }

        var newHeight = Mathf.Lerp(currentHeight, _targetHeight, crouchTransitionSpeed * Time.deltaTime);
        _characterController.height = newHeight;
        _characterController.center = Vector3.up * (newHeight * 0.5f);

        var cameraTargetPosition = cameraTransform.localPosition;
        cameraTargetPosition.y = _targetHeight - cameraOffset;
        cameraTransform.localPosition = Vector3.Lerp(
            cameraTransform.localPosition,
            cameraTargetPosition,
            crouchTransitionSpeed * Time.deltaTime);
    }

    private void HandleAnimation()
    {
        bool isMoving = _moveInput.magnitude > 0.1f;
        bool isCrouchWalking = _isCrouching && isMoving;
        bool isWalking = !_isCrouching && isMoving && _isGrounded;
        bool isIdle = !_isCrouching && !isMoving && _isGrounded;
        bool isJumping = !_isGrounded && _verticalVelocity > 0.1f;
        bool isCrouching = _isCrouching && !isMoving && _isGrounded;

        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isCrouching", isCrouching);
        animator.SetBool("isCrouchWalking", isCrouchWalking);
        animator.SetBool("isIdle", isIdle);
    }

    private void HandleRotation()
    {
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0;
        if (cameraForward.sqrMagnitude > 0.001f)
        {
            transform.forward = cameraForward.normalized;
        }
    }

    private Vector2 currentVelocity = Vector2.zero;
    private float deceleration = 10;

    private void HandleCameraLook()
    {
        if (Mouse.current == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        if (mouseDelta.sqrMagnitude > 0.0001f)
        {
            currentVelocity = mouseDelta * mouseSensitivity;
        }
        else
        {
            currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, deceleration * Time.deltaTime);
        }

        float mouseX = currentVelocity.x;
        float mouseY = currentVelocity.y;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -clampAngle, clampAngle);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        if (isInSpaceMode)
        {
            // In space, rotate the entire player body with mouse X
            transform.Rotate(Vector3.up * mouseX, Space.World);
        }
        else
        {
            transform.Rotate(Vector3.up * mouseX);
        }
    }

    public void SetViewBobbing(bool enabled)
    {
        viewBobbingEnabled = enabled;
    }

    private void HandleViewbobbing()
    {
        if (!viewBobbingEnabled) return;
        if (!_isGrounded && wasGroundedLastFrame)
        {
            isJumping = true;
            jumpBobTimer = 0f;
        }
        if (_isGrounded && !wasGroundedLastFrame)
        {
            isLanding = true;
            landBobTimer = 0f;
        }
        wasGroundedLastFrame = _isGrounded;

        Vector3 targetPos = cameraDefaultLocalPos;

        bool isWalking = _moveInput.magnitude > 0.1f && _isGrounded && !_isCrouching;
        if (isWalking)
        {
            bobTimer += Time.deltaTime * walkBobSpeed;
            targetPos.y += Mathf.Sin(bobTimer) * walkBobAmount;
        }
        else
        {
            bobTimer = 0f;
        }

        if (isJumping)
        {
            jumpBobTimer += Time.deltaTime;
            float t = jumpBobTimer / jumpBobDuration;
            if (t < 1f)
            {
                targetPos.y -= Mathf.Sin(t * Mathf.PI) * jumpBobAmount;
            }
            else
            {
                isJumping = false;
            }
        }

        if (isLanding)
        {
            landBobTimer += Time.deltaTime;
            float t = landBobTimer / landBobDuration;
            if (t < 1f)
            {
                targetPos.y -= Mathf.Sin(t * Mathf.PI) * landBobAmount;
            }
            else
            {
                isLanding = false;
            }
        }
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPos, 12f * Time.deltaTime);
    }

    private Vector3 rollDirection;

    private void OnRoll(InputAction.CallbackContext context)
    {
        if (isInSpaceMode) return;
        if (playerStamina != null && playerStamina.IsExhausted()) return;

        if (!isRolling && _isGrounded && _moveInput.magnitude > 0.1f)
        {
            if (playerStamina != null && !playerStamina.UseStamina(playerStamina.rollCost))
            {
                return; // Not enough stamina
            }

            isRolling = true;
            rollTimer = 0f;
            Vector3 inputDir = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;
            rollDirection = cameraTransform.TransformDirection(inputDir);
            rollDirection.y = 0;
            rollDirection.Normalize();

            if (rollDirection.sqrMagnitude > 0.01f)
                transform.forward = rollDirection;

            if (animator != null)
                animator.SetTrigger("Roll");
        }
    }

    private void HandleRoll()
    {
        if (isRolling)
        {
            rollTimer += Time.deltaTime;
            _characterController.Move(rollDirection * rollSpeed * Time.deltaTime);

            if (rollTimer >= rollDuration)
            {
                isRolling = false;
                rollTimer = 0f;
            }
        }
    }

    private void HandleWallClimb()
    {
        isClimbingWall = false;
        isTouchingWall = false;
        isOnStickyWall = false;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1.1f))
        {
            if (hit.collider.CompareTag(wallTag) && !_isGrounded && _moveInput.y > 0)
            {
                isClimbingWall = true;
                isTouchingWall = true;
                wallNormal = hit.normal;
                wallStickTimer = wallStickTime;
            }
            else if (hit.collider.CompareTag(stickyWallTag) && !_isGrounded)
            {
                isOnStickyWall = true;
                wallNormal = hit.normal;
            }
        }
        if (isClimbingWall)
        {
            _verticalVelocity = 0;
            Vector3 alongWall = Vector3.Cross(wallNormal, Vector3.up).normalized;
            float input = Mathf.Clamp(_moveInput.x, -1f, 1f);
            Vector3 move = alongWall * input * wallClimbSpeed;
            _characterController.Move(move * Time.deltaTime);

            if (_moveInput.magnitude < 0.1f)
                _characterController.Move(Vector3.down * 0.5f * Time.deltaTime);

            wallStickTimer -= Time.deltaTime;
            if (wallStickTimer <= 0f && !isTouchingWall)
            {
                isClimbingWall = false;
            }
        }
        if (isOnStickyWall)
        {
            _verticalVelocity = -stickySlideSpeed;
        }
    }

    public void HoldItemInHand(PickupItem item, HandType hand)
    {
        switch (hand)
        {
            case HandType.Left:
                if (heldLeftItem != null) ReleaseItem(heldLeftItem, true);
                heldLeftItem = item;
                item.transform.SetParent(leftHand);
                item.transform.localPosition = Vector3.zero;
                item.transform.localRotation = Quaternion.identity;
                break;
            case HandType.Right:
                if (heldRightItem != null) ReleaseItem(heldRightItem, true);
                heldRightItem = item;
                item.transform.SetParent(rightHand);
                item.transform.localPosition = Vector3.zero;
                item.transform.localRotation = Quaternion.identity;
                break;
            case HandType.Both:
                if (heldLeftItem != null) ReleaseItem(heldLeftItem, true);
                if (heldRightItem != null) ReleaseItem(heldRightItem, true);
                heldLeftItem = item;
                heldRightItem = item;
                item.transform.SetParent(bothHands);
                item.transform.localPosition = Vector3.zero;
                item.transform.localRotation = Quaternion.identity;
                break;
        }
        var rb = item.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
        foreach (var col in item.GetComponentsInChildren<Collider>())
            col.enabled = false;

        item.SetHeld(true);
    }

    private void ReleaseItem(PickupItem item, bool withForce)
    {
        if (isInSpaceMode) return; // Disable dropping in space mode

        item.transform.SetParent(null);
        if (withForce)
        {
            item.transform.position = transform.position + transform.forward + Vector3.up * 0.5f;
            var rb = item.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.linearVelocity = Vector3.zero;
                rb.AddForce(transform.forward * 2f + Vector3.up, ForceMode.Impulse);
                rb.AddForce(transform.forward * throwPower + Vector3.up, ForceMode.Impulse);
            }
        }
        else
        {
            Vector3 dropPos = dropPoint != null ? dropPoint.position : (transform.position + Vector3.down * 0.5f);
            item.transform.position = dropPos;
            var rb = item.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.linearVelocity = Vector3.zero;
            }
        }
        item.SetHeld(false);

        foreach (var col in item.GetComponentsInChildren<Collider>())
            col.enabled = true;
    }

    private void OnDropReleased(InputAction.CallbackContext context)
    {
        if (isInSpaceMode) return; // Disable dropping in space

        isDropHeld = false;
        if (heldLeftItem != null && heldLeftItem == heldRightItem)
        {
            ReleaseItem(heldLeftItem, true);
            heldLeftItem = null;
            heldRightItem = null;
        }
        else
        {
            if (heldLeftItem != null)
            {
                ReleaseItem(heldLeftItem, true);
                heldLeftItem = null;
            }
            if (heldRightItem != null)
            {
                ReleaseItem(heldRightItem, true);
                heldRightItem = null;
            }
        }
    }

    private void OnDropPressed(InputAction.CallbackContext context)
    {
        if (isInSpaceMode) return;
        isDropHeld = true;
    }

    private void OnLeftMouse(InputAction.CallbackContext context)
    {
        if (isInSpaceMode) return;

        if (isDropHeld && heldLeftItem != null && heldLeftItem != heldRightItem)
        {
            ReleaseItem(heldLeftItem, true);
            heldLeftItem = null;
        }
        else if (isDropHeld && heldLeftItem != null && heldLeftItem == heldRightItem)
        {
            ReleaseItem(heldLeftItem, true);
            heldLeftItem = null;
            heldRightItem = null;
        }
    }

    private void OnRightMouse(InputAction.CallbackContext context)
    {
        if (isInSpaceMode) return;

        if (isDropHeld && heldRightItem != null && heldRightItem != heldLeftItem)
        {
            ReleaseItem(heldRightItem, true);
            heldRightItem = null;
        }
        else if (isDropHeld && heldRightItem != null && heldRightItem == heldLeftItem)
        {
            ReleaseItem(heldRightItem, true);
            heldLeftItem = null;
            heldRightItem = null;
        }
    }

    private void StoreMovementInput(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void SetControlsEnabled(bool enabled)
    {
        _controlsEnabled = enabled;
    }
}