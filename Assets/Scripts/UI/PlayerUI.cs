using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Hearts")]
    [SerializeField] private Sprite heartFull;
    [SerializeField] private Sprite heartThreeQuarters;
    [SerializeField] private Sprite heartHalf;
    [SerializeField] private Sprite heartOneQuarter;
    [SerializeField] private Sprite heartEmpty;
    [SerializeField] Image[] heartImages;
    [Header("Coins")]
    [SerializeField] Text coinsText;

    void Start()
    {
        RenderHearts();
        RenderCoins();
    }

    void Update()
    {
        if (PlayerState.Instance.IsDamaged())
        {
            RenderHearts();
        }
        RenderCoins();
    }

    void RenderCoins()
    {
        coinsText.text = LogicScript.Instance.Coins.ToString();
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
