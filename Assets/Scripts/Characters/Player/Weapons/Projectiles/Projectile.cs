using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] Collider2D col;
    [SerializeField] bool destroyOnCollision = true;
    float _lifetime;
    public Character Sender { get; set; }
    public int Damage { get; set; } = 1;
    public float Lifetime { get { return _lifetime; } }

    public void SetLifeTime(float lifetime)
    {
        _lifetime = lifetime;
        StartCoroutine(DestroyAfterSeconds(lifetime));
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Collider2D _col = other.collider;
        List<string> _collisionIgnoreTags = Sender.CollisionIgnoreTags;

        if (
            _col.CompareTag("Player") || _col.CompareTag("Enemy")
        )
        {
            Physics2D.IgnoreCollision(_col, col);
            return;
        }
        else if (!destroyOnCollision)
        {
            return;
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<BodyTrigger>(out var body) && body.DamageableBy(Sender))
        {
            body.Character.TakeDamage(Damage);
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
