using UnityEngine;

public class Slug : NPC
{
    [SerializeField] int damagePerAttack = 20;
    [SerializeField] float attackSpeed = 1f;
    [SerializeField] float knockbackForce = 6f;
    float attackTimer = 0f;
    bool hasAttacked = false;
    bool playerInRange = false;
    void Start()
    {

    }

    void Update()
    {
        if (health <= 0)
        {
            Die();
        }

        if (attackTimer % (1 / attackSpeed) < 1 / (2 * attackSpeed))
        {
            if (!hasAttacked)
            {
                Attack();
            }
        }
        else
        {
            hasAttacked = false;
        }

        UpdateRotation();
        attackTimer += Time.deltaTime;
    }

    void Attack()
    {
        if (!playerInRange)
        {
            return;
        }
        PlayerState.Instance.TakeDamage(damagePerAttack);
        PlayerConfig.Instance.Rb.AddForce(GetComponent<NPC_Controller>().PlayerToNPC().normalized * knockbackForce);
        PlayerConfig.Instance.Status = PlayerStatus.Knockback;
        hasAttacked = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        playerInRange = true;
        attackTimer = 0f;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        playerInRange = false;
    }
}
