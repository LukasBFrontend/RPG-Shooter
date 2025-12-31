using UnityEngine;

public interface IInteractable
{
    void Interact();
    void Focus();
    void UnFocus();
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
