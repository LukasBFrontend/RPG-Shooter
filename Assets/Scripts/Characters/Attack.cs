using System;
using System.Collections;
using System.Collections.Generic;
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
    public float Lifetime;
    public float Velocity;
}

public enum AttackType
{
    Melee,
    Ranged,
    Lunge
}
[Serializable]
public class Attack
{
    [Serializable]
    public struct RangedAndLungeFields
    {
        public Transform Origin;
        public GameObject Projectile;
        public float Lifetime;
        public float Velocity;
    }
    [SerializeField] string name;
    [SerializeField] AttackType attackType;
    [SerializeField] float windUp = 0f;
    [Range(0, 24)]
    [SerializeField] int damage;
    [SerializeField] float knockbackForce;
    [Range(0, 10)]
    [SerializeField] float cooldown;
    [SerializeField] RangedAndLungeFields rangedAndLungeFields;
    bool _attackWasCancelled = false;
    List<GameObject> _projectiles = new();

    public string Name => name;
    public int Damage => damage;
    public float KnockbackForce => knockbackForce;
    public float WindUp => windUp;
    public float Cooldown => cooldown;
    public AttackType AttackType => attackType;
    public float Lifetime => rangedAndLungeFields.Lifetime;

    public AttackAction ActionAction { get; set; } = null;
    public bool OnCooldown { get; private set; } = false;
    public bool IsWindingUp { get; private set; } = false;
    public List<Character> CharactersInRange { get; set; } = new();

    public void CleanUpProjectiles()
    {
        foreach (GameObject projectile in _projectiles)
        {
            if (projectile != null)
            {
                GameObject.Destroy(projectile);
            }
        }
        _projectiles.Clear();
    }

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
                Projectile = rangedAndLungeFields.Projectile,
                Velocity = rangedAndLungeFields.Velocity,
                Lifetime = rangedAndLungeFields.Lifetime,
            },
            out GameObject _projectileInstance
        );

        if (_projectileInstance != null)
        {
            _projectiles.Add(_projectileInstance);
        }
    }

    IEnumerator AttemptDirectional(Character attacker, Vector2 direction)
    {
        IsWindingUp = true;
        float _elapsedWindUp = 0f;

        while (_elapsedWindUp < windUp)
        {
            if (_attackWasCancelled)
            {
                yield break;
            }

            yield return new WaitForEndOfFrame();
            _elapsedWindUp += Time.deltaTime;
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
                Direction = direction,
                Origin = rangedAndLungeFields.Origin,
                Projectile = rangedAndLungeFields.Projectile,
                Velocity = rangedAndLungeFields.Velocity,
                Lifetime = rangedAndLungeFields.Lifetime,

            },
            out GameObject _projectileInstance
        );

        if (_projectileInstance != null)
        {
            _projectiles.Add(_projectileInstance);
        }
    }

    IEnumerator CooldownTimer(float cooldown)
    {
        OnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        OnCooldown = false;
    }
}
