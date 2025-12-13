using UnityEngine;

[RequireComponent(typeof(Player))]
public class Movement : Singleton<Movement>
{
    [HideInInspector] public Vector2 Direction = Vector2.zero;
    [HideInInspector] public bool IsTurningRight, IsTurningLeft = false;
    public bool IsRecievingInput { get; set; }
    public float MoveSpeed { get; private set; } = 6;
    const float Deadzone = 0.1f;

    void FixedUpdate()
    {
        if (IsMovementEnabled())
        {
            SetVelocity();
        }
        UpdateAnimator();
    }

    void SetVelocity()
    {
        if (Player.State.Status != PlayerStatus.None && Player.State.Status != PlayerStatus.Falling)
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

        if (Player.State.Status == PlayerStatus.Falling)
        {
            velocity = Vector2.zero;
        }
        Player.Config.Rigidbody.linearVelocity = velocity;
    }

    void UpdateAnimator()
    {
        if (!Player.Config.Animator)
        {
            return;
        }

        float _speed = Direction.magnitude;
        Player.Config.Animator.SetFloat("Speed", _speed);

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

            Player.Config.Animator.SetFloat("DirectionIndex", (float)index);
        }
    }

    bool IsMovementEnabled()
    {
        switch (Player.State.Status)
        {
            case PlayerStatus.Recoil:
                return false;
            case PlayerStatus.Falling:
                return false;
            default:
                break;
        }

        return true;
    }
}
