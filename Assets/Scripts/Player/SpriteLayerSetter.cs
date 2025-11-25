using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteLayerSetter : MonoBehaviour
{
    [SerializeField] GameObject objectToSync;
    [Header("Optional")]
    [SerializeField] GameObject[] zLights;
    SpriteRenderer sprite;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();

    }

    public void Synch()
    {

        switch (gameObject.layer)
        {
            case 16:
                sprite.sortingLayerName = "BottomWall";
                SetSelfLight(0);
                break;
            case 17:
                sprite.sortingLayerName = "MiddleWall";
                SetSelfLight(1);
                break;
            case 18:
                sprite.sortingLayerName = "UpperWall";
                SetSelfLight(2);
                break;
            default:
                return;
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

    void Update()
    {
        if (objectToSync.layer == gameObject.layer)
        {
            return;
        }

        gameObject.layer = objectToSync.layer;
        Synch();
    }
}
