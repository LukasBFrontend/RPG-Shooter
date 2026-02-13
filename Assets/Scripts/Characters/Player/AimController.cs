using System;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class AimController : Singleton<AimController>
{
    public bool IsControlledWithMouse { get; private set; } = false;
    float _aimAngle;
    Vector2 _aimDirection;
    bool _isFocusing;
    Player _player;
    const float AIM_SPEED = 720f;

    void Start()
    {
        _player = GetComponent<Player>();
    }

    void LateUpdate()
    {
        if (!_isFocusing)
        {
            AimWithMove();
            IsControlledWithMouse = false;
        }
        else
        {
            IsControlledWithMouse = true;
        }
        _isFocusing = false;
    }

    public void AimWithMouse()
    {
        _isFocusing = true;
        Vector2 _mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 _mouseToPlayer = _mouseWorld - _player.SpriteCenter();

        float _targetAngle = Mathf.Atan2(_mouseToPlayer.y, _mouseToPlayer.x) * Mathf.Rad2Deg;

        float _delta = Mathf.DeltaAngle(_aimAngle, _targetAngle);

        if (Mathf.Abs(Mathf.Abs(_delta) - 180f) < 0.0001f)
        {
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
        _player.SetFacing(_aimDirection);
    }

    protected void AimWithMove()
    {
        float _targetAngle = Mathf.Atan2(_player.FaceDir.y, _player.FaceDir.x) * Mathf.Rad2Deg;

        float _delta = Mathf.DeltaAngle(_aimAngle, _targetAngle);

        if (Mathf.Abs(Mathf.Abs(_delta) - 180f) < 0.0001f)
        {
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

    public Vector2 GetAimDirection()
    {
        return _aimDirection;
    }

    /// <summary>
    /// Get the aim angle in degrees.
    /// </summary>
    /// <returns>A value between -180f and 180f, where 0 represents the righand direction.</returns>
    public float GetAimAngleInDegrees()
    {
        return _aimAngle;
    }

    public Quaternion GetAimAngle()
    {
        return Quaternion.Euler(0, 0, _aimAngle);
    }
    public Quaternion GetAimAngleReversed()
    {
        return Quaternion.Euler(0, 0, -_aimAngle - 180);
    }
}
