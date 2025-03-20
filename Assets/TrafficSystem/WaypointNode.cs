using UnityEngine;
using System.Collections.Generic;

public class WaypointNode : MonoBehaviour
{
    [Tooltip("List of possible next waypoints. If more than one is assigned, one will be chosen at random.")]
    public List<WaypointNode> nextWaypoints;

    [Tooltip("If true, this node is an end (exit) node.")]
    public bool isEndNode = false;

    void OnDrawGizmos()
    {
        // End nodes are drawn in red; regular nodes in yellow.
        Gizmos.color = isEndNode ? Color.red : Color.yellow;
        float sphereSize = isEndNode ? 0.5f : 0.3f;
        Gizmos.DrawSphere(transform.position, sphereSize);

        // Optionally draw lines to each possible next waypoint.
        if (nextWaypoints != null && nextWaypoints.Count > 0)
        {
            Gizmos.color = Color.magenta;
            foreach (WaypointNode node in nextWaypoints)
            {
                if (node != null)
                    Gizmos.DrawLine(transform.position, node.transform.position);
            }
        }
    }
}

