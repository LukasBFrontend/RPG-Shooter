using System.Collections.Generic;
using UnityEngine;

public class WorldUI : MonoBehaviour
{
    [SerializeField] GameObject healthBarPrefab;
    List<NPC> _npcs;
    Dictionary<NPC, GameObject> _healthbars = new();

    void Start()
    {
        _npcs = new List<NPC>(FindObjectsByType<NPC>(FindObjectsSortMode.None));

        AssignHealthBars();
    }

    void AssignHealthBars()
    {
        foreach (NPC npc in _npcs)
        {
            if (_healthbars.ContainsKey(npc))
            {
                continue;
            }
            GameObject _bar = Instantiate(healthBarPrefab, transform);
            _healthbars[npc] = _bar;

            _bar.GetComponent<NPC_Healthbar>().AssignToNPC(npc);
        }
    }

    public GameObject GetHealthBar(NPC npc)
    {
        if (_healthbars.TryGetValue(npc, out GameObject bar))
            return bar;

        return null;
    }
}
