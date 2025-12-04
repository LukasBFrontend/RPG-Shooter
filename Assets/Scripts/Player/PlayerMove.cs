using UnityEngine;

public class PlayerMove : Singleton<PlayerMove>
{
    [HideInInspector] public Vector2 Direction = Vector2.zero;
    [HideInInspector] public bool IsTurningRight, IsTurningLeft = false;
    public bool IsRecievingInput { get; set; }
    public float MoveSpeed = 4;
    Rigidbody2D _rb;
    Animator _animator;

    const float Deadzone = 0.1f;

    void Start()
    {
        _rb = PlayerConfig.Instance.Rb;
        _animator = PlayerConfig.Instance.Animator;
        _rb.gravityScale = 0;
    }

    void FixedUpdate()
    {
        SetVelocity();
        UpdateAnimator();
    }

    void SetVelocity()
    {
        if (PlayerConfig.Instance.Status != PlayerStatus.None)
        {
            return;
        }

        if (Direction.magnitude > 1)
        {
            Direction.Normalize();
        }

        Vector2 velocity = IsRecievingInput ?
            Direction * MoveSpeed :
            Vector2.zero;

        _rb.linearVelocity = velocity;
    }

    void UpdateAnimator()
    {
        if (!_animator)
        {
            return;
        }

        float _speed = Direction.magnitude;
        _animator.SetFloat("Speed", _speed);

        if (_speed > Deadzone)
        {
            Vector2 _dir = Direction.normalized;

            float _angle = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg;
            if (_angle < 0)
            {
                _angle += 360f;
            }

            float adjusted = (_angle + 22.5f) % 360f;

            int index = Mathf.FloorToInt(adjusted / 45f);

            _animator.SetFloat("DirectionIndex", (float)index);
        }
    }
}
