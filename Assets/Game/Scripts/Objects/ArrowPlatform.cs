using UnityEngine;
using System.Collections.Generic;

public class ArrowPlatform : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float tiltAngle = 10f;
    public float tiltSpeed = 5f;

    public Vector3 platformSize = new Vector3(1, 1, 1); // Set this to match your platform's collider size
    public LayerMask obstacleLayers; // Set this in the Inspector to include walls/obstacles

    private Vector3 targetDirection = Vector3.zero;
    private Quaternion originalRotation;

    // Track last position for delta calculation
    private Vector3 lastPosition;

    // Track players on the platform
    private HashSet<CharacterController> playersOnPlatform = new HashSet<CharacterController>();

    private void Awake()
    {
        originalRotation = transform.rotation;
        lastPosition = transform.position;
    }

    public void MoveInDirection(Vector3 direction)
    {
        targetDirection = direction;
    }

    public void StopMoving()
    {
        targetDirection = Vector3.zero;
    }

    private void Update()
    {
        Vector3 oldPosition = transform.position;
        Vector3 movement = Vector3.zero;

        if (targetDirection != Vector3.zero)
        {
            movement = targetDirection.normalized * moveSpeed * Time.deltaTime;

            // Check for obstacles using BoxCast
            if (!Physics.BoxCast(
                transform.position,
                platformSize * 0.5f,
                targetDirection.normalized,
                transform.rotation,
                movement.magnitude,
                obstacleLayers))
            {
                transform.position += movement;

                // After moving, check for overlap
                Collider[] hits = Physics.OverlapBox(
                    transform.position,
                    platformSize * 0.5f,
                    transform.rotation,
                    obstacleLayers);

                if (hits.Length > 0)
                {
                    // Revert move if overlapping
                    transform.position -= movement;
                }
            }
            // else: blocked, do not move
        }

        // Tilt logic (unchanged)
        Quaternion targetRot = originalRotation;
        if (targetDirection != Vector3.zero)
        {
            Vector3 tiltAxis = Vector3.Cross(Vector3.up, targetDirection.normalized);
            targetRot = Quaternion.AngleAxis(tiltAngle, tiltAxis) * originalRotation;
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, tiltSpeed * Time.deltaTime);

        // Move players by platform delta
        Vector3 delta = transform.position - oldPosition;
        foreach (var player in playersOnPlatform)
        {
            if (player != null)
            {
                player.Move(delta);
            }
        }
        lastPosition = transform.position;
    }

    // Call these from the arrow button triggers or platform trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var cc = other.GetComponent<CharacterController>();
            if (cc != null)
                playersOnPlatform.Add(cc);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var cc = other.GetComponent<CharacterController>();
            if (cc != null)
                playersOnPlatform.Remove(cc);
        }
    }
}
