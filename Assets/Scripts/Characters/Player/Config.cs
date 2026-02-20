using UnityEngine;

[RequireComponent(typeof(Player))]
public class Config : Singleton<Config>
{
    public Rigidbody2D Rigidbody;
    public Collider2D Collider;
    public Animator Animator;
    public SpriteRenderer SpriteRenderer { get { return _player.SpriteRenderer; } }
    Player _player;

    void Start()
    {
        _player = GetComponent<Player>();
    }
    public Vector2 ColliderCenter()
    {
        return Collider.bounds.center;
    }

    public Vector2 SpriteCenter()
    {
        return SpriteRenderer.bounds.center;
    }

    protected override void OnAwake()
    {

    }
}
