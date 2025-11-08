using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] string[] ignoreCollisionTags = { "Player" };
    [SerializeField] string[] enemyTags = { "Enemy" };
    [SerializeField] int damage = 50;

    void OnTriggerEnter2D(Collider2D other)
    {

        Debug.Log("Bullet collision");

        foreach (string tag in enemyTags)
        {
            if (other.CompareTag(tag))
            {
                NPC enemy = other.GetComponent<NPC>();

                if (enemy)
                {
                    enemy.TakeDamage(damage);
                }
                break;
            }
        }

        Destroy(this.gameObject);
    }
}
