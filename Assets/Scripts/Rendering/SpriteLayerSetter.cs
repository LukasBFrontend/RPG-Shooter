using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteLayerSetter : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject objectToSync;
    [Header("Optional")]
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

        switch (trackedLayer)
        {
            case 16:
                spriteRenderer.sortingLayerName = "BottomWall";
                SetSelfLight(0);
                break;
            case 17:
                spriteRenderer.sortingLayerName = "MiddleWall";
                SetSelfLight(1);
                break;
            case 18:
                spriteRenderer.sortingLayerName = "UpperWall";
                SetSelfLight(2);
                break;
            default:
                return;
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
