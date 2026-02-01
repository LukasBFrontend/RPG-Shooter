using UnityEngine;

public class Blunderbuss : Weapon, IInventoryItem
{
    [SerializeField] Attack attack;
    [SerializeField] GameObject fireVFX;
    [SerializeField] FireArea fireArea;
    [Header("Inventory Item Info")]
    [SerializeField]
    Sprite sprite;
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
        fireArea.transform.rotation = GetAimAngle();
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
        Instantiate(fireVFX, (Vector2)gameObject.transform.position + GetAimDirection() * 1, GetAimAngle(), null);
        attack.Attempt(Wielder, fireArea.TargetsInRange.ToArray());
        Recoil();
    }
}
