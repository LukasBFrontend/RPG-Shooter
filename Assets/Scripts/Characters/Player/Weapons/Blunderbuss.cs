using UnityEngine;

public class Blunderbuss : Weapon, IInventoryItem
{
    [SerializeField] Attack attack;
    [SerializeField] GameObject fireVFX;
    [SerializeField] FireArea fireArea;
    [Header("Inventory Item Info")]
    [Range(0, 1)]
    [SerializeField] int count = 1;
    public int Count { get; set; }
    public string Name { get; } = "Blunderbuss";

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
        fireArea.transform.rotation = GetAimAngle();
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
