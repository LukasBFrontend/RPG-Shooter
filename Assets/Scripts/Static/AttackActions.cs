using UnityEngine;

public delegate void AttackAction(AttackContext context, out GameObject projectile);
public static class AttackActions
{
    public static AttackAction Melee { get; private set; } = (AttackContext context, out GameObject projectileInstance) =>
    {
        Character _attacker = context.Attacker;
        float _knockbackForce = context.KnockbackForce;
        projectileInstance = null;

        foreach (Character target in context.Targets)
        {
            target.TakeDamage(context.Damage);
            Vector2 _dir = _attacker.FaceDir;

            if (_knockbackForce > 0)
            {
                target.Rigidbody.AddForce(_dir * context.KnockbackForce);
                target.State.SetStatus(CharacterStatus.Knockback, .3f);
            }
        }
    };

    public static AttackAction Ranged { get; private set; } = (AttackContext context, out GameObject projectileInstance) =>
    {
        Character _attacker = context.Attacker;
        GameObject _projectile = context.Projectile;
        Transform _origin = context.Origin;
        Vector2 _dir = context.Direction.normalized;
        float _velocity = context.Velocity;
        int _damage = context.Damage;
        float _lifeTime = context.Lifetime;

        if (_projectile == null)
        {
            Debug.LogError("Projectile can not be null. AttackAction Ranged aborted");
            projectileInstance = null;
        }

        Quaternion _aimAngle = Quaternion.Euler(new(0, 0, Mathf.Rad2Deg * Mathf.Atan2(_dir.y, _dir.x)));

        projectileInstance = Object.Instantiate(_projectile, (Vector2)_origin.position + _dir, _aimAngle, null);
        projectileInstance.layer = _attacker.gameObject.layer + 3;
        projectileInstance.GetComponent<Rigidbody2D>().linearVelocity = _dir * _velocity;

        Projectile _projectileRef = projectileInstance.GetComponent<Projectile>();
        _projectileRef.SetLifeTime(_lifeTime);
        _projectileRef.Damage = _damage;
        _projectileRef.Sender = _attacker;
    };

    public static AttackAction Lunge { get; private set; } = (AttackContext context, out GameObject projectileInstance) =>
    {
        Character _attacker = context.Attacker;
        GameObject _projectile = context.Projectile;
        Character[] _targets = context.Targets;
        float _velocity = context.Velocity;
        int _damage = context.Damage;
        float _lifeTime = context.Lifetime;

        Vector2 _dir = _targets != null && _targets.Length > 0 ? (_targets[0].ColliderCenter() - _attacker.ColliderCenter()).normalized : context.Direction.normalized;

        _attacker.Controller.Jump(_dir, _velocity);

        Rigidbody2D _rb = _attacker.Rigidbody;

        projectileInstance = Object.Instantiate(_projectile, _attacker.ColliderCenter(), Quaternion.identity, _attacker.transform);
        projectileInstance.layer = _attacker.gameObject.layer;

        Projectile _projectileRef = projectileInstance.GetComponent<Projectile>();
        _projectileRef.SetLifeTime(_lifeTime);
        _projectileRef.Damage = _damage;
        _projectileRef.Sender = _attacker;
    };
}
