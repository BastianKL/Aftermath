using UnityEngine;

public class MoveableBlock : MonoBehaviour
{
    [SerializeField] private float moveDistance = 1f;
    [SerializeField] private float moveSpeed = 3f;
    private bool isMoving = false;

    // Called externally when player wants to push
    public void TryPush(Vector3 playerPosition)
    {
        if (isMoving) return;
        Vector3 pushDir = GetPushDirection(playerPosition);
        if (pushDir != Vector3.zero)
            StartCoroutine(MoveBlock(pushDir));
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
        isMoving = false;
    }
}
