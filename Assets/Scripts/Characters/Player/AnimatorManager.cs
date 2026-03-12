using UnityEngine;

[RequireComponent(typeof(Player))]
public class AnimatorManager : MonoBehaviour
{
    [SerializeField] Animator animator;
    AimController _aimController;
    Controller _moveController;
    const float DEADZONE = 0.1f;
    void Start()
    {
        Cache();
    }
    void Update()
    {
        SetAnimatorValues();
    }

    void Cache()
    {
        _aimController = GameState.Player.AimController;
        _moveController = GameState.Player.Controller;
    }


    void SetAnimatorValues()
    {
        Vector2 _moveInput = _moveController.Input;
        float _moveSpeed = _moveInput.magnitude;
        bool _isAimingMouse = _aimController.IsControlledWithMouse;
        float _directionIndex = -1;

        float _movementAngle = Mathf.Atan2(_moveInput.normalized.y, _moveInput.normalized.x) * Mathf.Rad2Deg;
        float _aimAngle = _aimController.GetAimAngleInDegrees();

        if (_moveSpeed > DEADZONE)
        {
            _directionIndex = Utils.RotationIndexFromAngle(_isAimingMouse ? _aimAngle : _movementAngle, 8);
        }
        else if (_isAimingMouse)
        {
            _directionIndex = Utils.RotationIndexFromAngle(_aimAngle, 8);
        }

        if (_directionIndex != -1)
        {
            animator.SetFloat("DirectionIndex", _directionIndex);
        }
        animator.SetFloat("Speed", _moveSpeed);
    }
}
