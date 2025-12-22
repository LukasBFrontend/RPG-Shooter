using UnityEngine;

[RequireComponent(typeof(Movement))]
public class Player : Character
{
    public static Config Config { get; private set; }
    public static Movement Movement { get; private set; }
    public static Actions Actions { get; private set; }

    public static Vector2 LastValidRespawn;

    void Start()
    {
        Cache();
        SetMaxHealth(Health);
    }

    void Cache()
    {
        Config = Config.Instance;
        Actions = Actions.Instance;
        Movement = GetComponent<Movement>();

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

}
