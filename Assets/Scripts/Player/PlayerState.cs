using UnityEngine;

public static class PlayerState
{
    public static int health = 100;
    public static bool alive = true;

    public static void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
        Debug.Log("Player took " + damage + " damage");
        Visuals.Instance.Flicker(PlayerConfig.Instance.SpriteRenderer, 4, .25f);
    }

    public static void Die()
    {
        if (!alive)
        {
            return;
        }

        alive = false;
        Debug.Log("Player dead");
    }
}
