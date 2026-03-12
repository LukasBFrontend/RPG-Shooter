using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Slug : NPC
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
        return;
        if (Health <= 0)
        {
            Die();
        }


    }
}
