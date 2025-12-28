using UnityEngine;

interface IInteractable
{
    void Interact();
}

public interface IInventoryItem
{
    string Name { get; }
    int Count { get; set; }
    Sprite UI_Sprite { get; }
    GameObject GameObject { get; }
    void Use();
    void Focus();

}

interface IObstructive
{
    Bounds OccupationBounds(float offset);
}
