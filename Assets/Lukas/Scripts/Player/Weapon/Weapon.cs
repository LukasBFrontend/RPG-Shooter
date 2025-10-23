using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected GameObject player;
    [SerializeField] float recoilForce = 0f;
    float aimAngle;
    Vector2 aimDirection;

    protected void SetAimAngle()
    {
        Vector3 mouseScreen = Input.mousePosition;
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        Vector2 playerToCamera = player.transform.position - Camera.main.transform.position;

        Vector2 mouseToPlayer = mouseWorld - playerToCamera;
        aimDirection = mouseToPlayer.normalized;
        aimAngle = Mathf.Atan2(mouseToPlayer.y, mouseToPlayer.x) * Mathf.Rad2Deg;
    }


    protected void SetWeaponRotation()
    {
        int playerSortOrder = PlayerConfig.Instance.SpriteRenderer.sortingOrder;
        Pixelate pixelate = gameObject.GetComponent<Pixelate>();

        bool behindCharacter = aimAngle > 0;
        bool flipWeapon = aimAngle < 90 && aimAngle > -90;
        if (flipWeapon) pixelate.RotateQuad(0, 180, 0);
        else pixelate.RotateQuad(0, 0, 0);
        pixelate.SetSortingOrder(behindCharacter ? playerSortOrder - 1 : playerSortOrder + 1);
        pixelate.rotation = flipWeapon ? GetAimAngle() : GetAimAngleReversed();
    }
    protected Vector2 GetAimDirection()
    {
        return aimDirection;
    }

    protected void Recoil()
    {
        Vector2 recoilDirection = new(-aimDirection.x, -aimDirection.y);
        Rigidbody2D rb = PlayerConfig.Instance.Rb;
        rb.AddForce(recoilDirection * recoilForce);
        PlayerConfig.Instance.Status = PlayerStatus.Recoil;
    }

    protected Quaternion GetAimAngle()
    {
        return Quaternion.Euler(0, 0, aimAngle);
    }
    protected Quaternion GetAimAngleReversed()
    {
        return Quaternion.Euler(0, 0, -aimAngle - 180);
    }
}
