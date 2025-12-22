using UnityEngine;

public class LayerSyncer : MonoBehaviour
{
    [SerializeField] GameObject rootObject;
    [SerializeField] GameObject[] synchObjects;

    void Update()
    {
        int _layer = rootObject.layer;
        foreach (GameObject obj in synchObjects)
        {
            obj.layer = _layer;
        }
    }
}
