using UnityEngine;

public class MovingPlatformWaypoints : MonoBehaviour
{
    [Header("Waypoint Settings")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool loop = true;
    [SerializeField] private float waitTimeAtWaypoint = 1f;

    private int currentWaypointIndex = 0;
    private float waitTimer;
    private bool waiting;

    private void Update()
    {
        if (waypoints.Length == 0) return;

        if (waiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                waiting = false;
                NextWaypoint();
            }
            return;
        }

        // Move to current waypoint
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, moveSpeed * Time.deltaTime);

        // Check if reached waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            waiting = true;
            waitTimer = waitTimeAtWaypoint;
        }
    }

    private void NextWaypoint()
    {
        currentWaypointIndex++;

        if (currentWaypointIndex >= waypoints.Length)
        {
            if (loop)
            {
                currentWaypointIndex = 0;
            }
            else
            {
                currentWaypointIndex = waypoints.Length - 1;
            }
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
}