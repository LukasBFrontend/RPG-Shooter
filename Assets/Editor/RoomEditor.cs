using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Room))]
public class RoomEditor : Editor
{
    SerializedProperty _sizeProp;
    const float MIN_SIZE = 0.5f;
    bool _showHandles = true;

    void OnEnable()
    {
        _sizeProp = serializedObject.FindProperty("Size");
        SceneView.duringSceneGui += DrawHandles;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= DrawHandles;
    }

    void DrawHandles(SceneView sceneView)
    {
        if (!_showHandles)
        {
            return;
        }

        Room _room = (Room)target;
        if (_room == null)
        {
            return;
        }

        Transform t = _room.transform;
        serializedObject.Update();
        Vector2 size = _sizeProp.vector2Value;
        Vector3 pos = t.position;

        float halfW = size.x * 0.5f;
        float halfH = size.y * 0.5f;

        EditorGUI.BeginChangeCheck();

        // Edge handles
        Vector3 _worldLeft = t.TransformPoint(new Vector3(-halfW, 0, 0));
        Vector3 _worldRight = t.TransformPoint(new Vector3(halfW, 0, 0));
        Vector3 _worldBottom = t.TransformPoint(new Vector3(0, -halfH, 0));
        Vector3 _worldTop = t.TransformPoint(new Vector3(0, halfH, 0));

        float _handleSize = HandleUtility.GetHandleSize(pos) * 0.1f;

        _worldLeft = Handles.Slider(_worldLeft, -t.right, _handleSize, Handles.CubeHandleCap, 0);
        _worldRight = Handles.Slider(_worldRight, t.right, _handleSize, Handles.CubeHandleCap, 0);
        _worldBottom = Handles.Slider(_worldBottom, -t.up, _handleSize, Handles.CubeHandleCap, 0);
        _worldTop = Handles.Slider(_worldTop, t.up, _handleSize, Handles.CubeHandleCap, 0);

        if (EditorGUI.EndChangeCheck())
        {
            float newWidth = t.InverseTransformPoint(_worldRight).x - t.InverseTransformPoint(_worldLeft).x;
            float newHeight = t.InverseTransformPoint(_worldTop).y - t.InverseTransformPoint(_worldBottom).y;

            // Snap & clamp
            newWidth = Mathf.Max(MIN_SIZE, Mathf.Round(newWidth * 2f) / 2f);
            newHeight = Mathf.Max(MIN_SIZE, Mathf.Round(newHeight * 2f) / 2f);

            Undo.RecordObject(_room, "Resize Room");
            _sizeProp.vector2Value = new Vector2(newWidth, newHeight);
            serializedObject.ApplyModifiedProperties();
        }

        // Center handle
    }
}
