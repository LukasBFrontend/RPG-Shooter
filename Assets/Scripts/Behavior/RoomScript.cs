using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class Room : MonoBehaviour
{
    public Vector2 size = new Vector2(4f, 2f);
    [SerializeField] bool gizmosEnabled = true;

    void OnValidate()
    {
        size.x = Mathf.Max(0.5f, size.x);
        size.y = Mathf.Max(0.5f, size.y);
    }

    private void OnDrawGizmos()
    {
        if (!gizmosEnabled || (size.x <= 0 || size.y <= 0)) return;

        // Semi-transparent red for visibility
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);

        Vector3 pos = transform.position;
        float halfW = size.x * 0.5f;
        float halfH = size.y * 0.5f;

        // Draw rectangle edges
        Color color = Color.yellow;
        const float width = 5;
        Vector3 topLeft = pos + transform.TransformVector(new Vector3(-halfW, halfH, 0));
        Vector3 topRight = pos + transform.TransformVector(new Vector3(halfW, halfH, 0));
        Vector3 bottomRight = pos + transform.TransformVector(new Vector3(halfW, -halfH, 0));
        Vector3 bottomLeft = pos + transform.TransformVector(new Vector3(-halfW, -halfH, 0));

        Handles.DrawBezier(topLeft, topRight, topLeft, topRight, color, null, width);
        Handles.DrawBezier(topRight, bottomRight, topRight, bottomRight, color, null, width);
        Handles.DrawBezier(bottomRight, bottomLeft, bottomRight, bottomLeft, color, null, width);
        Handles.DrawBezier(bottomLeft, topLeft, bottomLeft, topLeft, color, null, width);

        // Draw cross through center
        Vector3 center = pos;
        // Horizontal line
        /*         Gizmos.DrawLine(center + transform.TransformVector(new Vector3(-halfW, 0, 0)),
                                center + transform.TransformVector(new Vector3(halfW, 0, 0))); */

        Handles.DrawBezier(center + transform.TransformVector(new Vector3(-halfW, 0, 0)), center + transform.TransformVector(new Vector3(halfW, 0, 0)), center + transform.TransformVector(new Vector3(-halfW, 0, 0)), center + transform.TransformVector(new Vector3(halfW, 0, 0)), color, null, width);

        // Vertical line
        /*         Gizmos.DrawLine(center + transform.TransformVector(new Vector3(0, -halfH, 0)),
                                center + transform.TransformVector(new Vector3(0, halfH, 0))); */

        Handles.DrawBezier(center + transform.TransformVector(new Vector3(0, -halfH, 0)), center + transform.TransformVector(new Vector3(0, halfH, 0)), center + transform.TransformVector(new Vector3(0, -halfH, 0)), center + transform.TransformVector(new Vector3(0, halfH, 0)), color, null, width);
    }
}
