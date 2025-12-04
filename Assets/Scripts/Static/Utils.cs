using System.Collections;
using UnityEngine;

public static class Utils
{
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

    static IEnumerator ExecuteFlicker(SpriteRenderer sprite, float frequency, float duration)
    {
        Color _baseColor = sprite.color;
        Color _flashColor = Color.red;
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

    public static void Flicker(SpriteRenderer sprite, float frequency, float duration)
    {
        GetRunner().StartCoroutine(ExecuteFlicker(sprite, frequency, duration));
    }
}
