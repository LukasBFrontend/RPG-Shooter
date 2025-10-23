using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") return;
        Destroy(this.gameObject);
        Debug.Log("Bullet collision");
    }
}
