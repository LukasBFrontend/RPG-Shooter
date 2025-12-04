using UnityEngine;
using System.Collections.Generic;

public class PixelateLayerManager : Singleton<PixelateLayerManager>
{
    [SerializeField]
    List<string> pixelateLayers = new()
    {
        "PixelateSprite0",
        "PixelateSprite1",
        "PixelateSprite2",
        "PixelateSprite3",
    };
    Dictionary<int, GameObject> usedLayers = new();

    void Update()
    {
        UnAssignUnusedLayers();
    }

    public void UnAssignUnusedLayers()
    {
        foreach (var kvp in usedLayers)
        {
            if (kvp.Value == null)
            {
                usedLayers.Remove(kvp.Key);
                break;
            }
        }
    }

    public int AssignUnusedLayer(GameObject obj)
    {
        for (int i = 0; i < pixelateLayers.Count; i++)
        {
            int _layer = LayerMask.NameToLayer(pixelateLayers[i]);
            if (_layer == -1)
            {
                Debug.LogWarning($"Layer '{pixelateLayers[i]}' does not exist in Tags & Layers.");
                continue;
            }

            if (!usedLayers.ContainsKey(_layer))
            {
                obj.layer = _layer;
                usedLayers[_layer] = obj;
                return _layer;
            }
        }

        Debug.LogWarning("No unused pixelate layers available.");
        return -1;
    }

    public void ReleaseLayer(GameObject obj)
    {
        foreach (var kvp in usedLayers)
        {
            if (kvp.Value == obj)
            {
                usedLayers.Remove(kvp.Key);
                obj.layer = 0;
                break;
            }
        }
    }
}
