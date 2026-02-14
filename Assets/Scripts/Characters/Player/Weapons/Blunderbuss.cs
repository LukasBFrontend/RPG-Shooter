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
        Instantiate(fireVFX, (Vector2)gameObject.transform.position + AimController.Instance.GetAimDirection() * 1, AimController.Instance.GetAimAngle(), null);
        attack.Attempt(Wielder, fireArea.TargetsInRange.ToArray());
        Recoil();
    }
}
