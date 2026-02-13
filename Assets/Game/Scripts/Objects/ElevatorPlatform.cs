using UnityEngine;

public class ElevatorPlatform : MonoBehaviour
{
    public Transform topPoint;
    public Transform bottomPoint;
    public float speed = 2f;
    public bool startAtBottom = true;
    public bool moveUpOnPlate = true; // If false, moves down on plate

    private bool isMoving = false;
    private bool isAtTop = false;
    private bool isAtBottom = true;
    private Vector3 targetPosition;

    private void Start()
    {
        if (startAtBottom)
        {
            transform.position = bottomPoint.position;
            isAtBottom = true;
            isAtTop = false;
        }
        else
        {
            transform.position = topPoint.position;
            isAtBottom = false;
            isAtTop = true;
        }
    }

    public void OnPlatePressed()
    {
        if (moveUpOnPlate)
            MoveTo(topPoint.position);
        else
            MoveTo(bottomPoint.position);
    }

    private void MoveTo(Vector3 position)
    {
        targetPosition = position;
        isMoving = true;
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
                // Automatically return after reaching the top/bottom
                if (moveUpOnPlate && targetPosition == topPoint.position)
                    Invoke(nameof(ReturnToBottom), 1.0f); // Wait 1s at top
                else if (!moveUpOnPlate && targetPosition == bottomPoint.position)
                    Invoke(nameof(ReturnToTop), 1.0f); // Wait 1s at bottom
            }
        }
    }

    private void ReturnToBottom()
    {
        MoveTo(bottomPoint.position);
    }

    private void ReturnToTop()
    {
        MoveTo(topPoint.position);
    }
}
