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
    bool _isFocusing;


    void LateUpdate()
    {
        if (!_isFocusing)
        {
            AimWithMove();
        }
        SetWeaponRotation();
        _isFocusing = false;
    }

    public void Focus()
    {
        AimWithMouse();
        _isFocusing = true;
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
