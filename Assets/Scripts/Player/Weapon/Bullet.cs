using System.Collections;
using System.Linq;
using UnityEngine;


[System.Serializable]
enum BulletType
{
    Ball,
    Pellet,
}

public class Bullet : MonoBehaviour
{
    [SerializeField] BulletType bulletType = BulletType.Ball;
    [SerializeField] string[] ignoreCollisionTags = { "Player" };
    [SerializeField] string[] enemyTags = { "Enemy" };
    [SerializeField] int damage = 50;

    void OnCollisionEnter2D(Collision2D other)
    {
        Collider2D col = other.collider;
        if (ignoreCollisionTags.Contains(col.tag))
        {
            Physics2D.IgnoreCollision(col, GetComponent<Collider2D>());
            return;
        }
        foreach (string tag in enemyTags)
        {
            if (col.CompareTag(tag))
            {
                NPC enemy = col.GetComponent<NPC>();

                if (enemy)
                {
                    enemy.TakeDamage(damage);
                }
                break;
            }
        }
        if (bulletType == BulletType.Ball)
        {
            /* Debug.Log("Collision with " + col.name + " (tag: " + col.tag + "): gameObject destroyed"); */
            Destroy(gameObject);
        }
        else
        {
            GetComponent<Rigidbody2D>().linearVelocity = GetComponent<Rigidbody2D>().linearVelocity / 3;
            StartCoroutine(DestroyDelayed(.02f));
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (ignoreCollisionTags.Contains(other.tag))
        {
            return;
        }
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
        /* Debug.Log("Trigger with " + other.name + " (tag: " + other.tag + "): gameObject destroyed"); */
        Destroy(gameObject);
    }
    IEnumerator DestroyDelayed(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
