using UnityEngine;

public class SpriteLayerSetter : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject objectToSync;
    [Header("Optional")]
    [SerializeField] bool assignSortLayer = true;
    [SerializeField] bool syncObjectLayer = true;
    [SerializeField] GameObject[] zLights;
    int trackedLayer = 0;

    void Update()
    {
        if (objectToSync.layer == trackedLayer)
        {
            return;
        }

        Sync();
    }

    public void Sync()
    {
        trackedLayer = objectToSync.layer;

        if (assignSortLayer)
        {
            switch (trackedLayer)
            {
                case 16:
                    spriteRenderer.sortingLayerName = "Character A";
                    SetSelfLight(0);
                    break;
                case 17:
                    spriteRenderer.sortingLayerName = "Character B";
                    SetSelfLight(1);
                    break;
                case 18:
                    spriteRenderer.sortingLayerName = "Character C";
                    SetSelfLight(2);
                    break;
                default:
                    return;
            }
        }

        if (syncObjectLayer)
        {
            gameObject.layer = trackedLayer;
        }
    }

    void SetSelfLight(int zIndex)
    {
        if (zLights.Length <= 0)
        {
            return;
        }
        for (int i = 0; i < zLights.Length; i++)
        {
            zLights[i].SetActive(i == zIndex);
        }
    }
}
