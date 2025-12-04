using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(MeshRenderer))]
public class QuadLayerSetter : MonoBehaviour
{
    public enum LayerTrackingMode
    {
        Sprite,
        ParticleSystem
    }
    public LayerTrackingMode TrackingMode = LayerTrackingMode.Sprite;
    public Renderer RendererToTrack;
    public int SortingLayerID;
    public int SortingOrder;

    void OnValidate()
    {
        Apply();
    }

    void Update()
    {
        Sync();
    }
    void Apply()
    {
        if (TryGetComponent<MeshRenderer>(out var _renderer))
        {
            _renderer.sortingLayerID = SortingLayerID;
            _renderer.sortingOrder = SortingOrder;
        }
    }

    public void Sync()
    {
        if (SortingLayerID == RendererToTrack.sortingLayerID && SortingOrder == RendererToTrack.sortingOrder)
        {
            return;
        }
        SortingLayerID = RendererToTrack.sortingLayerID;
        SortingOrder = RendererToTrack.sortingOrder;

        Apply();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(QuadLayerSetter))]
public class SortingLayerSetterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var _script = (QuadLayerSetter)target;
        var _renderer = _script.GetComponent<MeshRenderer>();

        var _sortingLayers = SortingLayer.layers;
        string[] _layerNames = new string[_sortingLayers.Length];
        int[] _layerIDs = new int[_sortingLayers.Length];

        for (int i = 0; i < _sortingLayers.Length; i++)
        {
            _layerNames[i] = SortingLayer.IDToName(_sortingLayers[i].id);
            _layerIDs[i] = _sortingLayers[i].id;
        }

        int _currentIndex = System.Array.IndexOf(_layerIDs, _renderer.sortingLayerID);

        if (_currentIndex < 0)
        {
            _currentIndex = 0;
        }

        int _newIndex = EditorGUILayout.Popup("Sorting Layer", _currentIndex, _layerNames);

        if (_newIndex != _currentIndex)
        {
            Undo.RecordObject(_script, "Change Sorting Layer");
            _script.GetComponent<MeshRenderer>().sortingLayerID = _layerIDs[_newIndex];
            _script.GetType().GetField("sortingLayerID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_script, _layerIDs[_newIndex]);
        }

        int _newOrder = EditorGUILayout.IntField("Order in Layer", _renderer.sortingOrder);
        if (_newOrder != _renderer.sortingOrder)
        {
            Undo.RecordObject(_script, "Change Sorting Order");
            _renderer.sortingOrder = _newOrder;
            _script.GetType().GetField("sortingOrder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_script, _newOrder);
        }
    }
}
#endif
