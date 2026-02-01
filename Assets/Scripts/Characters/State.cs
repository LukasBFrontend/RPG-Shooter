using UnityEngine;

public enum CharacterStatus
{
    None,
    Recoil,
    Knockback,
    Falling,
}

[RequireComponent(typeof(Character))]
public class State : MonoBehaviour
{
    public CharacterStatus Status { get; private set; }
    public int Hearts { get; private set; } = 3;
    public int Health { get { return _character.Health; } }
    float _statusTimer = 0f;
    Character _character;

    void Start()
    {
        _character = GetComponent<Character>();
    }

    void Update()
    {
        if (_statusTimer > 0f)
        {
            _statusTimer -= Time.deltaTime;
            return;
        }

        if (Status != CharacterStatus.None)
        {
            Status = CharacterStatus.None;
        }
    }

    public bool IsDamaged()
    {
        return _character.IsDamaged();
    }

    public void SetStatus(CharacterStatus status, float duration)
    {
        _statusTimer = duration;

        Status = status;
    }
    public Node CurrentNode()
    {
        return NodeManager.Instance.ClosestNode(transform.position);
    }
}
