using UnityEngine;

public class Skull : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    void Awake()
    {
        Utils.TransitionSpriteColor(spriteRenderer, Color.clear, 15f, 1.5f);
    }
}
