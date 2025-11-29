using Unity.Mathematics;
using UnityEngine;

public class Flintlock : Weapon, IInventoryItem
{
    [SerializeField] GameObject bullet;
    [SerializeField] float bulletVelocity = 5f;
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
        Vector2 aimDirection = GetAimDirection();
        GameObject bulletInstance = Instantiate(bullet, aimDirection * 1, GetAimAngle(), null);
        bulletInstance.layer = PlayerConfig.Instance.gameObject.layer + 3;
        bulletInstance.GetComponent<Rigidbody2D>().linearVelocity = aimDirection * bulletVelocity;
        bulletInstance.transform.position = bulletInstance.transform.position + gameObject.transform.position;

        Recoil();
    }
}
