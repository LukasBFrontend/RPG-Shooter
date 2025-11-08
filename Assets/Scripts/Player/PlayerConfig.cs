using UnityEngine;

public enum PlayerStatus
{
    None,
    Recoil,
    Knockback
}

public class PlayerConfig : Singleton<PlayerConfig>
{
    [SerializeField] private PlayerStatus _status = PlayerStatus.None;
    public PlayerStatus Status
    {
        get => _status;
        set
        {
            _status = value;

            if (_status != PlayerStatus.None)
            {
                statusTimer = 0.5f;
            }

        }
    }

    public Rigidbody2D Rb;
    public PolygonCollider2D Col;
    public SpriteRenderer SpriteRenderer;
    public Animator Animator;
    public Node Node;

    [HideInInspector] public int Layer;
    private GameObject mainCamera;
    private float statusTimer = 0f;

    void Start()
    {
        Cache();
    }

    public Vector2 ColliderCenter()
    {
        return Col.bounds.center;
    }

    void Update()
    {
        Node = NodeManager.Instance.ClosestNode(transform.position);

        if (statusTimer > 0f)
        {
            statusTimer -= Time.deltaTime;
            return;
        }

        if (Status != PlayerStatus.None)
        {
            Status = PlayerStatus.None;
        }
    }

    private void Cache()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }
}
