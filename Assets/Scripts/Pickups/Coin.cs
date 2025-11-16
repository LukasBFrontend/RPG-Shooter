using UnityEngine;

public class Coin : MonoBehaviour
{
    [Range(1, 50)][SerializeField] int value = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        LogicScript.Instance.AddCoins(value);
        Destroy(gameObject);
    }
}
