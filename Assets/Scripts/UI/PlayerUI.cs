using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UIElements.Image;

[RequireComponent(typeof(UIDocument))]
public class PlayerUI : MonoBehaviour
{
    [Header("Hearts")]
    [SerializeField] Sprite heartFull;
    [SerializeField] Sprite heartThreeQuarters;
    [SerializeField] Sprite heartHalf;
    [SerializeField] Sprite heartOneQuarter;
    [SerializeField] Sprite heartEmpty;
    [SerializeField] UnityEngine.UI.Image[] heartImages;
    UIDocument _document;
    List<VisualElement> _heartTextures;
    List<VisualElement> _itemImages;
    Label _coinsText;

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
        _document = GetComponent<UIDocument>();
        _heartTextures = _document.rootVisualElement.Query<VisualElement>(className: "heart").ToList();
        _itemImages = _document.rootVisualElement.Query<VisualElement>(className: "item-image").ToList();
        _coinsText = _document.rootVisualElement.Query<Label>(name: "CoinText");
    }
    void RenderCoins()
    {
        _coinsText.text = GameState.Coins.ToString();
    }
    void RenderHearts()
    {
        int _healthPerHeart = PlayerState.Instance.HealthPerHeart;
        int _health = PlayerState.Instance.Health;

        for (int i = 1; i <= _heartTextures.Count; i++)
        {
            Sprite _targetSprite = null;

            if (_health >= i * _healthPerHeart)
            {
                _targetSprite = heartFull;
            }
            else if (_health <= _healthPerHeart * i - _healthPerHeart)
            {
                _targetSprite = heartEmpty;
            }
            else if (_health % _healthPerHeart == 3)
            {
                _targetSprite = heartThreeQuarters;
            }
            else if (_health % _healthPerHeart == 2)
            {
                _targetSprite = heartHalf;
            }
            else if (_health % _healthPerHeart == 1)
            {
                _targetSprite = heartOneQuarter;
            }
            else
            {
                Debug.LogError("Correct target sprite not identified for health graphic");
            }
            _heartTextures[i - 1].style.backgroundImage = Background.FromSprite(_targetSprite);
        }
    }
}
