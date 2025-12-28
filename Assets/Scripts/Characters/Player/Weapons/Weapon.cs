using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Pixelate))]
public class Weapon : MonoBehaviour
{
    public Player Wielder;
    [SerializeField] float recoilForce = 0f;
    public SpriteRenderer Renderer { get; private set; }
    float _aimAngle;
    Vector2 _aimDirection;
    Pixelate _pixelate;
    const float AIM_SPEED = 720f;

    void Cache()
    {
        Renderer = GetComponent<SpriteRenderer>();
        _pixelate = GetComponent<Pixelate>();
    }
    protected void AimWithMouse()
    {
        Vector3 _mouseScreen = Input.mousePosition;
        Vector2 _mouseWorld = Camera.main.ScreenToWorldPoint(_mouseScreen);
        Vector2 _wielderToCamera = Wielder.transform.position - Camera.main.transform.position;

        Vector2 _mouseToPlayer = _mouseWorld - _wielderToCamera;

        float _targetAngle = Mathf.Atan2(_mouseToPlayer.y, _mouseToPlayer.x) * Mathf.Rad2Deg;

        float _delta = Mathf.DeltaAngle(_aimAngle, _targetAngle);

        // Handle the 180° ambiguity
        if (Mathf.Abs(Mathf.Abs(_delta) - 180f) < 0.0001f)
        {
            // Decide direction based on which semicircle we're in
            // (-180..0) → positive, (0..-180) → negative
            float bias = (_aimAngle < 0f) ? +0.001f : -0.001f;
            _targetAngle -= bias;
        }

        _aimAngle = Mathf.MoveTowardsAngle(
            _aimAngle,
            _targetAngle,
            AIM_SPEED * Time.deltaTime
        );

        _aimDirection = new Vector2(
            Mathf.Cos(_aimAngle * Mathf.Deg2Rad),
            Mathf.Sin(_aimAngle * Mathf.Deg2Rad)
        );
    }

    protected void AimWithMove()
    {
        float _targetAngle = Mathf.Atan2(Wielder.FaceDir.y, Wielder.FaceDir.x) * Mathf.Rad2Deg;

        float _delta = Mathf.DeltaAngle(_aimAngle, _targetAngle);

        // Handle the 180° ambiguity
        if (Mathf.Abs(Mathf.Abs(_delta) - 180f) < 0.0001f)
        {
            // Decide direction based on which semicircle we're in
            // (-180..0) → positive, (0..-180) → negative
            float bias = (_aimAngle < 0f) ? +0.001f : -0.001f;
            _targetAngle -= bias;
        }

        _aimAngle = Mathf.MoveTowardsAngle(
            _aimAngle,
            _targetAngle,
            AIM_SPEED * Time.deltaTime
        );

        _aimDirection = new Vector2(
            Mathf.Cos(_aimAngle * Mathf.Deg2Rad),
            Mathf.Sin(_aimAngle * Mathf.Deg2Rad)
        );
    }



    protected void SetWeaponRotation()
    {
        if (GameState.Status != RunState.Running)
        {
            return;
        }
        else if (_pixelate == null || Renderer == null)
        {
            Cache();
        }

        int _playerSortOrder = Wielder.SpriteRenderer.sortingOrder;

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

        Renderer.sortingOrder = _behindCharacter ? _playerSortOrder - 1 : _playerSortOrder + 3;
        _pixelate.Rotation = _flipWeapon ? GetAimAngle() : GetAimAngleReversed();
    }

    protected Vector2 GetAimDirection()
    {
        return _aimDirection;
    }

    protected void Recoil()
    {
        if (recoilForce == 0)
        {
            return;
        }
        Vector2 _recoilDirection = new(-_aimDirection.x, -_aimDirection.y);
        Rigidbody2D _rb = Wielder.Rigidbody;
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(_recoilDirection * recoilForce);
        Wielder.State.SetStatus(CharacterStatus.Recoil, .5f);
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
