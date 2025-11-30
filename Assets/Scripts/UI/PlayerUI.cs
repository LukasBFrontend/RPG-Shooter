using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UIElements.Image;

[RequireComponent(typeof(UIDocument))]
public class PlayerUI : MonoBehaviour
{
    [Header("Hearts")]
    [SerializeField] private Sprite heartFull;
    [SerializeField] private Sprite heartThreeQuarters;
    [SerializeField] private Sprite heartHalf;
    [SerializeField] private Sprite heartOneQuarter;
    [SerializeField] private Sprite heartEmpty;
    [SerializeField] UnityEngine.UI.Image[] heartImages;
    private UIDocument document;
    List<VisualElement> heartTextures;
    List<VisualElement> itemImages;
    Label coinsText;

    void Start()
    {
        Cache();
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

    void Cache()
    {
        document = GetComponent<UIDocument>();
        heartTextures = document.rootVisualElement.Query<VisualElement>(className: "heart").ToList();
        itemImages = document.rootVisualElement.Query<VisualElement>(className: "item-image").ToList();
        coinsText = document.rootVisualElement.Query<Label>(name: "CoinText");
    }
    void RenderCoins()
    {
        coinsText.text = GameState.Coins.ToString();
    }
    void RenderHearts()
    {
        int healthPerHeart = PlayerState.Instance.healthPerHeart;
        int health = PlayerState.Instance.health;
        for (int i = 1; i <= heartTextures.Count; i++)
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
            heartTextures[i - 1].style.backgroundImage = Background.FromSprite(targetSprite);
        }
    }
}
