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
