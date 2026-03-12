using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class Controller : MonoBehaviour
{
    [SerializeField] float moveSpeed = 6;
    [Header("Optional")]
    [Range(0, 5)]
    public float MovespeedMultiplier { get; set; } = 1f;
    Character _character;
    public Node CurrentNode { get; private set; }
    public Vector2 Input { get; private set; } = Vector2.zero;

    void Awake()
    {
        _character = GetComponent<Character>();
    }

    public void Jump(Vector2 direction, float speed)
    {
        Rigidbody2D _rb = _character.Rigidbody;
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(direction * speed);
    }

    public void FollowCharacter(Character otherCharacter)
    {
        Vector2 _direction = (otherCharacter.ColliderCenter() - _character.ColliderCenter()).normalized;
        _character.SetFacing(_direction);
        Move(_direction);
    }

    public void FollowCharacter(Character otherCharacter, Vector2 offset)
    {
        Vector2 _direction = (otherCharacter.ColliderCenter() - _character.ColliderCenter()).normalized + offset;
        _character.SetFacing(_direction);
        Move(_direction);
    }

    public void FollowPath(List<Node> path)
    {
        int x = 0;
        Vector3 _targetPos = new(path[x].transform.position.x, path[x].transform.position.y, 0);
        Vector2 _direction = (_targetPos - transform.position).normalized;
        _character.SetFacing(_direction);

        Move(_direction);

        if (Vector2.Distance(transform.position, path[x].transform.position) < 0.1f)
        {
            _character.Node = path[x];
            path.RemoveAt(x);
        }
    }

    public void Move(Vector2 input)
    {
        if (input.sqrMagnitude > 1)
        {
            input.Normalize();
        }
        if (input.sqrMagnitude != 0)
        {
            _character.SetFacing(input);
        }
        Vector2 _velocity = moveSpeed * MovespeedMultiplier * input;
        _character.Rigidbody.linearVelocity = _velocity;
        Input = input;
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
