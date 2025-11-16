using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Heart Sprites")]
    [SerializeField] private Sprite heartFull;
    [SerializeField] private Sprite heartThreeQuarters;
    [SerializeField] private Sprite heartHalf;
    [SerializeField] private Sprite heartOneQuarter;
    [SerializeField] private Sprite heartEmpty;
    [Header("Health Target Images")]
    [SerializeField] Image[] heartImages;
    void Start()
    {
        RenderHearts();
    }

    void Update()
    {
        if (!PlayerState.Instance.IsDamaged())
        {
            return;
        }

        RenderHearts();
    }

    void RenderHearts()
    {
        int healthPerHeart = PlayerState.Instance.healthPerHeart;
        int health = PlayerState.Instance.health;
        for (int i = 1; i <= heartImages.Length; i++)
        {
            Sprite targetSprite = null;

            if (health >= i * healthPerHeart)
            {
                targetSprite = heartFull;
            }
            else if (health <= healthPerHeart * i - healthPerHeart)
            {
                targetSprite = heartEmpty;
            }
            else if (health % healthPerHeart == 3)
            {
                targetSprite = heartThreeQuarters;
            }
            else if (health % healthPerHeart == 2)
            {
                targetSprite = heartHalf;
            }
            else if (health % healthPerHeart == 1)
            {
                targetSprite = heartOneQuarter;
            }
            else
            {
                Debug.LogError("Correct target sprite not identified for health graphic");
            }

            heartImages[i - 1].sprite = targetSprite;
        }
    }
}
