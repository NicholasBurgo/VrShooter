using UnityEngine;
using System.Collections;

public class IntersectionManager : MonoBehaviour
{
    [Tooltip("The three lane nodes (IntersectionNodes) for this intersection.")]
    public IntersectionNode[] lanes; 

    [Tooltip("How long (in seconds) a lane stays active before switching.")]
    public float greenTime = 5f;

    void Start()
    {
        if (lanes.Length != 3)
        {
            Debug.LogError("IntersectionManager: Please assign exactly three IntersectionNode references.");
            return;
        }
        StartCoroutine(CycleLanes());
    }

    IEnumerator CycleLanes()
    {
        while (true)
        {
            for (int i = 0; i < lanes.Length; i++)
            {
                for (int j = 0; j < lanes.Length; j++)
                {
                    lanes[j].canGo = (j == i);
                }
                yield return new WaitForSeconds(greenTime);
            }
        }
    }

    // Draw lines to each assigned lane node.
    void OnDrawGizmos()
    {
        if (lanes == null)
            return;

        Gizmos.color = Color.cyan;
        foreach (IntersectionNode node in lanes)
        {
            if (node != null)
            {
                Gizmos.DrawLine(transform.position, node.transform.position);
            }
        }
    }
}
