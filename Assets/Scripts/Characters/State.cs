using UnityEngine;

public enum CharacterStatus
{
    None,
    Recoil,
    Knockback,
    Falling,
}

public class State : MonoBehaviour
{
    [SerializeField] Character character;
    public CharacterStatus Status { get; private set; }
    public int Hearts { get; private set; } = 3;
    public int Health { get { return character.Health; } }
    float _statusTimer = 0f;

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

    public void SetStatus(CharacterStatus status, float duration)
    {
        _statusTimer = duration;

        Status = status;
    }
}
