using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Flintlock : Weapon, IInventoryItem
{
    [SerializeField] Attack attack;
    [Header("Inventory Item Info")]
    [SerializeField] Sprite sprite;
    [Range(0, 1)]
    [SerializeField] int count = 1;
    public string Name
    {
        get => gameObject.name;
    }
    public int Count
    {
        get => count;
        set => count = value;
    }
    public GameObject GameObject
    {
        get => gameObject;
    }
    public Sprite UI_Sprite
    {
        get => sprite;
    }


    void LateUpdate()
    {
        SetWeaponRotation();
    }

    public void Focus()
    {
        AimController.Instance.AimWithMouse();
    }

    public void Use()
    {
        if (attack.OnCooldown || IsHolstered)
        {
            return;
        }
        Vector2 _aimDirection = AimController.Instance.GetAimDirection();

        attack.Attempt(Wielder, _aimDirection);

        Recoil();
    }
}
