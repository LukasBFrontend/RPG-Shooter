using System.Collections.Generic;
using UnityEngine;

public class Blunderbuss : Weapon, IInventoryItem
{
    [SerializeField] Attack attack;
    [SerializeField] Attack farAttack;
    [SerializeField] GameObject fireVFX;
    [SerializeField] FireArea fireArea;
    [Header("Inventory Item Info")]
    [SerializeField]
    Sprite sprite;
    [Range(0, 1)]
    [SerializeField] int count = 1;
    public string Name
    {
        get => gameObject.name;
    }
    public int Count
    {
        get => count;
        set => count = value;
    }
    public GameObject GameObject
    {
        get => gameObject;
    }
    public Sprite UI_Sprite
    {
        get => sprite;
    }


    void LateUpdate()
    {
        SetWeaponRotation();
    }

    public void Focus()
    {
        AimController.Instance.AimWithMouse();
    }

    List<NPC> _targetsClose = new();
    List<NPC> _targetsFar = new();


    public void Use()
    {
        if (attack.OnCooldown || IsHolstered)
        {
            return;
        }
        Instantiate(fireVFX, (Vector2)gameObject.transform.position + AimController.Instance.GetAimDirection() * 1, AimController.Instance.GetAimAngle(), null);

        SubdivideTargets(fireArea.TargetsInRange);
        attack.Attempt(Wielder, _targetsClose.ToArray());
        farAttack.Attempt(Wielder, _targetsFar.ToArray());
        Recoil();
    }

    void SubdivideTargets(List<NPC> targets)
    {
        _targetsClose.Clear();
        _targetsFar.Clear();
        foreach (NPC npc in targets)
        {
            if (Vector2.Distance(npc.ColliderCenter(), Wielder.ColliderCenter()) < Vector2.Distance(fireArea.GetComponent<Collider2D>().bounds.center, Wielder.ColliderCenter()))
            {
                _targetsClose.Add(npc);
            }
            else
            {
                _targetsFar.Add(npc);
            }
        }
    }
}
