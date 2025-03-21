using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AiNode : MonoBehaviour
{
    (int Nodid, int cost)[] nodes;
    [SerializeField]
    float radius = 10f;
    public List<Transform> nextDestinations;
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(transform.position, 0.2f);
        if (nextDestinations != null && nextDestinations.Count > 0)
        {
            Gizmos.color = Color.yellow;
            foreach (Transform dest in nextDestinations)
            {
                if (dest != null)
                    Gizmos.DrawLine(transform.position, dest.position);
            }
        }
        //each node will have a cost per distance/difficulty
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }


}
