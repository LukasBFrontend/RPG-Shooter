using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class PlayerUI : MonoBehaviour
{
    const int HEALTH_PER_HEART = 4;
    [Header("Item Sprites")]
    [SerializeField] Sprite flintlock;
    [SerializeField] Sprite blunderbuss;
    [Header("Heart Sprites")]
    [SerializeField] Sprite heartFull;
    [SerializeField] Sprite heartThreeQuarters;
    [SerializeField] Sprite heartHalf;
    [SerializeField] Sprite heartOneQuarter;
    [SerializeField] Sprite heartEmpty;
    UIDocument _document;
    List<VisualElement> _heartTextures;
    List<VisualElement> _itemSlots;
    List<VisualElement> _itemImages;
    Label _coinsText;

    void Start()
    {
        Cache();
        RenderHearts();
        RenderItems();
        RenderCoins();
    }

    void Update()
    {
        RenderHearts();
        RenderItems();
        RenderCoins();
    }

    void Cache()
    {
        _document = GetComponent<UIDocument>();
        _heartTextures = _document.rootVisualElement.Query<VisualElement>(className: "heart").ToList();
        _itemSlots = _document.rootVisualElement.Query<VisualElement>(className: "item-slot").ToList();
        _itemImages = _document.rootVisualElement.Query<VisualElement>(className: "item-image").ToList();
        _coinsText = _document.rootVisualElement.Query<Label>(name: "CoinText");

    }
    void RenderCoins()
    {
        _coinsText.text = GameState.Coins.ToString();
    }

    void RenderItems()
    {
        List<IInventoryItem> _inventoryItems = GameState.Player.Inventory.Items;
        if (_inventoryItems == null)
        {
            return;
        }

        int _heldItemIndex = GameState.Player.Inventory.HeldIndex;
        for (int i = 0; i < _inventoryItems.Count; i++)
        {
            Sprite _itemSprite = _inventoryItems[i].UI_Sprite;
            _itemImages[i].style.backgroundImage = Background.FromSprite(_itemSprite);

            VisualElement _itemSlot = _itemSlots[i];
            string _selectedClass = "item-slot-selected";

            if (i == _heldItemIndex)
            {
                _itemSlot.AddToClassList(_selectedClass);
            }
            else
            {
                _itemSlot.RemoveFromClassList(_selectedClass);
            }
        }
    }
    void RenderHearts()
    {
        int _health = GameState.Player.Health;

        for (int i = 1; i <= _heartTextures.Count; i++)
        {
            Sprite _targetSprite = null;

            if (_health >= i * HEALTH_PER_HEART)
            {
                _targetSprite = heartFull;
            }
            else if (_health <= HEALTH_PER_HEART * i - HEALTH_PER_HEART)
            {
                _targetSprite = heartEmpty;
            }
            else if (_health % HEALTH_PER_HEART == 3)
            {
                _targetSprite = heartThreeQuarters;
            }
            else if (_health % HEALTH_PER_HEART == 2)
            {
                _targetSprite = heartHalf;
            }
            else if (_health % HEALTH_PER_HEART == 1)
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
