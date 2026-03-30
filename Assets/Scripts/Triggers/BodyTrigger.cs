using UnityEngine;

public class BodyTrigger : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] bool triggerEvents = false;
    [SerializeField] bool damageable = true;

    public Character Character { get => character; }
    public bool TriggerEvents { get => triggerEvents; }

    public bool DamageableBy(Character attacker)
    {
        return damageable && attacker != character;
    }
}
