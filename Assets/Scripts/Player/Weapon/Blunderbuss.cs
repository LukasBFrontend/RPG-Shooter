using UnityEngine;

public class Blunderbuss : Weapon, IInventoryItem
{
    [Header("Inventory Item Info")]
    [Range(0, 1)]
    [SerializeField] int count = 1;
    [Header("Attack")]
    [SerializeField] GameObject fireVFX;
    [SerializeField] FireArea fireArea;
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
        Instantiate(fireVFX, (Vector2)gameObject.transform.position + GetAimDirection() * 1, GetAimAngle(), null);
        fireArea.ExecuteAttack();

        Recoil();
    }
}
