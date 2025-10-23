using UnityEngine;
using System.Collections.Generic;

public class PixelateLayerManager : Singleton<PixelateLayerManager>
{
    [SerializeField]
    private List<string> pixelateLayers = new List<string>()
    {
        "PixelateSprite0",
        "PixelateSprite1",
        "PixelateSprite2",
        "PixelateSprite3",
        // Add more as needed
    };

    private Dictionary<int, GameObject> usedLayers = new Dictionary<int, GameObject>();

    /// <summary>
    /// Assigns the first unused pixelate layer to the object (not recursively).
    /// Returns the assigned layer number, or -1 if none available.
    /// </summary>
    public int AssignUnusedLayer(GameObject obj)
    {
        for (int i = 0; i < pixelateLayers.Count; i++)
        {
            int layer = LayerMask.NameToLayer(pixelateLayers[i]);
            if (layer == -1)
            {
                Debug.LogWarning($"Layer '{pixelateLayers[i]}' does not exist in Tags & Layers.");
                continue;
            }

            if (!usedLayers.ContainsKey(layer))
            {
                obj.layer = layer;
                usedLayers[layer] = obj;
                return layer;
            }
        }

        Debug.LogWarning("No unused pixelate layers available.");
        return -1;
    }

    /// <summary>
    /// Clears the layer assignment and frees the layer for reuse.
    /// Only resets the layer of the root object.
    /// </summary>
    public void ReleaseLayer(GameObject obj)
    {
        foreach (var kvp in usedLayers)
        {
            if (kvp.Value == obj)
            {
                usedLayers.Remove(kvp.Key);
                obj.layer = 0; // Reset to Default
                break;
            }
        }
    }
}
