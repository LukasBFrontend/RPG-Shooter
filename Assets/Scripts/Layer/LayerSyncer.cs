using System;
using UnityEngine;

public class LayerSyncer : MonoBehaviour
{
    [SerializeField] GameObject rootObject;
    [SerializeField] GameObject[] synchObjects;
    [Header("Syncs the layer of the active player instance")]
    [SerializeField] bool synchPlayer;

    void Start()
    {
        if (synchPlayer)
        {
            int _newLength = synchObjects.Length + 1;
            Array.Resize(ref synchObjects, _newLength);
            synchObjects[_newLength - 1] = GameState.Player.gameObject;
        }
    }

    void Update()
    {
        int _layer = rootObject.layer;
        foreach (GameObject obj in synchObjects)
        {
            obj.layer = _layer;
        }
    }
}
