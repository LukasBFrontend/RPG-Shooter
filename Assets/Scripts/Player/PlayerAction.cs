using System.Collections.Generic;
using UnityEngine;

public interface IInventoryItem
{
    string Name { get; }
    int Count { get; set; }
    void Use();
}
public class PlayerAction : Singleton<PlayerAction>
{
    [SerializeField] Weapon[] weapons;
    [SerializeField] Collider2D interactCollider;
    List<IInventoryItem> inventory;
    List<GameObject> inventoryObjects;
    List<Interactable> interactablesInRange = new();
    bool interactionQued;
    int heldIndex = 0;
    public void HeldItemAction()
    {
        HeldItem().Use();
    }

    public IInventoryItem HeldItem()
    {
        return inventory[heldIndex];
    }

    public void Interact()
    {
        interactionQued = true;
    }

    public void SetSelectedItemSlot(int index)
    {
        if (index < 0 || index >= inventory.Count)
        {
            return;
        }

        heldIndex = index;
    }

    void MakeInteraction(Interactable interactable)
    {
        if (!interactionQued)
        {
            return;
        }

        interactable.Interact();
        interactionQued = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.TryGetComponent<Interactable>(out var interactable))
        {
            return;
        }
        interactablesInRange.Add(interactable);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.TryGetComponent<Interactable>(out var interactable))
        {
            return;
        }
        interactablesInRange.Remove(interactable);
    }
    void RenderHeldItem()
    {
        for (int i = 0; i < inventoryObjects.Count; i++)
        {
            inventoryObjects[i].SetActive(i == heldIndex);
        }
    }

    void CacheInventory()
    {
        if (inventory == null || inventory.Count <= 0)
        {
            inventory = new();
            inventoryObjects = new();
            foreach (Weapon weapon in weapons)
            {
                if (weapon.TryGetComponent<IInventoryItem>(out var item))
                {
                    inventory.Add(item);
                    inventoryObjects.Add(weapon.gameObject);
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

        interactCollider.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(PlayerMove.Instance.direction.y, PlayerMove.Instance.direction.x) * Mathf.Rad2Deg);

        if (interactablesInRange.Count > 0)
        {
            MakeInteraction(interactablesInRange[0]);
        }
        interactionQued = false;
    }
}
