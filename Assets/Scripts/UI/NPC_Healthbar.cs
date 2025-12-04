using System;
using UnityEngine;

public class NPC_Healthbar : MonoBehaviour
{
    [SerializeField] RectTransform healthFill;
    [SerializeField] Vector2 positionOffset = new(0, 1);
    public NPC NPC;
    int _maxHealth;


    /// <summary>
    /// Assigns the healthbar to track target NPC
    /// </summary>
    /// <param name="npc">The non player character to track</param>
    public void AssignToNPC(NPC npc)
    {
        NPC = npc;
        _maxHealth = npc.Health;
        RenderHealth();
    }

    void Update()
    {
        if (!NPC)
        {
            Destroy(transform.gameObject);
            return;
        }

        transform.position = (Vector2)NPC.transform.position + positionOffset;

        if (NPC.Health == _maxHealth)
        {
            return;
        }
        RenderHealth();
    }

    void RenderHealth()
    {
        float _healthActual = (float)NPC.Health / (float)_maxHealth;

        healthFill.anchorMax = new(_healthActual, healthFill.anchorMax.y);
    }
}
