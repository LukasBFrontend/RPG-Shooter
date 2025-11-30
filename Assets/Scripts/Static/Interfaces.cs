using System;
using System.Collections.Generic;
using UnityEngine;

interface Interactable
{
    void Interact();
}

interface IObstructive
{
    Bounds OccupationBounds(float offset);
}
