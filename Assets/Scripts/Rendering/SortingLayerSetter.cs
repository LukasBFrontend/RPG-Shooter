using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum LayerTrackingMode
{
    Sprite,
    ParticleSystem
}

[RequireComponent(typeof(MeshRenderer))]
public class QuadLayerSetter : MonoBehaviour
{
    public LayerTrackingMode trackingMode = LayerTrackingMode.Sprite;
    public Renderer rendererToTrack;
    public int sortingLayerID;
    public int sortingOrder;

    void OnValidate() => Apply();


    void Update()
    {
        Sync();
    }
    private void Apply()
    {
        if (TryGetComponent<MeshRenderer>(out var renderer))
        {
            renderer.sortingLayerID = sortingLayerID;
            renderer.sortingOrder = sortingOrder;
        }
    }

    public void Sync()
    {
        if (sortingLayerID == rendererToTrack.sortingLayerID && sortingOrder == rendererToTrack.sortingOrder)
        {
            return;
        }
        sortingLayerID = rendererToTrack.sortingLayerID;
        sortingOrder = rendererToTrack.sortingOrder;


        Apply();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(QuadLayerSetter))]
public class SortingLayerSetterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var script = (QuadLayerSetter)target;
        var renderer = script.GetComponent<MeshRenderer>();

        // Build sorting layer list
        var sortingLayers = SortingLayer.layers;
        string[] layerNames = new string[sortingLayers.Length];
        int[] layerIDs = new int[sortingLayers.Length];
        for (int i = 0; i < sortingLayers.Length; i++)
        {
            layerNames[i] = SortingLayer.IDToName(sortingLayers[i].id);
            layerIDs[i] = sortingLayers[i].id;
        }

        // Current index
        int currentIndex = System.Array.IndexOf(layerIDs, renderer.sortingLayerID);
        if (currentIndex < 0) currentIndex = 0;

        // Dropdown
        int newIndex = EditorGUILayout.Popup("Sorting Layer", currentIndex, layerNames);
        if (newIndex != currentIndex)
        {
            Undo.RecordObject(script, "Change Sorting Layer");
            script.GetComponent<MeshRenderer>().sortingLayerID = layerIDs[newIndex];
            script.GetType().GetField("sortingLayerID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(script, layerIDs[newIndex]);
        }

        // Order in Layer
        int newOrder = EditorGUILayout.IntField("Order in Layer", renderer.sortingOrder);
        if (newOrder != renderer.sortingOrder)
        {
            Undo.RecordObject(script, "Change Sorting Order");
            renderer.sortingOrder = newOrder;
            script.GetType().GetField("sortingOrder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(script, newOrder);
        }
    }
}
#endif
