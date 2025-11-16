using System;
using UnityEngine;

public class NPC_Healthbar : MonoBehaviour
{
    public NPC npc;
    [SerializeField] RectTransform healthFill;
    [SerializeField] Vector2 positionOffset = new(0, 1);
    int maxHealth;

    public void Initialize(NPC npc)
    {
        this.npc = npc;
        maxHealth = npc.health;
        RenderHealth();
    }

    void Update()
    {
        if (!npc)
        {
            Destroy(transform.gameObject);
            return;
        }
        transform.position = (Vector2)npc.transform.position + positionOffset;
        if (npc.health == maxHealth)
        {
            return;
        }
        RenderHealth();
    }

    void RenderHealth()
    {
        float healthActual = (float)npc.health / (float)maxHealth;

        healthFill.anchorMax = new(healthActual, healthFill.anchorMax.y);
    }
}
