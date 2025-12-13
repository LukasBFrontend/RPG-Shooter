using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class State : Singleton<State>
{

    [HideInInspector] public bool Alive = true;
    public PlayerStatus Status
    {
        get => status;
        set
        {
            status = value;

            if (status == PlayerStatus.Recoil || status == PlayerStatus.Knockback)
            {
                _statusTimer = 0.5f;
            }

        }
    }
    public int Hearts { get; private set; } = 3;
    public int Health { get { return _player.Health; } }
    PlayerStatus status = PlayerStatus.None;
    public Vector2 LastValidRespawn;
    float _statusTimer = 0f;
    Player _player;

    void Start()
    {
        _player = GetComponent<Player>();
        LastValidRespawn = transform.position;
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

    public bool IsDamaged()
    {
        return _player.IsDamaged();
    }

    public void Respawn()
    {
        transform.position = LastValidRespawn;

        Utils.FlickerSprite(Player.Config.SpriteRenderer, new Color(1, 1, 1, 0), 6, .5f);
    }

    public void SetStatusFalling(float duration)
    {
        _statusTimer = duration;

        Status = PlayerStatus.Falling;
    }
    public Node CurrentNode()
    {
        return NodeManager.Instance.ClosestNode(transform.position);
    }
}
