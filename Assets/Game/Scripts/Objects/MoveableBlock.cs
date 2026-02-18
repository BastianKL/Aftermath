using UnityEngine;

public class MoveableBlock : MonoBehaviour, Interactable
{
    [SerializeField] private float moveDistance = 1f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Vector3 blockSize = new Vector3(1, 1, 1); // Set to match your block's collider size
    [SerializeField] private LayerMask obstacleLayers; // Set in Inspector

    private bool isMoving = false;

    // Called by InteractionController when player presses E
    public void Interact()
    {
        var player = FindObjectOfType<PlayerMovement>();
        if (player != null)
            TryPush(player.transform.position);
    }

    public void TryPush(Vector3 playerPosition)
    {
        if (isMoving) return;
        Vector3 pushDir = GetPushDirection(playerPosition);
        if (pushDir != Vector3.zero)
        {
            Vector3 start = transform.position;
            Vector3 end = start + pushDir * moveDistance;

            // BoxCast to check if path is clear
            if (!Physics.BoxCast(
                start,
                blockSize * 0.5f,
                pushDir,
                transform.rotation,
                moveDistance,
                obstacleLayers))
            {
                StartCoroutine(MoveBlock(pushDir));
            }
        }
    }

    private Vector3 GetPushDirection(Vector3 playerPosition)
    {
        Vector3 dir = (transform.position - playerPosition).normalized;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
            return dir.x > 0 ? Vector3.right : Vector3.left;
        else
            return dir.z > 0 ? Vector3.forward : Vector3.back;
    }

    private System.Collections.IEnumerator MoveBlock(Vector3 direction)
    {
        isMoving = true;
        Vector3 start = transform.position;
        Vector3 end = start + direction * moveDistance;
        float t = 0;
        while (t < 1f)
        {
            transform.position = Vector3.Lerp(start, end, t);
            t += Time.deltaTime * moveSpeed;
            yield return null;
        }
        transform.position = end;

        Collider[] hits = Physics.OverlapBox(
            transform.position,
            blockSize * 0.5f,
            transform.rotation,
            obstacleLayers);

        bool overlappingOther = false;
        foreach (var hit in hits)
        {
            if (hit.gameObject != gameObject)
            {
                overlappingOther = true;
                break;
            }
        }

        if (overlappingOther)
        {
            // Revert move if overlapping another object
            transform.position = start;
        }

        isMoving = false;
    }
}
