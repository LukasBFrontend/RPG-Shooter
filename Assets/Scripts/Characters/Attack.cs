using System;
using System.Collections;
using UnityEngine;

public struct AttackContext
{
    public Character Attacker;
    public Character[] Targets;
    public int Damage;
    public float KnockbackForce;
    public GameObject Projectile;
    public Transform Origin;
    public Vector2 Direction;
    public float Velocity;
}
public delegate void AttackAction(AttackContext context);

[Serializable]
public class Attack
{
    public enum AttackType
    {
        Melee,
        Ranged
    }
    [SerializeField] string name;
    [SerializeField] private AttackType attackType;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] Transform origin;
    [SerializeField] float projectileVelocity = 10f;
    [Range(0, 24)]
    [SerializeField] private int damage;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float cooldown;

    public string Name => name;
    public int Damage => damage;
    public float KnockbackForce => knockbackForce;
    public float Cooldown => cooldown;

    public AttackAction ActionAction { get; set; } = null;
    public bool OnCooldown { get; private set; } = false;

    public void Attempt(Character attacker, Character[] targets)
    {
        if (OnCooldown)
        {
            return;
        }
        if (ActionAction == null)
        {
            switch (attackType)
            {
                case AttackType.Melee:
                    ActionAction = AttackActions.Melee;
                    break;
                case AttackType.Ranged:
                    ActionAction = AttackActions.Ranged;
                    break;
            }
        }

        ActionAction(
            new AttackContext
            {
                Attacker = attacker,
                Targets = targets,
                Damage = damage,
                KnockbackForce = knockbackForce,
                Projectile = projectilePrefab,
                Velocity = projectileVelocity
            }
        );
        attacker.StartCoroutine(CooldownTimer(Cooldown));
    }

    public void Attempt(Character attacker, Vector2 direction)
    {
        if (OnCooldown)
        {
            return;
        }
        if (ActionAction == null)
        {
            switch (attackType)
            {
                case AttackType.Melee:
                    ActionAction = AttackActions.Melee;
                    break;
                case AttackType.Ranged:
                    ActionAction = AttackActions.Ranged;
                    break;
            }
        }

        ActionAction(
            new AttackContext
            {
                Attacker = attacker,
                Damage = damage,
                KnockbackForce = knockbackForce,
                Projectile = projectilePrefab,
                Origin = origin,
                Direction = direction,
                Velocity = projectileVelocity,

            }
        );
        attacker.StartCoroutine(CooldownTimer(Cooldown));
    }



    IEnumerator CooldownTimer(float cooldown)
    {
        OnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        OnCooldown = false;
    }
}
