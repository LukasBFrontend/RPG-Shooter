using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : Singleton<PlayerAction>
{
    [SerializeField] Weapon[] weapons;
    [SerializeField] Collider2D interactCollider;
    List<IInventoryItem> _inventory;
    List<IInteractable> _interactablesInRange = new();
    List<GameObject> _inventoryObjects;
    bool _interactionQued;
    int _heldIndex = 0;
    public void HeldItemAction()
    {
        HeldItem().Use();
    }

    public IInventoryItem HeldItem()
    {
        return _inventory[_heldIndex];
    }

    public void Interact()
    {
        _interactionQued = true;
    }

    public void SetSelectedItemSlot(int index)
    {
        if (index < 0 || index >= _inventory.Count)
        {
            return;
        }

        _heldIndex = index;
    }

    void MakeInteraction(IInteractable interactable)
    {
        if (!_interactionQued)
        {
            return;
        }

        interactable.Interact();
        _interactionQued = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.TryGetComponent<IInteractable>(out var interactable))
        {
            return;
        }
        _interactablesInRange.Add(interactable);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.TryGetComponent<IInteractable>(out var interactable))
        {
            return;
        }
        _interactablesInRange.Remove(interactable);
    }
    void RenderHeldItem()
    {
        for (int i = 0; i < _inventoryObjects.Count; i++)
        {
            _inventoryObjects[i].SetActive(i == _heldIndex);
        }
    }

    void CacheInventory()
    {
        if (_inventory == null || _inventory.Count <= 0)
        {
            _inventory = new();
            _inventoryObjects = new();
            foreach (Weapon weapon in weapons)
            {
                if (weapon.TryGetComponent<IInventoryItem>(out var item))
                {
                    _inventory.Add(item);
                    _inventoryObjects.Add(weapon.gameObject);
                }
                else
                {
                    Debug.LogError("No IInventoryItem found on GameObject " + weapon.name);
                }
            }
        }
    }

    void Start()
    {
        CacheInventory();
    }

    void Update()
    {
        RenderHeldItem();

        interactCollider.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(PlayerMove.Instance.Direction.y, PlayerMove.Instance.Direction.x) * Mathf.Rad2Deg);

        if (_interactablesInRange.Count > 0)
        {
            MakeInteraction(_interactablesInRange[0]);
        }
        _interactionQued = false;
    }
}
