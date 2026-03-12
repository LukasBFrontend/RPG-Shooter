using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PitfallTrigger : MonoBehaviour
{
    const float FALL_SPEED = 2.25f;
    const float FALL_DURATION = .8f;
    [SerializeField] CompositeCollider2D col;
    [SerializeField] GameObject skullPrefab;
    [SerializeField] bool DrawDebugLines;

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
        if (DrawDebugLines)
        {
            DrawColliders();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Character _character = other.GetComponent<Character>();

        if (_character is Player)
        {
            Node _closestNode = NodeManager.Instance.ClosestNode((Vector2)other.transform.position);

            if (_closestNode == null)
            {
                Debug.LogError($"{typeof(NodeManager).Name}.Instance.ClosestNode() returned null. Respawn defaulted to (0, 0)");
                GameState.Player.LastValidRespawn = Vector2.zero;
            }
            GameState.Player.LastValidRespawn = _closestNode.transform.position;

        }
        else
        {
            return;
        }

        if (!_character || _fallTriggered || _character.State.Status == CharacterStatus.Recoil)
        {
            return;
        }

        InitiateFall(_character, true);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        Character _character = other.GetComponent<Character>();

        if (_character is not Player)
        {
            return;
        }

        if (!_character || _fallTriggered || _character.State.Status == CharacterStatus.Recoil)
        {
            return;
        }

        InitiateFall(_character, !IsFullyInside(_character.SpriteRenderer));
    }

    bool IsFullyInside(SpriteRenderer spriteRenderer)
    {
        Physics2D.queriesHitTriggers = true;
        foreach (Vector2 vertice in spriteRenderer.sprite.vertices)
        {
            Vector2 _worldPoint = vertice / 2 + (Vector2)spriteRenderer.bounds.center;
            Collider2D[] _hits = Physics2D.OverlapPointAll(_worldPoint);

            if (!_hits.Contains(col))
            {
                return false;
            }
        }
        Physics2D.queriesHitTriggers = false;

        return true;
    }

    IEnumerator FallSequence(Character character)
    {
        const float CAMERA_SIZE_DELTA = 3f;
        const float TIME_STEP = .1f;

        float _elapsed = 0f;

        Pixelate[] _pixelates = character.GetComponentsInChildren<Pixelate>();

        SpriteRenderer[] _sprites = character.GetComponentsInChildren<SpriteRenderer>();

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
                    (Vector2)character.transform.position + Vector2.up / 2f,
                    Quaternion.identity
                );
            }

            yield return new WaitForSeconds(TIME_STEP);
            _elapsed += TIME_STEP;
        }

        ResetFall(character, _cameraStartSize);
        _fallTriggered = false;
    }

    void InitiateFall(Character character, bool directional)
    {
        _fallTriggered = true;

        character.State.SetStatus(CharacterStatus.Falling, FALL_DURATION + .5f);

        Vector2 _target = Utils.ClosestPointOnPolygon(_insetVertices, character.SpriteCenter());
        Vector2 _dir = (_target - character.SpriteCenter()).normalized;

        Vector2 _fallVelocity = CalculateFallVelocity(_dir, FALL_SPEED);

        character.Rigidbody.linearVelocity = directional
            ? _fallVelocity
            : Vector2.zero
        ;

        StartCoroutine(FallSequence(character));
    }

    void ResetFall(Character character, float cameraStartSize)
    {
        Pixelate[] _pixelates = character.GetComponentsInChildren<Pixelate>();
        SpriteRenderer[] _sprites = character.GetComponentsInChildren<SpriteRenderer>();

        foreach (Pixelate pixelate in _pixelates)
        {
            pixelate.OrtographicSize = cameraStartSize;
        }

        foreach (SpriteRenderer sprite in _sprites)
        {
            sprite.color = Color.white;
        }

        if (character is Player _player)
        {
            _player.Respawn();
        }

        character.Rigidbody.linearVelocity = Vector2.zero;
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

    void DrawColliders()
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

        Vector2 playerPos = GameState.Player.SpriteCenter();
        Vector2 pointA = Utils.ClosestPointOnPolygon(_insetVertices, playerPos);
        Debug.DrawLine(pointA, playerPos);
    }
}
