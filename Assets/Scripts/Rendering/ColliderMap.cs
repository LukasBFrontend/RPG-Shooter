using UnityEngine;
using UnityEngine.Tilemaps;

public class ColliderMap : MonoBehaviour
{
    [SerializeField] Color runtimeColor;
    Tilemap _tilemap;
    void Start()
    {
        _tilemap = gameObject.GetComponent<Tilemap>();

        _tilemap.color = runtimeColor;
    }
}
