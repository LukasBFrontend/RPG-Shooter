using System.Collections;
using System.Linq;
using UnityEngine;

[System.Serializable]
enum BulletType
{
    Ball,
    Pellet,
}

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] BulletType bulletType = BulletType.Ball;
    [SerializeField] string[] ignoreCollisionTags = { "Player" };
    [SerializeField] string[] enemyTags = { "Enemy" };
    [SerializeField] int damage = 50;

    void OnCollisionEnter2D(Collision2D other)
    {
        Collider2D _col = other.collider;
        if (ignoreCollisionTags.Contains(_col.tag))
        {
            Physics2D.IgnoreCollision(_col, GetComponent<Collider2D>());
            return;
        }
        foreach (string _tag in enemyTags)
        {
            if (_col.CompareTag(_tag))
            {
                NPC _enemy = _col.GetComponent<NPC>();

                if (_enemy)
                {
                    _enemy.TakeDamage(damage);
                }
                break;
            }
        }
        if (bulletType == BulletType.Ball)
        {
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
                NPC _enemy = other.GetComponent<NPC>();

                if (_enemy)
                {
                    _enemy.TakeDamage(damage);
                }
                break;
            }
        }

        Destroy(gameObject);
    }
    IEnumerator DestroyDelayed(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
