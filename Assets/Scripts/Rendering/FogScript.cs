using UnityEngine;
using UnityEngine.UI;

public class FogScript : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] Color darkColor;
    [SerializeField] Color middleColor;
    [SerializeField] Color lightColor;
    [SerializeField] SpriteRenderer fog1;
    [SerializeField] SpriteRenderer fog2;
    [SerializeField] SpriteRenderer fog3;

    Color[] colors, startColors, targetColors;
    SpriteRenderer[] filters;

    int zIndex;

    // Transition tracking
    float transitionDuration = .25f; // default 2 seconds
    float transitionTimer;
    bool isTransitioning;

    void Start()
    {
        colors = new Color[] { lightColor, middleColor/* , darkColor */ };
        filters = new SpriteRenderer[] { fog2, fog1/* , fog3 */ };
        startColors = new Color[filters.Length];
        targetColors = new Color[filters.Length];
    }

    void Update()
    {
        UpdateFog();

        if (isTransitioning)
        {
            ColorTransitionStep();
        }
    }

    void UpdateFog()
    {
        int z0Index = 16;
        zIndex = player.layer - z0Index;

        int length = colors.Length - 1;
        var newTargetColors = new Color[colors.Length];

        for (int i = 0; i < colors.Length; i++)
        {
            newTargetColors[i] = colors[Mathf.Clamp(i - length + zIndex, 0, length)];
        }

        // If target colors change, start a new transition
        for (int i = 0; i < newTargetColors.Length; i++)
        {
            if (newTargetColors[i] != targetColors[i])
            {
                StartColorTransition(newTargetColors, transitionDuration);
                break;
            }
        }
    }

    void StartColorTransition(Color[] newTargets, float time)
    {
        // Copy current colors as start
        for (int i = 0; i < filters.Length; i++)
        {
            startColors[i] = filters[i].color;
            targetColors[i] = newTargets[i];
        }

        transitionDuration = time;
        transitionTimer = 0f;
        isTransitioning = true;
    }

    void ColorTransitionStep()
    {
        transitionTimer += Time.deltaTime;
        float t = Mathf.Clamp01(transitionTimer / transitionDuration);

        for (int i = 0; i < filters.Length; i++)
        {
            filters[i].color = Color.Lerp(startColors[i], targetColors[i], t);
        }

        if (t >= 1f)
        {
            isTransitioning = false;
        }
    }
}
