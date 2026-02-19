using UnityEngine;

public class MovingPlatformLinear : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Vector3 pointA = Vector3.zero;
    [SerializeField] private Vector3 pointB = Vector3.forward * 10f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool useLocalSpace = true;

    private Vector3 targetPoint;
    private Vector3 worldPointA;
    private Vector3 worldPointB;

    private void Start()
    {
        if (useLocalSpace)
        {
            worldPointA = transform.position + pointA;
            worldPointB = transform.position + pointB;
        }
        else
        {
            worldPointA = pointA;
            worldPointB = pointB;
        }

        targetPoint = worldPointB;
    }

    private void Update()
    {
        // Move towards target
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, moveSpeed * Time.deltaTime);

        // Switch direction when reached
        if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
        {
            targetPoint = targetPoint == worldPointB ? worldPointA : worldPointB;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 drawPointA = useLocalSpace ? transform.position + pointA : pointA;
        Vector3 drawPointB = useLocalSpace ? transform.position + pointB : pointB;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(drawPointA, 0.5f);
        Gizmos.DrawWireSphere(drawPointB, 0.5f);
        Gizmos.DrawLine(drawPointA, drawPointB);
    }
}