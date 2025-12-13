using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPC_Controller))]
public class Slug : NPC
{
    [SerializeField] int damagePerAttack = 20;
    [SerializeField] float attackSpeed = 1f;
    [SerializeField] float knockbackForce = 6f;
    float _attackTimer = 0f;
    bool _hasAttacked = false;
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

        if (_attackTimer % (1 / attackSpeed) < 1 / (2 * attackSpeed))
        {
            if (!_hasAttacked)
            {
                Attack();
            }
        }
        else
        {
            _hasAttacked = false;
        }

        UpdateRotation();
        _attackTimer += Time.deltaTime;
    }

    void Attack()
    {
        if (_charactersInRange.Count == 0)
        {
            return;
        }

        foreach (Character character in _charactersInRange)
        {
            character.TakeDamage(damagePerAttack);
            Player.Config.Rigidbody.AddForce(GetComponent<NPC_Controller>().PlayerToNPC().normalized * knockbackForce);
            Player.State.Status = PlayerStatus.Knockback;
        }
        _hasAttacked = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if (!player)
        {
            return;
        }
        _charactersInRange.Add(player);
        _attackTimer = 0f;
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
