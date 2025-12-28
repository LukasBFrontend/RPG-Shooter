using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class Actions : Singleton<Actions>
{
    [SerializeField] Collider2D interactCollider;
    List<IInteractable> _interactablesInRange = new();
    bool _interactionQued;
    Player _player;

    void Start()
    {
        _player = GetComponent<Player>();
    }
    void Update()
    {
        float _rotation = Mathf.Atan2(_player.FaceDir.y, _player.FaceDir.x) * Mathf.Rad2Deg;
        interactCollider.transform.rotation = Quaternion.Euler(0, 0, _rotation);

        if (_interactablesInRange.Count > 0)
        {
            MakeInteraction(_interactablesInRange[0]);
        }
        _interactionQued = false;
    }

    public void HeldItemAction()
    {
        Player.Inventory.HeldItem().Use();
    }

    public void HeldItemFocus()
    {
        Player.Inventory.HeldItem().Focus();
    }

    public void SetSelectedItemSlot(int index)
    {
        Player.Inventory.SetSelectedItemSlot(index);
    }

    public void Interact()
    {
        _interactionQued = true;
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
}
