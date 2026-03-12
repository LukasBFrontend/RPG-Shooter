using UnityEngine;

[RequireComponent(typeof(Controller))]
[RequireComponent(typeof(Actions))]
[RequireComponent(typeof(Inventory))]
[RequireComponent(typeof(AimController))]
public class Player : Character
{
    public Controller Movement { get; private set; }
    public Actions Actions { get; private set; }
    public Inventory Inventory { get; private set; }
    public AimController AimController { get; private set; }
    public Vector2 LastValidRespawn;

    void Start()
    {
        Cache();
        SetMaxHealth(Health);
    }

    void Cache()
    {
        Movement = GetComponent<Controller>();
        Actions = GetComponent<Actions>();
        Inventory = GetComponent<Inventory>();
        AimController = GetComponent<AimController>();

        CollisionIgnoreTags = Utils.PlayerTags;
        OnDeath = () =>
        {
            Respawn();
        };

        LastValidRespawn = transform.position;
    }

    public void Respawn()
    {
        ResetHealth();
        transform.position = LastValidRespawn;

        Utils.FlickerSprite(SpriteRenderer, new Color(1, 1, 1, 0), 6, .5f);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.collider.TryGetComponent<Character>(out var character))
        {
            return;
        }
        Debug.Log($"Ignore collision between {Collider.name} and {other.collider.name}");

        Physics2D.IgnoreCollision(Collider, other.collider);
    }
}
