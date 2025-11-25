using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : Singleton<PlayerAction>
{
    [SerializeField] Flintlock flintlock;
    [SerializeField] Collider2D interactCollider;
    bool interactionQued;
    List<Interactable> interactablesInRange = new();
    public void FireWeapon()
    {
        flintlock.Fire();
    }

    public void HolsterWeapon(bool isHolstering)
    {
        flintlock.GetComponent<SpriteRenderer>().enabled = !isHolstering;
    }

    public void Interact()
    {
        interactionQued = true;
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


    void Update()
    {
        interactCollider.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(PlayerMove.Instance.direction.y, PlayerMove.Instance.direction.x) * Mathf.Rad2Deg);

        if (interactablesInRange.Count > 0)
        {
            MakeInteraction(interactablesInRange[0]);
        }
        interactionQued = false;
    }
}
