using UnityEngine;

public class Chest : MonoBehaviour, IInteractable, IObstructive
{
    [SerializeField] Animator animator;
    [SerializeField] Collider2D col;
    bool _isOpen = false;

    public Bounds OccupationBounds(float offSet = 0f)
    {
        Bounds _bounds = col.bounds;
        _bounds.Expand(offSet);

        return _bounds;
    }

    public void Interact()
    {
        _isOpen = !_isOpen;
        animator.SetBool("IsOpen", _isOpen);
    }
    void SpawnLoot()
    {

    }
}
