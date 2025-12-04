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
        Vector2 _aimDirection = GetAimDirection();
        GameObject _bulletInstance = Instantiate(bullet, (Vector2)gameObject.transform.position + _aimDirection * 1, GetAimAngle(), null);
        _bulletInstance.layer = PlayerConfig.Instance.gameObject.layer + 3;
        _bulletInstance.GetComponent<Rigidbody2D>().linearVelocity = _aimDirection * bulletVelocity;

        Recoil();
    }
}
