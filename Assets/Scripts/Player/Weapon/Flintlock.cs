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
        Vector2 aimDirecdtion = GetAimDirection();
        GameObject bulletInstance = Instantiate(bullet, player.transform.position, quaternion.identity);
        bulletInstance.GetComponent<Rigidbody2D>().linearVelocity = GetAimDirection() * bulletVelocity;

        Quaternion rotation = GetAimAngle();
        bulletInstance.transform.rotation = rotation;

        Recoil();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        //Debug.DrawLine(player.transform.position, GetAimDirection());
    }
}
