using Unity.Mathematics;
using UnityEngine;

public class Flintlock : Weapon, IInventoryItem
{
    [Header("Inventory Item Info")]
    [Range(0, 1)]
    [SerializeField] int count = 1;
    [Header("Attack")]
    [SerializeField] GameObject bullet;
    [SerializeField] float bulletVelocity = 5f;
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
        Vector2 aimDirection = GetAimDirection();
        GameObject bulletInstance = Instantiate(bullet, (Vector2)gameObject.transform.position + aimDirection * 1, GetAimAngle(), null);
        bulletInstance.layer = PlayerConfig.Instance.gameObject.layer + 3;
        bulletInstance.GetComponent<Rigidbody2D>().linearVelocity = aimDirection * bulletVelocity;

        Recoil();
    }
}
