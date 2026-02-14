using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(State))]
[RequireComponent(typeof(Movement))]
public class Character : MonoBehaviour
{
    [System.Serializable]
    struct CharacterFields
    {
        public SpriteRenderer SpriteRenderer;
        public Animator Animator;
        public Rigidbody2D Rigidbody;
        public Collider2D Collider;
        [Header("Stats")]
        public int Health;
        public bool UsesZDepth;
    }
    [SerializeField] CharacterFields characterFields;
    public int Health
    {
        get { return characterFields.Health; }
        private set { characterFields.Health = value; }
    }
    public bool UsesZDept
    {
        get { return characterFields.UsesZDepth; }
    }
    public SpriteRenderer SpriteRenderer
    {
        get { return characterFields.SpriteRenderer; }
    }
    public Animator Animator
    {
        get { return characterFields.Animator; }
    }
    public Rigidbody2D Rigidbody
    {
        get { return characterFields.Rigidbody; }
    }
    public Collider2D Collider
    {
        get { return characterFields.Collider; }
    }
    public State State { get { return GetComponent<State>(); } }
    public Movement Movement { get { return GetComponent<Movement>(); } }
    public Vector2 FaceDir { get; private set; } = Vector2.down;
    public List<string> CollisionIgnoreTags { get; set; }
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

    public void ResetHealth()
    {
        Health = _maxHealth;
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

    public void SetFacing(Vector2 direction)
    {
        float _rotationDegrees = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float _rotationDegreesSnapped = Mathf.Round(_rotationDegrees / 45f) * 45f;
        while (_rotationDegreesSnapped < 0)
        {
            _rotationDegreesSnapped += 360;
        }
        float _rotationRadians = _rotationDegreesSnapped * Mathf.Deg2Rad;

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
