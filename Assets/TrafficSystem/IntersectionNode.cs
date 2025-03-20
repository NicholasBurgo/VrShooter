using UnityEngine;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class IntersectionNode : MonoBehaviour
{
    [HideInInspector]
    public bool canGo = false;

    [Tooltip("Always Green: when true, this node will always be considered green regardless of the IntersectionManager.")]
    public bool alwaysGreen = false;

    [Tooltip("List of possible next destinations for a car at this intersection. A random destination will be chosen if more than one is provided.")]
    public List<Transform> nextDestinations;

    // Property to determine if this node is green.
    public bool IsGreen
    {
        get { return alwaysGreen ? true : canGo; }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = IsGreen ? Color.green : Color.blue;
        Gizmos.DrawSphere(transform.position, 0.5f);

        if (nextDestinations != null && nextDestinations.Count > 0)
        {
            Gizmos.color = Color.magenta;
            foreach (Transform dest in nextDestinations)
            {
                if (dest != null)
                    Gizmos.DrawLine(transform.position, dest.position);
            }
        }
    }
}
