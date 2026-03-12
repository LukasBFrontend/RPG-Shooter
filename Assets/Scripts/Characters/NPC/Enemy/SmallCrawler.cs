using UnityEngine;

public class SmallCrawler : NPC
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



        //TurnNPCSmooth();
    }
}
