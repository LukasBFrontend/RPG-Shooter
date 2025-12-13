using System;
using UnityEngine;

public class Character : MonoBehaviour
{
    [System.Serializable]
    struct CharacterFields
    {
        public SpriteRenderer SpriteRenderer;
        public Rigidbody2D Rigidbody;
        public Collider2D Collider;
        public int Health;
    }
    [SerializeField] CharacterFields characterFields;
    public int Health
    {
        get { return characterFields.Health; }
        private set { characterFields.Health = value; }
    }
    public SpriteRenderer SpriteRenderer
    {
        get { return characterFields.SpriteRenderer; }
    }
    public Rigidbody2D Rigidbody
    {
        get { return characterFields.Rigidbody; }
    }
    public Collider2D Collider
    {
        get { return characterFields.Collider; }
    }
    public Vector2 FaceDir { get; private set; } = Vector2.down;
    public Action OnDeath { get; set; }
    int _maxHealth;

    public bool IsDamaged()
    {
        return Health < _maxHealth;
    }

    public void SetMaxHealth(int maxHealth)
    {
        _maxHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        Utils.FlickerSprite(SpriteRenderer, Color.red, 4, .25f);

        if (Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        OnDeath();
    }

    public void SetFacing(Vector2 point)
    {
        float _rotationDegrees = Mathf.Atan2(point.y, point.x) * Mathf.Rad2Deg;
        _rotationDegrees = Mathf.Round(_rotationDegrees / 45f) * 45f;
        float _rotationRadians = _rotationDegrees * Mathf.Deg2Rad;

        FaceDir = new(Mathf.Cos(_rotationRadians), Mathf.Sin(_rotationRadians));
    }

    public Vector2 ColliderCenter()
    {
        return Collider.bounds.center;
    }

    public Vector2 SpriteCenter()
    {
        return SpriteRenderer.bounds.center;
    }
}
