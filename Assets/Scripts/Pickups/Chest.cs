using System;
using UnityEngine;

public class Chest : MonoBehaviour, Interactable
{
    [SerializeField] Animator animator;
    bool isOpen = false;

    Vector2 Size
    {
        get
        {
            return new(2, 2);
        }
    }

    public void Interact()
    {
        isOpen = !isOpen;
        animator.SetBool("IsOpen", isOpen);
    }

    void SpawnLoot()
    {

    }
}
