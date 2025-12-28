using UnityEngine;

[RequireComponent(typeof(Character))]
public class Movement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 6;
    [Header("Optional")]
    [SerializeField] Animator animator;
    public Vector2 Input { get; set; } = Vector2.zero;
    [Range(0, 5)]
    public float MovespeedMultiplier { get; set; } = 1f;
    const float DEADZONE = 0.1f;
    Character _character;

    void Start()
    {
        _character = GetComponent<Character>();
    }

    void FixedUpdate()
    {
        if (IsMovementEnabled())
        {
            SetVelocity();
        }
        SetAnimatorValues();
    }

    void SetVelocity()
    {
        Vector2 _velocity;
        Vector2 _input = Input;

        if (_input.sqrMagnitude > 1)
        {
            _input.Normalize();
        }
        if (_input.sqrMagnitude != 0)
        {
            _character.SetFacing(_input);
        }
        _velocity = _input * moveSpeed * MovespeedMultiplier;

        _character.Rigidbody.linearVelocity = _velocity;
    }

    void SetAnimatorValues()
    {
        if (!animator)
        {
            return;
        }

        float _speed = Input.magnitude;
        animator.SetFloat("Speed", _speed);

        if (_speed > DEADZONE)
        {
            Vector2 _dir = Input.normalized;

            float _angle = Mathf.Atan2(_dir.y, _dir.x) * Mathf.Rad2Deg;
            if (_angle < 0)
            {
                _angle += 360f;
            }

            float _adjusted = (_angle + 22.5f) % 360f;

            int _index = Mathf.FloorToInt(_adjusted / 45f);

            Player.Config.Animator.SetFloat("DirectionIndex", (float)_index);
        }
    }

    bool IsMovementEnabled()
    {
        switch (_character.State.Status)
        {
            case CharacterStatus.Recoil:
                return false;
            case CharacterStatus.Falling:
                return false;
            case CharacterStatus.Knockback:
                return false;
            default:
                break;
        }

        return true;
    }
}
