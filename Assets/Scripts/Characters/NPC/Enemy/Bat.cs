using UnityEngine;

public class Bat : NPC
{
    void Start()
    {
        OnDeath = () =>
        {
            Destroy(gameObject);
        };
    }

    void Update()
    {
        if (Health <= 0)
        {
            Die();
        }

    }
}
