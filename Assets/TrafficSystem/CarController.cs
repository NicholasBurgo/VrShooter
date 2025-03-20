using UnityEngine;
using System.Collections.Generic;

public class CarController : MonoBehaviour
{
    [Tooltip("List of waypoints (including IntersectionNodes and the ExitNode) the car will follow.")]
    public List<Transform> waypoints;

    [Tooltip("Maximum speed of the car.")]
    public float maxSpeed = 25f;

    [Tooltip("Minimum possible acceleration for a car.")]
    public float minAcceleration = 3f;

    [Tooltip("Maximum possible acceleration for a car.")]
    public float maxAcceleration = 7f;

    [Tooltip("Rotation offset (in degrees) to apply when aligning the car's forward direction.")]
    public Vector3 rotationOffset = Vector3.zero;

    [Tooltip("Minimum safe following distance.")]
    public float safeDistanceMin = 2f;

    [Tooltip("Maximum safe following distance.")]
    public float safeDistanceMax = 4f;

    [Tooltip("Offset from the car's pivot where the detection sphere should start (e.g., at the front of the car).")]
    public Vector3 detectionOriginOffset = new Vector3(0, 0, 1f);

    [Tooltip("Radius of the detection sphere used to check for other cars.")]
    public float detectionRadius = 1f;

    private float safeDistance;
    private int currentWaypointIndex = 0;
    private Transform overrideTarget = null;
    private float currentSpeed = 0f; // Track current speed
    private float acceleration; // Random acceleration value

    void Start()
    {
        // Randomize the safe following distance for dynamic behavior.
        safeDistance = Random.Range(safeDistanceMin, safeDistanceMax);

        // Assign a random acceleration value for this car.
        acceleration = Random.Range(minAcceleration, maxAcceleration);
    }

    void Update()
    {
        // Process override target first (if set by an IntersectionNode).
        if (overrideTarget != null)
        {
            MoveTowards(overrideTarget);
            float distOverride = Vector3.Distance(transform.position, overrideTarget.position);

            // If the override target is an ExitNode, destroy the car upon arrival.
            if (overrideTarget.GetComponent<ExitNode>() != null)
            {
                if (distOverride < 0.5f)
                {
                    Destroy(gameObject);
                    return;
                }
            }
            else if (distOverride < 0.5f)
            {
                overrideTarget = null;
            }
            return;
        }

        if (currentWaypointIndex < waypoints.Count)
        {
            Transform target = waypoints[currentWaypointIndex];
            float dist = Vector3.Distance(transform.position, target.position);

            // Handle ExitNode: move toward it and destroy the car upon arrival.
            if (target.GetComponent<ExitNode>() != null)
            {
                MoveTowards(target);
                if (dist < 0.5f)
                {
                    Destroy(gameObject);
                }
                return;
            }

            // Check if target is an IntersectionNode.
            IntersectionNode node = target.GetComponent<IntersectionNode>();
            if (node != null)
            {
                MoveTowards(target);

                if (dist < 0.5f)
                {
                    if (node.IsGreen)
                    {
                        if (!node.alwaysGreen && node.nextDestinations != null && node.nextDestinations.Count > 0)
                        {
                            int randomIndex = Random.Range(0, node.nextDestinations.Count);
                            overrideTarget = node.nextDestinations[randomIndex];
                        }
                        currentWaypointIndex++;
                    }
                    else
                    {
                        // Stay at the node if red.
                        transform.position = target.position;
                    }
                }
            }
            else
            {
                // Regular waypoint.
                MoveTowards(target);
                if (dist < 0.5f)
                {
                    currentWaypointIndex++;
                }
            }
        }
    }

    void MoveTowards(Transform target)
    {
        // Determine where to start the detection sphere.
        Vector3 detectionOrigin = transform.position + transform.TransformDirection(detectionOriginOffset);

        // Use OverlapSphere to check if any other car is within the safe zone.
        Collider[] hits = Physics.OverlapSphere(detectionOrigin, detectionRadius);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject != this.gameObject && hit.GetComponent<CarController>() != null)
            {
                // Another car is too close—do not move forward.
                return;
            }
        }

        // If no other car is detected in the safe area, accelerate.
        currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);

        Vector3 moveDirection = (target.position - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, target.position, currentSpeed * Time.deltaTime);
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            targetRotation *= Quaternion.Euler(rotationOffset);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    // Visualize the detection sphere using gizmos.
    void OnDrawGizmos()
    {
        // Draw a blue sphere at the car's pivot.
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.2f);

        // Calculate the detection sphere origin.
        Vector3 detectionOrigin = transform.position + transform.TransformDirection(detectionOriginOffset);

        // Draw a green sphere at the detection origin.
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(detectionOrigin, 0.2f);

        // Draw a red wire sphere representing the detection radius.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionOrigin, detectionRadius);
    }
}
