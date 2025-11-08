using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NPC : MonoBehaviour
{
    public int health = 100;
    public BoxCollider2D hitCollider;
    [HideInInspector] public Vector2 faceDir = Vector2.down;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Visuals.Instance.Flicker(GetComponent<SpriteRenderer>(), 4, .25f);
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }

    public void UpdateRotation()
    {
        float angle = Mathf.Atan2(faceDir.y, faceDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new(0, 0, angle + 90));
    }
}
