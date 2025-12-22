using UnityEngine;
public static class AttackActions
{
    public static AttackAction Melee { get; private set; } = (context) =>
    {
        Character _attacker = context.Attacker;
        float _knockbackForce = context.KnockbackForce;

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

    public static AttackAction Ranged { get; private set; } = (context) =>
    {
        Character _attacker = context.Attacker;
        GameObject _projectile = context.Projectile;
        Transform _origin = context.Origin;
        Vector2 _dir = context.Direction.normalized;
        float _velocity = context.Velocity;
        int _damage = context.Damage;

        if (_projectile == null)
        {
            Debug.LogError("Projectile can not be null. AttackAction Ranged aborted");
            return;
        }

        Quaternion _aimAngle = Quaternion.Euler(new(0, 0, Mathf.Rad2Deg * Mathf.Atan2(_dir.y, _dir.x)));

        GameObject _projectileInstance = Object.Instantiate(_projectile, (Vector2)_origin.position + _dir, _aimAngle, null);
        _projectileInstance.layer = _attacker.gameObject.layer + 3;
        _projectileInstance.GetComponent<Rigidbody2D>().linearVelocity = _dir * _velocity;

        Projectile _projectileRef = _projectileInstance.GetComponent<Projectile>();
        _projectileRef.Damage = _damage;
        _projectileRef.Sender = _attacker;
    };
}
