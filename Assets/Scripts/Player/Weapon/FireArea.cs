using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireArea : MonoBehaviour
{
    [SerializeField] string[] enemyTags = { "Enemy" };
    [SerializeField] int damage = 50;
    public List<NPC> Targets { get; } = new();

    public void ExecuteAttack()
    {
        foreach (NPC enemy in Targets)
        {
            enemy.TakeDamage(damage);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!enemyTags.Contains(other.tag) || !other.gameObject.TryGetComponent<NPC>(out var enemy))
        {
            return;
        }

        Targets.Add(enemy);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!enemyTags.Contains(other.tag) || !other.gameObject.TryGetComponent<NPC>(out var enemy))
        {
            return;
        }

        Targets.Remove(enemy);
    }
}
