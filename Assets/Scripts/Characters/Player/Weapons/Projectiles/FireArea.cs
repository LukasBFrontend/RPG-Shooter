using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FireArea : MonoBehaviour
{
    public List<NPC> TargetsInRange = new();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.TryGetComponent<NPC>(out var _enemy))
        {
            return;
        }

        TargetsInRange.Add(_enemy);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.TryGetComponent<NPC>(out var _enemy))
        {
            return;
        }

        TargetsInRange.Remove(_enemy);
    }
}
