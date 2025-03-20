using UnityEngine;

public class ExitNode : MonoBehaviour
{
    // Draw a red sphere to mark the exit point.
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
