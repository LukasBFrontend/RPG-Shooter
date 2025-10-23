using UnityEngine;
using UnityEngine.Tilemaps;

public class ColliderMap : MonoBehaviour
{
    [SerializeField] Color runtimeColor;
    Tilemap tilemap;
    void Start()
    {
        tilemap = gameObject.GetComponent<Tilemap>();

        tilemap.color = runtimeColor;
    }
}
