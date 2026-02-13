using UnityEngine;

public class ButtonElevator : MonoBehaviour
{
    public Transform topPoint;
    public Transform bottomPoint;
    public float speed = 2f;

    private Vector3 targetPosition;
    private bool isMoving = false;

    private void Start()
    {
        transform.position = bottomPoint.position;
        targetPosition = transform.position;
    }

    public void MoveUp()
    {
        targetPosition = topPoint.position;
        isMoving = true;
    }

    public void MoveDown()
    {
        targetPosition = bottomPoint.position;
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
            }
        }
    }
}
