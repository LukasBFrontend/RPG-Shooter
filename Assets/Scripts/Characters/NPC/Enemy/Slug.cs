using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(NPC_Controller))]
public class Slug : NPC
{
    [SerializeField] Attack[] attacks;
    List<Character> _charactersInRange = new();

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

        Attack();

        RenderFaceDirection();
    }

    void Attack()
    {
        if (_charactersInRange.Count == 0)
        {
            return;
        }
        attacks.First().Attempt(this, _charactersInRange.ToArray());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponentInChildren<Player>();

        if (!player)
        {
            return;
        }
        _charactersInRange.Add(player);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if (!player)
        {
            return;
        }
        _charactersInRange.Remove(player);
    }
}
