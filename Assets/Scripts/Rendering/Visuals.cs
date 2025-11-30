using System;
using System.Collections;
using UnityEngine;

public class Visuals : Singleton<Visuals>
{
    private IEnumerator ExecuteFlicker(SpriteRenderer sprite, float frequency, float duration)
    {
        Color baseColor = sprite.color;
        Color flashColor = Color.red;
        bool isBaseColor = true;
        float interval = 1f / (2 * frequency);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (!sprite)
            {
                yield break;
            }

            sprite.color = isBaseColor ? flashColor : baseColor;
            isBaseColor = !isBaseColor;

            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        sprite.color = baseColor;
    }

    public void Flicker(SpriteRenderer sprite, float frequency, float duration)
    {
        StartCoroutine(ExecuteFlicker(sprite, frequency, duration));
    }
}
