using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class SpikeBallController : MonoBehaviour
{
    [SerializeField] float MoveSpeed = 2;
    Rigidbody2D _rigidBody;
    Vector2 _currentDir = new Vector2(1, 1).normalized;
    bool _directionChangeEnable = true;
    void Start()
    {
        Cache();
    }

    void Cache()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.linearVelocity = _currentDir * MoveSpeed;
    }

    IEnumerator CollisionTimer(int frames)
    {
        _directionChangeEnable = false;
        int _elapsed = 0;

        while (_elapsed <= frames)
        {

            yield return new WaitForEndOfFrame();
            _elapsed++;
        }

        _directionChangeEnable = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_directionChangeEnable)
        {
            return;
        }
        Vector2 _closest = collision.collider.ClosestPoint(transform.position);
        Vector2 _dir = _closest - (Vector2)transform.position;
        Vector2 _multiple;

        float _absoluteX = Mathf.Abs(_dir.x);
        float _absoluteY = Mathf.Abs(_dir.y);

        if (_absoluteX > _absoluteY)
        {
            _multiple = new(-1, 1);
        }
        else
        {
            _multiple = new(1, -1);
        }
        _currentDir *= _multiple;

        _rigidBody.linearVelocity = _currentDir * MoveSpeed;
        StartCoroutine(CollisionTimer(2));
    }

    public void AnchorToSlot()
    {
        //
    }
}
