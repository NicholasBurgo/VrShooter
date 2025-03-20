using UnityEngine;
using System.Collections.Generic;

public class WaypointCarController : MonoBehaviour
{
    [Tooltip("The starting waypoint node for the car.")]
    public WaypointNode startNode;

    [Tooltip("Movement speed of the car.")]
    public float speed = 5f;

    // The current waypoint the car is moving toward.
    private WaypointNode currentWaypoint;

    void Start()
    {
        if (startNode != null)
        {
            currentWaypoint = startNode;
        }
        else
        {
            Debug.LogError("No starting waypoint node assigned to WaypointCarController on " + gameObject.name);
        }
    }

    void Update()
    {
        if (currentWaypoint == null)
            return;

        // Move toward the current waypoint.
        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.transform.position, speed * Time.deltaTime);

        // Rotate the car to face the current waypoint.
        Vector3 moveDir = currentWaypoint.transform.position - transform.position;
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        // When close enough to the waypoint...
        if (Vector3.Distance(transform.position, currentWaypoint.transform.position) < 0.2f)
        {
            // If this waypoint is an end node, destroy the car.
            if (currentWaypoint.isEndNode)
            {
                Destroy(gameObject);
            }
            else if (currentWaypoint.nextWaypoints != null && currentWaypoint.nextWaypoints.Count > 0)
            {
                // Choose a random next waypoint from the list.
                int randomIndex = Random.Range(0, currentWaypoint.nextWaypoints.Count);
                currentWaypoint = currentWaypoint.nextWaypoints[randomIndex];
            }
            else
            {
                // If there is no next waypoint, simply destroy the car (or you could decide to loop, etc.).
                Destroy(gameObject);
            }
        }
    }
}
