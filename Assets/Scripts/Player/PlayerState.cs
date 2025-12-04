using UnityEngine;

public class PlayerState : Singleton<PlayerState>
{
    [HideInInspector] public int Health;
    [HideInInspector] public bool Alive = true;
    public int Hearts = 3;
    public int HealthPerHeart = 4;

    public void Start()
    {
        Health = Hearts * HealthPerHeart;
    }
    public void TakeDamage(int damage)
    {
        if (Health <= 0)
        {
            Die();
            return;
        }
        Health -= damage;

        Utils.Flicker(PlayerConfig.Instance.SpriteRenderer, 4, .25f);
    }

    public bool IsDamaged()
    {
        return Health != Hearts * HealthPerHeart;
    }

    public void Die()
    {
        if (!Alive)
        {
            return;
        }

        Alive = false;
        Debug.Log("Player dead");
    }
}
