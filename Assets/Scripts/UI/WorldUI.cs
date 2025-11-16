using System.Collections.Generic;
using UnityEngine;

public class WorldUI : MonoBehaviour
{
    List<NPC> NPCs;
    Dictionary<NPC, GameObject> healthbars = new Dictionary<NPC, GameObject>();

    [SerializeField] GameObject healthBarPrefab;

    void Start()
    {
        NPCs = new List<NPC>(FindObjectsByType<NPC>(FindObjectsSortMode.None));

        AssignHealthBars();
    }

    public void AssignHealthBars()
    {
        foreach (NPC npc in NPCs)
        {
            // If NPC already has a healthbar assigned, skip it
            if (healthbars.ContainsKey(npc))
                continue;

            // Spawn new bar
            GameObject bar = Instantiate(healthBarPrefab, transform);

            // Store association
            healthbars[npc] = bar;

            // Optional: initialize script on bar
            bar.GetComponent<NPC_Healthbar>().Initialize(npc);
        }
    }

    // Optional helper method
    public GameObject GetHealthBar(NPC npc)
    {
        if (healthbars.TryGetValue(npc, out GameObject bar))
            return bar;

        return null;
    }
}
