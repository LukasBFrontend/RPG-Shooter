using UnityEngine;

public enum PlayerStatus
{
    None,
    Recoil,
    Knockback
}
public class PlayerConfig : Singleton<PlayerConfig>
{
    [SerializeField] PlayerStatus status = PlayerStatus.None;
    public PlayerStatus Status
    {
        get => status;
        set
        {
            status = value;

            if (status != PlayerStatus.None)
            {
                _statusTimer = 0.5f;
            }

        }
    }
    [HideInInspector] public int Layer;
    public Rigidbody2D Rb;
    public Collider2D Col;
    public SpriteRenderer SpriteRenderer;
    public Animator Animator;
    float _statusTimer = 0f;

    public Node Node()
    {
        return NodeManager.Instance.ClosestNode(transform.position);
    }
    public Vector2 ColliderCenter()
    {
        return Col.bounds.center;
    }

    void Update()
    {
        if (_statusTimer > 0f)
        {
            _statusTimer -= Time.deltaTime;
            return;
        }

        if (Status != PlayerStatus.None)
        {
            Status = PlayerStatus.None;
        }
    }
}
