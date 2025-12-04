using UnityEngine;

interface IInteractable
{
    void Interact();
}

public interface IInventoryItem
{
    string Name { get; }
    int Count { get; set; }
    void Use();
}

interface IObstructive
{
    Bounds OccupationBounds(float offset);
}
