using UnityEngine;

public class ObjectCollider : MonoBehaviour
{
    public Collider2D Collider => GetComponent<Collider2D>();
    public bool IsJumping { get; set; } = false;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.TryGetComponent<BodyCollider>(out var bodyCollider) && bodyCollider.IsJumping)
        {
            return;
        }

        Physics2D.IgnoreCollision(bodyCollider.Collider, Collider);
    }
}
