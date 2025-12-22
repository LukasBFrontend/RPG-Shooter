using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BodyCollider : MonoBehaviour
{
    public Collider2D Collider => GetComponent<Collider2D>();
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.TryGetComponent<BodyCollider>(out var bodyCollider))
        {
            return;
        }

        Physics2D.IgnoreCollision(bodyCollider.Collider, this.Collider);
    }
}
