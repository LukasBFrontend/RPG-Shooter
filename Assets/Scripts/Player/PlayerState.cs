using UnityEngine;
using UnityEngine.UI;

public class PlayerState : Singleton<PlayerState>
{
    public int hearts = 3;
    public int healthPerHeart = 4;
    [HideInInspector] public int health;
    public bool alive = true;


    public void Start()
    {
        health = hearts * healthPerHeart;
    }
    public void TakeDamage(int damage)
    {
        if (health <= 0)
        {
            Die();
            return;
        }
        health -= damage;

        Visuals.Instance.Flicker(PlayerConfig.Instance.SpriteRenderer, 4, .25f);
    }

    public bool IsDamaged()
    {
        return health != hearts * healthPerHeart;
    }

    public void Die()
    {
        if (!alive)
        {
            return;
        }

        alive = false;
        Debug.Log("Player dead");
    }
}
