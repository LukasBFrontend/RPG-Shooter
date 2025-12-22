using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] Collider2D col;
    [SerializeField] int damage = 5;

    void OnCollisionEnter2D(Collision2D other)
    {
        Collider2D _col = other.collider;
        bool _cancelCollision = Utils.PlayerTags.Contains(_col.tag);

        if (_cancelCollision)
        {
            Physics2D.IgnoreCollision(_col, col);
            return;
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<NPC>(out var _enemy))
        {
            _enemy.TakeDamage(damage);
        }
        else
        {
            return;
        }

        Destroy(gameObject);
    }
}
