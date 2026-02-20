using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] float moveSpeed = 6;
    [Header("Optional")]
    public Vector2 Input { get; set; } = Vector2.zero;
    [Range(0, 5)]
    public float MovespeedMultiplier { get; set; } = 1f;

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
            character.SetFacing(_input);
        }
        _velocity = _input * moveSpeed * MovespeedMultiplier;

        character.Rigidbody.linearVelocity = _velocity;
    }
    bool IsMovementEnabled()
    {
        switch (character.State.Status)
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
