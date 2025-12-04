using UnityEngine;

public class FogScript : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] Color darkColor;
    [SerializeField] Color middleColor;
    [SerializeField] Color lightColor;
    [SerializeField] SpriteRenderer fog1;
    [SerializeField] SpriteRenderer fog2;
    [SerializeField] SpriteRenderer fog3;

    Color[] _colors, _startColors, _targetColors;
    SpriteRenderer[] _filters;
    int _zIndex;
    float _transitionDuration = .25f; // default 2 seconds
    float _transitionTimer;
    bool _isTransitioning;

    void Start()
    {
        _colors = new Color[] { lightColor, middleColor, darkColor };
        _filters = new SpriteRenderer[] { fog2, fog1, fog3 };
        _startColors = new Color[_filters.Length];
        _targetColors = new Color[_filters.Length];
    }

    void Update()
    {
        UpdateFog();

        if (_isTransitioning)
        {
            ColorTransitionStep();
        }
    }

    void UpdateFog()
    {
        int _z0 = 16;
        _zIndex = player.layer - _z0;

        int _length = _colors.Length - 1;
        var _newTargetColors = new Color[_colors.Length];

        for (int i = 0; i < _colors.Length; i++)
        {
            _newTargetColors[i] = _colors[Mathf.Clamp(i - _length + _zIndex, 0, _length)];
        }

        for (int i = 0; i < _newTargetColors.Length; i++)
        {
            if (_newTargetColors[i] != _targetColors[i])
            {
                StartColorTransition(_newTargetColors, _transitionDuration);
                break;
            }
        }
    }

    void StartColorTransition(Color[] newTargets, float time)
    {
        for (int i = 0; i < _filters.Length; i++)
        {
            _startColors[i] = _filters[i].color;
            _targetColors[i] = newTargets[i];
        }

        _transitionDuration = time;
        _transitionTimer = 0f;
        _isTransitioning = true;
    }

    void ColorTransitionStep()
    {
        _transitionTimer += Time.deltaTime;
        float _t = Mathf.Clamp01(_transitionTimer / _transitionDuration);

        for (int i = 0; i < _filters.Length; i++)
        {
            _filters[i].color = Color.Lerp(_startColors[i], _targetColors[i], _t);
        }

        if (_t >= 1f)
        {
            _isTransitioning = false;
        }
    }
}
