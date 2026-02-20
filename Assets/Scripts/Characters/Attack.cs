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
        Ranged,
        Lunge
    }
    [SerializeField] string name;
    [SerializeField] AttackType attackType;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform origin;
    [Range(0, 2)]
    [SerializeField] float windUp = 0f;
    [SerializeField] float projectileVelocity = 10f;
    [Range(0, 24)]
    [SerializeField] int damage;
    [SerializeField] float knockbackForce;
    [SerializeField] float cooldown;

    public string Name => name;
    public int Damage => damage;
    public float KnockbackForce => knockbackForce;
    public float Cooldown => cooldown;

    public AttackAction ActionAction { get; set; } = null;
    public bool OnCooldown { get; private set; } = false;
    public bool IsWindingUp { get; private set; } = false;
    bool _attackWasCancelled = false;

    public void Attempt(Character attacker, Character[] targets)
    {
        if (OnCooldown)
        {
            return;
        }

        attacker.StartCoroutine(AttemptTargets(attacker, targets));
        attacker.StartCoroutine(CooldownTimer(Cooldown));
    }

    public void Attempt(Character attacker, Vector2 direction)
    {
        if (OnCooldown)
        {
            return;
        }

        attacker.StartCoroutine(AttemptDirectional(attacker, direction));
        attacker.StartCoroutine(CooldownTimer(Cooldown));
    }

    public void TryCancel()
    {
        _attackWasCancelled = true;
    }

    IEnumerator AttemptTargets(Character attacker, Character[] targets)
    {
        IsWindingUp = true;
        float _elapsed = 0f;

        while (_elapsed < windUp)
        {
            if (_attackWasCancelled)
            {
                yield break;
            }

            yield return new WaitForEndOfFrame();
            _elapsed += Time.deltaTime;
        }
        IsWindingUp = false;

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
                case AttackType.Lunge:
                    ActionAction = AttackActions.Lunge;
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
    }

    IEnumerator AttemptDirectional(Character attacker, Vector2 direction)
    {
        IsWindingUp = true;
        float _elapsed = 0f;

        while (_elapsed < windUp)
        {
            if (_attackWasCancelled)
            {
                yield break;
            }

            yield return new WaitForEndOfFrame();
            _elapsed += Time.deltaTime;
        }
        IsWindingUp = false;

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
                case AttackType.Lunge:
                    ActionAction = AttackActions.Lunge;
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
    }

    IEnumerator CooldownTimer(float cooldown)
    {
        OnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        OnCooldown = false;
    }
}
