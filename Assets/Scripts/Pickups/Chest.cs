using System;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, Interactable, IObstructive
{
    [SerializeField] Animator animator;
    [SerializeField] Collider2D col;

    bool isOpen = false;

    Vector2 Size
    {
        get
        {
            return new(2, 2);
        }
    }

    /* IObstructive */
    public Bounds OccupationBounds(float offSet = 0f)
    {
        Bounds bounds = col.bounds;
        bounds.Expand(offSet);

        return bounds;
    }

    /* IObstructive */

    public void Interact()
    {
        isOpen = !isOpen;
        animator.SetBool("IsOpen", isOpen);
    }
    void SpawnLoot()
    {

    }
}
