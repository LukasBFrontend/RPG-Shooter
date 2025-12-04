using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NPC : MonoBehaviour
{
    [HideInInspector] public Vector2 FaceDir = Vector2.down;
    public int Health = 100;
    public BoxCollider2D HitCollider;

    public void TakeDamage(int damage)
    {
        Health -= damage;
        Utils.Flicker(GetComponent<SpriteRenderer>(), 4, .25f);
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }

    public void UpdateRotation()
    {
        float _angle = Mathf.Atan2(FaceDir.y, FaceDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new(0, 0, _angle + 90));
    }
}
