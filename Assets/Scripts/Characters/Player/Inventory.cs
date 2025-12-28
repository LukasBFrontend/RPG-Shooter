using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class Inventory : Singleton<Inventory>
{
    [SerializeField] Weapon[] weapons;
    public List<IInventoryItem> Items { get; private set; }
    public int HeldIndex { get; private set; } = 0;

    void Start()
    {
        Cache();
    }

    void Update()
    {
        RenderHeldItem();
    }

    public IInventoryItem HeldItem()
    {
        return Items[HeldIndex];
    }

    public void SetSelectedItemSlot(int index)
    {
        if (index < 0 || index >= Items.Count)
        {
            return;
        }

        HeldIndex = index;
    }

    void RenderHeldItem()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            Items[i].GameObject.GetComponent<SpriteRenderer>().enabled = i == HeldIndex;
        }
    }

    void Cache()
    {
        if (Items == null || Items.Count <= 0)
        {
            Items = new();
            foreach (Weapon weapon in weapons)
            {
                if (weapon.TryGetComponent<IInventoryItem>(out var item))
                {
                    Items.Add(item);
                }
                else
                {
                    Debug.LogError("No IInventoryItem found on GameObject " + weapon.name);
                }
            }
        }
    }
}
