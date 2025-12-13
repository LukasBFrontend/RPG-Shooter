using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static List<string> CharacterTags { get; } = new() { "Enemy", "Player", };
    public static List<string> PlayerTags { get; } = new() { "Player", "Player Interact", "Player Trigger" };
    class Runner : MonoBehaviour { }
    static Runner _runner;
    static Runner GetRunner()
    {
        if (!_runner)
        {
            var go = new GameObject("VisualsRunner");
            Object.DontDestroyOnLoad(go);
            _runner = go.AddComponent<Runner>();
        }
        return _runner;
    }


    public static bool VisibleToCamera(Transform transform, Camera camera)
    {
        Vector3 _viewPos = camera.WorldToViewportPoint(transform.position);

        return
            _viewPos.x >= 0 &&
            _viewPos.x <= 1 &&
            _viewPos.y >= 0 &&
            _viewPos.y <= 1 &&
            _viewPos.z > 0
        ;
    }

    static IEnumerator ExecuteFlicker(SpriteRenderer sprite, Color color, float frequency, float duration)
    {
        Color _baseColor = sprite.color;
        Color _flashColor = color;
        bool _isBaseColor = true;
        float _interval = 1f / (2 * frequency);
        float _elapsed = 0f;

        while (_elapsed < duration)
        {
            if (!sprite)
            {
                yield break;
            }

            sprite.color = _isBaseColor ? _flashColor : _baseColor;
            _isBaseColor = !_isBaseColor;

            yield return new WaitForSeconds(_interval);
            _elapsed += _interval;
        }

        sprite.color = _baseColor;
    }

    static IEnumerator ExecuteSpriteColorTransition(SpriteRenderer sprite, Color targetColor, float frequency, float duration, bool destroyAfter)
    {
        Color _baseColor = sprite.color;
        float _interval = duration / frequency;
        float _elapsed = 0f;

        while (_elapsed <= duration)
        {
            if (!sprite)
            {
                yield break;
            }

            float _t = _elapsed / (duration - _interval);
            sprite.color = Color.Lerp(_baseColor, targetColor, _t);

            yield return new WaitForSeconds(_interval);
            _elapsed += _interval;
        }

        if (destroyAfter)
        {
            Object.Destroy(sprite.gameObject);
        }
    }

    public static void FlickerSprite(SpriteRenderer sprite, Color color, float frequency, float duration)
    {
        GetRunner().StartCoroutine(ExecuteFlicker(sprite, color, frequency, duration));
    }

    /// <summary>
    /// Fades the sprite color from it's current color to clear
    /// </summary>
    /// <param name="sprite">The target sprite</param>
    /// <param name="frequency">Determines how many color transition steps ther are per second</param>
    /// <param name="duration">The total time during which the color transition takes place</param>
    /// <param name="destroyAfter">Whether to destroy the associated GameObject after the transition</param>
    /// <returns></returns>
    public static void TransitionSpriteColor(SpriteRenderer sprite, Color targetColor, float frequency, float duration, bool destroyAfter = true)
    {
        GetRunner().StartCoroutine(ExecuteSpriteColorTransition(sprite, targetColor, frequency, duration, destroyAfter));
    }

    public static List<Vector2> GetSamplePoints(Collider2D col)
    {
        List<Vector2> _points = new();

        var _b = col.bounds;
        _points.Add(_b.min);
        _points.Add(new Vector2(_b.min.x, _b.max.y));
        _points.Add(new Vector2(_b.max.x, _b.min.y));
        _points.Add(_b.max);

        return _points;
    }

    public static bool IsFullyInside(Collider2D inner, Collider2D outer)
    {
        var _points = GetSamplePoints(inner);

        ContactFilter2D _filter = new()
        {
            useTriggers = true,
            useLayerMask = false,
            useDepth = false,
        };

        Collider2D[] _results = new Collider2D[8];

        foreach (var p in _points)
        {
            Physics2D.queriesHitTriggers = true;
            int _hitCount = Physics2D.OverlapPoint(p, _filter, _results);
            Physics2D.queriesHitTriggers = false;

            bool _inside = false;

            for (int i = 0; i < _hitCount; i++)
            {
                if (_results[i].gameObject == outer.gameObject)
                {
                    _inside = true;
                    break;
                }
            }

            if (!_inside)
            {
                return false;
            }
        }

        return true;
    }

    public static List<Vector2> CompositeColliderVertices(CompositeCollider2D compositeCollider)
    {
        List<Vector2> _vertices = new();

        for (int i = 0; i < compositeCollider.pathCount; i++)
        {
            Vector2[] pathVerts = new Vector2[compositeCollider.GetPathPointCount(i)];
            compositeCollider.GetPath(i, pathVerts);
            _vertices.AddRange(pathVerts);
        }

        return _vertices;
    }

    public static List<Vector2> VertexNormalInset(List<Vector2> vertices, float inset)
    {
        List<Vector2> _insetVertices = new();
        int _count = vertices.Count;
        inset = -inset;

        for (int _i = 0; _i < _count; _i++)
        {
            Vector2 _prev = vertices[(_i - 1 + _count) % _count];
            Vector2 _curr = vertices[_i];
            Vector2 _next = vertices[(_i + 1) % _count];

            Vector2 _e1 = (_curr - _prev).normalized;
            Vector2 _e2 = (_next - _curr).normalized;

            Vector2 _n1 = new Vector2(_e1.y, -_e1.x);
            Vector2 _n2 = new Vector2(_e2.y, -_e2.x);

            Vector2 _bisector = (_n1 + _n2).normalized;

            _insetVertices.Add(_curr + _bisector * inset);
        }

        return _insetVertices;
    }

    public static Vector2 ClosestPointOnPolygon(List<Vector2> vertices, Vector2 point)
    {
        Vector2 _closest = Vector2.zero;
        float _minDist = float.MaxValue;

        int _count = vertices.Count;
        for (int _i = 0; _i < _count; _i++)
        {
            Vector2 _a = vertices[_i];
            Vector2 _b = vertices[(_i + 1) % _count];

            Vector2 _ab = _b - _a;
            float _t = Vector2.Dot(point - _a, _ab) / _ab.sqrMagnitude;
            _t = Mathf.Clamp01(_t);

            Vector2 _p = _a + _ab * _t;
            float _d = (point - _p).sqrMagnitude;

            if (_d < _minDist)
            {
                _minDist = _d;
                _closest = _p;
            }
        }

        return _closest;
    }
    public static Vector2 DirectionToClosestEdge(Vector2 point, List<Vector2> verts)
    {
        float _bestDist = float.PositiveInfinity;
        Vector2 _bestNormal = Vector2.zero;

        int _count = verts.Count;

        for (int _i = 0; _i < _count; _i++)
        {
            Vector2 _edgeA = verts[_i];
            Vector2 _edgeB = verts[(_i + 1) % _count];

            Vector2 _AB = _edgeB - _edgeA;
            Vector2 _AP = point - _edgeA;

            float _denom = Vector2.Dot(_AB, _AB);
            if (_denom == 0f)
            {
                continue;
            }

            float _t = Vector2.Dot(_AP, _AB) / _denom;
            if (_t < 0f || _t > 1f)
            {
                continue;
            }

            Vector2 _proj = _edgeA + _t * _AB;

            float _dist = Vector2.Distance(point, _proj);

            if (_dist < _bestDist)
            {
                _bestDist = _dist;

                Vector2 _normal = new Vector2(-_AB.y, _AB.x).normalized;
                _bestNormal = _normal;
            }
        }

        return _bestNormal;
    }




}
