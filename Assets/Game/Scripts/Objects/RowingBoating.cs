using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class RowingBoating : MonoBehaviour, Interactable
{
    public Transform seatPoint;

    public float strokeForce = 6f;
    public float turnTorque = 2.5f;
    public float strokeCooldown = 0.25f;
    public float reverseForce = 4f;
    public float linearDrag = 2f;
    public float angularDrag = 3f;


    private Rigidbody rb;

    private PlayerMovement seatedPlayerMove;
    private Transform seatedPlayer;
    private CharacterController seatedCC;

    private float nextLeftTime;
    private float nextRightTime;
    private float nextForwardTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Remove rotation constraints for realistic floating
        // rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Lower center of mass for stability
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // Adjust Y as needed
    }

    public void Interact()
    {
        if (seatPoint == null) return;

        if (seatedPlayer == null)
            TryEnter();
        else
            Exit();
    }

    void FixedUpdate()
    {
        if (seatedPlayer == null) return;

        rb.linearDamping = linearDrag;
        rb.angularDamping = angularDrag;

        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb[Key.D].isPressed) TryLeft();
        if (kb[Key.A].isPressed) TryRight();
        if (kb[Key.W].isPressed) TryForward();

        if (kb[Key.S].isPressed)
            rb.AddForce(-transform.forward * reverseForce, ForceMode.Force);
    }

    private void TryEnter()
    {
        var player = FindObjectOfType<PlayerMovement>();
        if (player == null) return;

        seatedPlayerMove = player;
        seatedPlayer = player.transform;
        seatedCC = player.GetComponent<CharacterController>();

        if (seatedCC != null) seatedCC.enabled = false;
        seatedPlayerMove.SetControlsEnabled(false);

        seatedPlayer.SetParent(seatPoint, true);
        seatedPlayer.localPosition = Vector3.zero;
        seatedPlayer.localRotation = Quaternion.identity;
        Vector3 e = transform.eulerAngles;
        transform.eulerAngles = new Vector3(e.x, e.y, 0f);
        rb.angularVelocity = Vector3.zero;
    }

    private void Exit()
    {
        if (seatedPlayer == null) return;

        seatedPlayer.SetParent(null, true);

        Vector3 exitPos = transform.position + transform.right * 1.2f + Vector3.up * 0.2f;
        seatedPlayer.position = exitPos;

        if (seatedCC != null) seatedCC.enabled = true;
        if (seatedPlayerMove != null) seatedPlayerMove.SetControlsEnabled(true);

        seatedPlayer = null;
        seatedPlayerMove = null;
        seatedCC = null;
    }

    private void TryLeft()
    {
        if (Time.time < nextLeftTime) return;
        nextLeftTime = Time.time + strokeCooldown;

        rb.AddForce(transform.forward * strokeForce, ForceMode.Impulse);
        rb.AddTorque(Vector3.up * turnTorque, ForceMode.Impulse);
    }

    private void TryRight()
    {
        if (Time.time < nextRightTime) return;
        nextRightTime = Time.time + strokeCooldown;

        rb.AddForce(transform.forward * strokeForce, ForceMode.Impulse);
        rb.AddTorque(-Vector3.up * turnTorque, ForceMode.Impulse);
    }

    private void TryForward()
    {
        if (Time.time < nextForwardTime) return;
        nextForwardTime = Time.time + strokeCooldown;

        rb.AddForce(transform.forward * strokeForce * 2f, ForceMode.Impulse);
    }
}
