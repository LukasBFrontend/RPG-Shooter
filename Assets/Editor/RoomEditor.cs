using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Room))]
public class RoomEditor : Editor
{
    SerializedProperty sizeProp;
    const float minSize = 0.5f;
    bool showHandles = true;

    void OnEnable()
    {
        sizeProp = serializedObject.FindProperty("size");
        SceneView.duringSceneGui += DrawHandles;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= DrawHandles;
    }

    /*     public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Room Size (Width × Height)", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(sizeProp);

            showHandles = EditorGUILayout.Toggle("Show Scene Handles", showHandles);

            serializedObject.ApplyModifiedProperties();
        } */

    void DrawHandles(SceneView sceneView)
    {
        if (!showHandles) return;

        Room room = (Room)target;
        if (room == null) return;

        Transform t = room.transform;
        serializedObject.Update();
        Vector2 size = sizeProp.vector2Value;
        Vector3 pos = t.position;

        float halfW = size.x * 0.5f;
        float halfH = size.y * 0.5f;

        EditorGUI.BeginChangeCheck();

        // Edge handles
        Vector3 worldLeft = t.TransformPoint(new Vector3(-halfW, 0, 0));
        Vector3 worldRight = t.TransformPoint(new Vector3(halfW, 0, 0));
        Vector3 worldBottom = t.TransformPoint(new Vector3(0, -halfH, 0));
        Vector3 worldTop = t.TransformPoint(new Vector3(0, halfH, 0));

        float handleSize = HandleUtility.GetHandleSize(pos) * 0.1f;

        worldLeft = Handles.Slider(worldLeft, -t.right, handleSize, Handles.CubeHandleCap, 0);
        worldRight = Handles.Slider(worldRight, t.right, handleSize, Handles.CubeHandleCap, 0);
        worldBottom = Handles.Slider(worldBottom, -t.up, handleSize, Handles.CubeHandleCap, 0);
        worldTop = Handles.Slider(worldTop, t.up, handleSize, Handles.CubeHandleCap, 0);

        if (EditorGUI.EndChangeCheck())
        {
            float newWidth = t.InverseTransformPoint(worldRight).x - t.InverseTransformPoint(worldLeft).x;
            float newHeight = t.InverseTransformPoint(worldTop).y - t.InverseTransformPoint(worldBottom).y;

            // Snap & clamp
            newWidth = Mathf.Max(minSize, Mathf.Round(newWidth * 2f) / 2f);
            newHeight = Mathf.Max(minSize, Mathf.Round(newHeight * 2f) / 2f);

            Undo.RecordObject(room, "Resize Room");
            sizeProp.vector2Value = new Vector2(newWidth, newHeight);
            serializedObject.ApplyModifiedProperties();
        }

        // Center handle
    }
}
