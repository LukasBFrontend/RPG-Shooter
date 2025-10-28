using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] string[] ignoreCollisionTags = { "Player" };
    [SerializeField] string[] enemyTags = { "Enemy" };
    [SerializeField] int damage = 50;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Hit Collider")) return;

        Debug.Log("Bullet collision");

        Transform parent = other.transform.parent;
        foreach (string tag in enemyTags)
        {
            if (parent.CompareTag(tag))
            {
                Enemy enemy = parent.GetComponent<Enemy>();

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
