using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Pixelate))]
[RequireComponent(typeof(Animator))]
public class Weapon : MonoBehaviour
{
    public Player Wielder;
    [SerializeField] float recoilForce = 0f;
    [SerializeField] Transform[] applyRotation;
    public SpriteRenderer Renderer { get; private set; }
    public bool IsHolstered { get; set; }
    Pixelate _pixelate;
    AimController _controller;
    Animator _animator;
    void Cache()
    {
        Renderer = GetComponent<SpriteRenderer>();
        _pixelate = GetComponent<Pixelate>();
        _animator = GetComponent<Animator>();
        _controller = AimController.Instance;
    }

    public void ToggleHolstered()
    {
        IsHolstered = !IsHolstered;
    }

    protected void SetWeaponRotation()
    {
        if (GameState.Status != RunState.Running)
        {
            return;
        }
        else if (_pixelate == null || Renderer == null || _controller == null)
        {
            Cache();
        }

        int _playerSortOrder = Wielder.SpriteRenderer.sortingOrder;
        float _aimDirectionIndex = Utils.RotationIndexFromAngle(_controller.GetAimAngleInDegrees(), 8);

        int _layerDifference = _aimDirectionIndex >= 1 && _aimDirectionIndex <= 3
            ? -1
            : 3
        ;
        Renderer.sortingOrder = _playerSortOrder + _layerDifference;
        _pixelate.Rotation = _controller.GetAimAngleReversed();

        foreach (Transform child in applyRotation)
        {
            child.transform.rotation = _controller.GetAimAngle();
        }

        _animator.SetFloat("AimDirectionIndex", _aimDirectionIndex);
    }

    protected void Recoil()
    {
        if (recoilForce == 0)
        {
            return;
        }
        Vector2 _aimDirection = _controller.GetAimDirection();
        Vector2 _recoilDirection = new(-_aimDirection.x, -_aimDirection.y);
        Rigidbody2D _rb = Wielder.Rigidbody;
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(_recoilDirection * recoilForce);
        Wielder.State.SetStatus(CharacterStatus.Recoil, .5f);
    }
}
