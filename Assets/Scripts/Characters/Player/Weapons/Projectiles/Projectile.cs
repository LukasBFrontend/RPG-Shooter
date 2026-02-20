using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] Collider2D col;
    [SerializeField] float lifetime = 1.5f;
    public Character Sender { get; set; }
    public int Damage { get; set; } = 1;

    void Awake()
    {
        StartCoroutine(DestroyAfterSeconds(lifetime));
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Collider2D _col = other.collider;

        if (Sender.CollisionIgnoreTags.Contains(_col.tag))
        {
            Physics2D.IgnoreCollision(_col, col);
            return;
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!IgnoreCollision(other) && other.TryGetComponent<Character>(out var _character))
        {
            _character.TakeDamage(Damage);
        }
        else
        {
            return;
        }

        Destroy(gameObject);
    }

    bool IgnoreCollision(Collider2D other)
    {
        if (Sender is Player)
        {
            return Sender.CollisionIgnoreTags.Contains(other.tag);
        }
        else if (Sender is NPC npc)
        {
            return npc.RaycastIgnore.Contains(other);
        }
        return false;
    }

    IEnumerator DestroyAfterSeconds(float seconds)
    {
        float _elapsed = 0f;

        while (_elapsed < seconds)
        {
            yield return new WaitForEndOfFrame();
            _elapsed += Time.deltaTime;
        }

        Destroy(gameObject);
    }
}
