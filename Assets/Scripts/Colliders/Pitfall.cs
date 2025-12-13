using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitfall : Trigger
{
    [SerializeField] CompositeCollider2D col;
    [SerializeField] GameObject skullPrefab;

    const float FALL_SPEED = 2.25f;
    const float FALL_DURATION = .8f;

    bool _fallTriggered = false;

    List<Vector2> _colVertices;
    List<Vector2> _insetVertices;

    void Start()
    {
        _colVertices = Utils.CompositeColliderVertices(col);
        _insetVertices = Utils.VertexNormalInset(_colVertices, .45f);
    }

    void Update()
    {
        for (int i = 0; i < _colVertices.Count; i++)
        {
            Vector2 edgeA = _colVertices[i];
            Vector2 edgeB = _colVertices[(i + 1) % _colVertices.Count];
            Debug.DrawLine(edgeA, edgeB);
        }

        for (int i = 0; i < _insetVertices.Count; i++)
        {
            Vector2 edgeA = _insetVertices[i];
            Vector2 edgeB = _insetVertices[(i + 1) % _insetVertices.Count];
            Debug.DrawLine(edgeA, edgeB);
        }

        Vector2 playerPos = Player.Config.SpriteCenter();
        Vector2 pointA = Utils.ClosestPointOnPolygon(_insetVertices, playerPos);
        Debug.DrawLine(pointA, playerPos);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Player.State.LastValidRespawn = new Vector2(10, 0);

        Character character = other.GetComponent<Character>();
        if (!character || _fallTriggered)
        {
            return;
        }

        InitiateDirectionalFall(character);
    }

    void InitiateDirectionalFall(Character character)
    {
        _fallTriggered = true;

        Player.State.SetStatusFalling(FALL_DURATION + .5f);
        Vector2 _target = Utils.ClosestPointOnPolygon(_insetVertices, character.SpriteCenter());
        Vector2 _dir = (_target - character.SpriteCenter()).normalized;

        Vector2 _fallVelocity = CalculateFallVelocity(_dir, FALL_SPEED);

        character.Rigidbody.linearVelocity = _fallVelocity;

        StartCoroutine(FallSequence(character.transform));
    }

    Vector2 CalculateFallVelocity(Vector2 direction, float baseFallSpeed)
    {
        Vector2 _velocityScale = direction;

        _velocityScale.x = Mathf.Clamp(
            _velocityScale.x,
            -.475f,
            .475f
        );

        _velocityScale.y = Mathf.Clamp(
            Mathf.Sign(_velocityScale.y) *
            Mathf.Pow(_velocityScale.y, 2),
            -1f,
            .1f
        );

        return _velocityScale * baseFallSpeed;
    }


    IEnumerator FallSequence(Transform targetTransform)
    {
        const float CAMERA_SIZE_DELTA = 3f;
        const float TIME_STEP = .1f;

        float _elapsed = 0f;

        Pixelate[] _pixelates = targetTransform.GetComponentsInChildren<Pixelate>();

        SpriteRenderer[] _sprites = targetTransform.GetComponentsInChildren<SpriteRenderer>();

        float _cameraStartSize = 0f;
        float _cameraTargetSize = 0f;

        if (_pixelates.Length > 0)
        {
            _cameraStartSize = _pixelates[0].OrtographicSize;
            _cameraTargetSize = _cameraStartSize + CAMERA_SIZE_DELTA;
        }

        foreach (SpriteRenderer sprite in _sprites)
        {
            Utils.TransitionSpriteColor(
                sprite,
                Color.black,
                8,
                FALL_DURATION,
                false
            );
        }

        while (_elapsed <= FALL_DURATION)
        {
            float _t = _elapsed / FALL_DURATION;

            foreach (Pixelate pixelate in _pixelates)
            {
                pixelate.OrtographicSize =
                    Mathf.Lerp(
                        _cameraStartSize,
                        _cameraTargetSize,
                        _t
                    );
            }

            if (
                _elapsed < FALL_DURATION - TIME_STEP &&
                _elapsed >= FALL_DURATION - TIME_STEP * 2
            )
            {
                Instantiate(
                    skullPrefab,
                    (Vector2)targetTransform.position + Vector2.up / 2f,
                    Quaternion.identity
                );
            }

            yield return new WaitForSeconds(TIME_STEP);
            _elapsed += TIME_STEP;
        }

        foreach (Pixelate pixelate in _pixelates)
        {
            pixelate.OrtographicSize = _cameraStartSize;
        }

        foreach (SpriteRenderer sprite in _sprites)
        {
            sprite.color = Color.white;
        }

        Player.State.Respawn();
        _fallTriggered = false;
    }
}
