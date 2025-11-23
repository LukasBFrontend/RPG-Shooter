using UnityEngine;

public class PlayerAction : Singleton<PlayerAction>
{
    [SerializeField] Flintlock flintlock;
    [SerializeField] Collider2D interactCollider;
    bool interactionQued;
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

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.TryGetComponent<Interactable>(out var interactable))
        {
            return;
        }

        MakeInteraction(interactable);
    }


    void Update()
    {
        interactCollider.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(PlayerMove.Instance.direction.y, PlayerMove.Instance.direction.x) * Mathf.Rad2Deg);
    }
}
