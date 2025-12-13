using System.Collections.Generic;
using UnityEngine;

public class DebugGizmos : MonoBehaviour
{
    public Collider2D inner;
    public Collider2D outer;

    private void OnDrawGizmos()
    {
        if (inner != null)
        {
            List<Vector2> innerPoints = Utils.GetSamplePoints(inner);
            Gizmos.color = Color.red;
            foreach (var p in innerPoints)
                Gizmos.DrawSphere(p, 0.05f);
        }

        if (outer != null)
        {
            List<Vector2> outerPoints = Utils.GetSamplePoints(outer);
            Gizmos.color = Color.green;
            foreach (var p in outerPoints)
                Gizmos.DrawSphere(p, 0.05f);
        }
    }
}
