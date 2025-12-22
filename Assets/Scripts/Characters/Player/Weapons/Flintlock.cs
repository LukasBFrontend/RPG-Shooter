using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Flintlock : Weapon, IInventoryItem
{
    [SerializeField] Attack attack;
    [Header("Inventory Item Info")]
    [Range(0, 1)]
    [SerializeField] int count = 1;
    public int Count { get; set; }
    public string Name { get; } = "Flintlock";

    void Start()
    {
        Cache();
    }

    void Cache()
    {
        Count = count;
    }

    void Update()
    {
        SetAimAngle();
        SetWeaponRotation();
    }



    public void Use()
    {
        if (attack.OnCooldown)
        {
            return;
        }
        Vector2 _aimDirection = GetAimDirection();

        attack.Attempt(Wielder, _aimDirection);

        Recoil();
    }
}
