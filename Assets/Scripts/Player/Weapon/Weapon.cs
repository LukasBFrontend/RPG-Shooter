using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Base")]
    [SerializeField] protected GameObject player;
    [SerializeField] float recoilForce = 0f;
    float _aimAngle;
    Vector2 _aimDirection;

    protected void SetAimAngle()
    {
        Vector3 _mouseScreen = Input.mousePosition;
        Vector2 _mouseWorld = Camera.main.ScreenToWorldPoint(_mouseScreen);
        Vector2 _playerToCamera = player.transform.position - Camera.main.transform.position;

        Vector2 _mouseToPlayer = _mouseWorld - _playerToCamera;
        _aimDirection = _mouseToPlayer.normalized;
        _aimAngle = Mathf.Atan2(_mouseToPlayer.y, _mouseToPlayer.x) * Mathf.Rad2Deg;
    }


    protected void SetWeaponRotation()
    {
        if (GameState.Status != RunState.Running)
        {
            return;
        }

        int _playerSortOrder = PlayerConfig.Instance.SpriteRenderer.sortingOrder;
        Pixelate _pixelate = gameObject.GetComponent<Pixelate>();

        bool _behindCharacter = _aimAngle > 0;
        bool _flipWeapon = _aimAngle < 90 && _aimAngle > -90;

        if (_flipWeapon)
        {
            _pixelate.RotateQuad(0, 180, 0);
        }
        else
        {
            _pixelate.RotateQuad(0, 0, 0);
        }

        if (TryGetComponent<SpriteRenderer>(out var _renderer))
        {
            _renderer.sortingOrder = _behindCharacter ? _playerSortOrder - 1 : _playerSortOrder + 1;
        }
        _pixelate.Rotation = _flipWeapon ? GetAimAngle() : GetAimAngleReversed();
    }
    protected Vector2 GetAimDirection()
    {
        return _aimDirection;
    }

    protected void Recoil()
    {
        Vector2 _recoilDirection = new(-_aimDirection.x, -_aimDirection.y);
        Rigidbody2D _rb = PlayerConfig.Instance.Rb;
        _rb.AddForce(_recoilDirection * recoilForce);
        PlayerConfig.Instance.Status = PlayerStatus.Recoil;
    }

    protected Quaternion GetAimAngle()
    {
        return Quaternion.Euler(0, 0, _aimAngle);
    }
    protected Quaternion GetAimAngleReversed()
    {
        return Quaternion.Euler(0, 0, -_aimAngle - 180);
    }
}
