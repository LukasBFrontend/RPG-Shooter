using UnityEngine;

public class PlayerAction : Singleton<PlayerAction>
{
    [SerializeField] Flintlock flintlock;
    public void FireWeapon()
    {
        flintlock.Fire();
    }
}
