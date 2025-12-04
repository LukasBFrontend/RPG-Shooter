using UnityEngine;

public class Slug : NPC
{
    [SerializeField] int damagePerAttack = 20;
    [SerializeField] float attackSpeed = 1f;
    [SerializeField] float knockbackForce = 6f;
    float _attackTimer = 0f;
    bool _hasAttacked = false;
    bool _playerInRange = false;

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
        if (!_playerInRange)
        {
            return;
        }
        PlayerState.Instance.TakeDamage(damagePerAttack);
        PlayerConfig.Instance.Rb.AddForce(GetComponent<NPC_Controller>().PlayerToNPC().normalized * knockbackForce);
        PlayerConfig.Instance.Status = PlayerStatus.Knockback;
        _hasAttacked = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        _playerInRange = true;
        _attackTimer = 0f;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        _playerInRange = false;
    }
}
