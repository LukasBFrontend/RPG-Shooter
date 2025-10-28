using Unity.Mathematics;
using UnityEngine;

public class Flintlock : Weapon
{
    [SerializeField] GameObject bullet;
    [SerializeField] float bulletVelocity = 5f;

    void Start()
    {

    }

    void Update()
    {
        SetAimAngle();
        SetWeaponRotation();
    }



    public void Fire()
    {
        Vector2 aimDirection = GetAimDirection();
        GameObject bulletInstance = Instantiate(bullet, aimDirection * 1, GetAimAngle(), transform);
        bulletInstance.GetComponent<Rigidbody2D>().linearVelocity = aimDirection * bulletVelocity;
        bulletInstance.transform.position = bulletInstance.transform.position + transform.position;

        Recoil();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        //Debug.DrawLine(player.transform.position, GetAimDirection());
    }
}
