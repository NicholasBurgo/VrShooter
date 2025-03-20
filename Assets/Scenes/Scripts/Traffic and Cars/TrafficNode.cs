using System.Collections.Generic;
using UnityEngine;

public class TrafficNode : MonoBehaviour
{
    public enum NodeType { Start, End, Intersection }

    [Header("Settings")]
    public NodeType nodeType;
    public float laneSwitchTime = 10f;
    public float stoppingDistance = 5f;

    [Header("Connections")]
    public Transform[] incomingLanes;
    public Transform[] outgoingLanes;

    [SerializeField] private int _currentActiveLane;
    private float _timer;
    public List<TrafficNode> connectedNodes;

    void Update()
    {
        if (nodeType != NodeType.Intersection) return;

        _timer += Time.deltaTime;
        if (_timer >= laneSwitchTime)
        {
            CycleLane();
            _timer = 0;
        }
    }

    public bool IsLaneGreen(Transform lane)
    {
        return outgoingLanes.Length > 0 &&
               outgoingLanes[_currentActiveLane] == lane;
    }

    private void CycleLane()
    {
        _currentActiveLane = (_currentActiveLane + 1) % outgoingLanes.Length;
    }

    public TrafficNode GetNextNode()
    {
        if (connectedNodes != null && connectedNodes.Count > 0)
        {
            return connectedNodes[0]; // or pick randomly
        }
        return null;
    }

    void OnDrawGizmos()
    {
        // Draw node type
        Gizmos.color = nodeType switch
        {
            NodeType.Start => Color.green,
            NodeType.End => Color.red,
            _ => Color.yellow
        };
        Gizmos.DrawWireSphere(transform.position, 1f);

        // Draw incoming and outgoing lanes only for intersections
        if (nodeType == NodeType.Intersection)
        {
            // Draw incoming lanes
            Gizmos.color = Color.blue;
            foreach (Transform lane in incomingLanes)
            {
                if (lane == null) continue;
                Gizmos.DrawLine(lane.position, transform.position);
            }

            // Draw outgoing lanes with traffic lights
            foreach (Transform lane in outgoingLanes)
            {
                if (lane == null) continue;

                // Lane state color
                Gizmos.color = IsLaneGreen(lane) ? Color.green : Color.red;

                // Main lane line
                Gizmos.DrawLine(transform.position, lane.position);

                // Stopping distance indicator
                Vector3 stopPoint = transform.position +
                    (lane.position - transform.position).normalized * stoppingDistance;
                Gizmos.DrawWireCube(stopPoint, Vector3.one * 0.5f);
            }
        }
    }
}