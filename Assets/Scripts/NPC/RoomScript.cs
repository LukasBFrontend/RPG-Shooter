using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class Room : MonoBehaviour
{
    [SerializeField] bool gizmosEnabled = true;
    public Vector2 Size = new(4f, 2f);
    public Room[] ConnectedRooms;

    void OnValidate()
    {
        Size.x = Mathf.Max(0.5f, Size.x);
        Size.y = Mathf.Max(0.5f, Size.y);
    }

    void OnDrawGizmos()
    {
        if (!gizmosEnabled || Size.x <= 0 || Size.y <= 0)
        {
            return;
        }

        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);

        Vector3 _pos = transform.position;
        float _halfW = Size.x * 0.5f;
        float _halfH = Size.y * 0.5f;

        Color _color = Color.yellow;
        const float WIDTH = 5;
        Vector3 _topLeft = _pos + transform.TransformVector(new Vector3(-_halfW, _halfH, 0));
        Vector3 _topRight = _pos + transform.TransformVector(new Vector3(_halfW, _halfH, 0));
        Vector3 _bottomRight = _pos + transform.TransformVector(new Vector3(_halfW, -_halfH, 0));
        Vector3 _bottomLeft = _pos + transform.TransformVector(new Vector3(-_halfW, -_halfH, 0));

        Handles.DrawBezier(_topLeft, _topRight, _topLeft, _topRight, _color, null, WIDTH);
        Handles.DrawBezier(_topRight, _bottomRight, _topRight, _bottomRight, _color, null, WIDTH);
        Handles.DrawBezier(_bottomRight, _bottomLeft, _bottomRight, _bottomLeft, _color, null, WIDTH);
        Handles.DrawBezier(_bottomLeft, _topLeft, _bottomLeft, _topLeft, _color, null, WIDTH);

        Vector3 _center = _pos;

        Handles.DrawBezier(_center + transform.TransformVector(new Vector3(-_halfW, 0, 0)), _center + transform.TransformVector(new Vector3(_halfW, 0, 0)), _center + transform.TransformVector(new Vector3(-_halfW, 0, 0)), _center + transform.TransformVector(new Vector3(_halfW, 0, 0)), _color, null, WIDTH);

        Handles.DrawBezier(_center + transform.TransformVector(new Vector3(0, -_halfH, 0)), _center + transform.TransformVector(new Vector3(0, _halfH, 0)), _center + transform.TransformVector(new Vector3(0, -_halfH, 0)), _center + transform.TransformVector(new Vector3(0, _halfH, 0)), _color, null, WIDTH);
    }
}
