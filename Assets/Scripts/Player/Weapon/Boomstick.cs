using Unity.Mathematics;
using UnityEngine;

public class Boomstick : Weapon, IInventoryItem
{
    [SerializeField] GameObject bullet;
    [SerializeField] float bulletVelocity = 10f;
    [Tooltip("Total bullet spread angle in degress")]
    [Range(0, 90)]
    [SerializeField] float spreadAngle;
    [Range(1, 10)]
    [SerializeField] int projectileCount = 3;
    [Range(0, 1)]
    [SerializeField] int count = 1;
    public int Count { get; set; }
    public string Name { get; } = "Boomstick";

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

        float aimAngle = GetAimAngle().eulerAngles.z;
        float minAngle = aimAngle - spreadAngle / 2;
        float maxAngle = aimAngle + spreadAngle / 2;

        for (int i = 0; i < projectileCount; i++)
        {
            float bulletAngle = Mathf.LerpAngle(minAngle, maxAngle, (float)i / (float)projectileCount);
            Vector2 bulletDirection = new(Mathf.Cos(Mathf.Deg2Rad * bulletAngle), Mathf.Sin(Mathf.Deg2Rad * bulletAngle));

            GameObject bulletInstance = Instantiate(bullet, aimDirection * 1.25f, GetAimAngle(), null);

            bulletInstance.layer = PlayerConfig.Instance.gameObject.layer + 3;
            bulletInstance.GetComponent<Rigidbody2D>().linearVelocity = bulletDirection * bulletVelocity;
            bulletInstance.transform.SetPositionAndRotation(bulletInstance.transform.position + gameObject.transform.position, Quaternion.Euler(0, 0, 0));
        }


        Recoil();
    }
}
