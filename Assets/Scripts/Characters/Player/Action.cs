using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class Actions : Singleton<Actions>
{
    const float INTERACT_RANGE = 1.25f;
    [SerializeField] Collider2D interactCollider;
    List<IInteractable> _interactablesCurrent = new();
    bool _interactionQued;
    Player _player;

    protected override void OnAwake()
    {
        _player = GetComponent<Player>();
    }
    void Update()
    {
        float _rotation = Mathf.Atan2(_player.FaceDir.y, _player.FaceDir.x) * Mathf.Rad2Deg;
        interactCollider.transform.rotation = Quaternion.Euler(0, 0, _rotation);

        if (_interactablesCurrent.Count > 0)
        {
            MakeInteraction(_interactablesCurrent[0]);
        }
        _interactionQued = false;

        FocusInteractables();
    }

    public void HolsterWeapon()
    {
        if (_player.Inventory.HeldItem() is Weapon weapon)
        {
            weapon.ToggleHolstered();
        }
    }

    public void HeldItemAction()
    {
        if (_player.State.Status == CharacterStatus.Falling)
        {
            return;
        }
        _player.Inventory.HeldItem().Use();
    }

    public void HeldItemFocus()
    {
        _player.Inventory.HeldItem().Focus();
    }

    public void SetSelectedItemSlot(int index)
    {
        _player.Inventory.SetSelectedItemSlot(index);
    }

    public void Interact()
    {
        _interactionQued = true;
    }

    void FocusInteractables()
    {
        Vector2 _mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        List<IInteractable> _interactablesNew = InteracteablesAtPoint(_mouseWorld);

        foreach (IInteractable interactable in _interactablesCurrent)
        {
            if (_interactablesNew.Contains(interactable))
            {
                continue;
            }

            interactable.UnFocus();
        }

        _interactablesCurrent.Clear();
        _interactablesCurrent.AddRange(_interactablesNew);
    }

    List<IInteractable> InteracteablesAtPoint(Vector2 position)
    {
        const float RADIUS = .5f;
        Collider2D[] _results = new Collider2D[5];

        Physics2D.OverlapCircle(position, RADIUS, new ContactFilter2D().NoFilter(), _results);
        List<IInteractable> _interactables = new();

        foreach (Collider2D collider in _results)
        {
            if (!collider || (_player.SpriteCenter() - (Vector2)collider.transform.position).magnitude > INTERACT_RANGE || !collider.TryGetComponent<IInteractable>(out var interactable))
            {
                continue;
            }

            interactable.Focus();
            _interactables.Add(interactable);
        }

        return _interactables;
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
}
