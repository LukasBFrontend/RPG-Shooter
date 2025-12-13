using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NPC : Character
{
    public void UpdateRotation()
    {
        float _angle = Mathf.Atan2(FaceDir.y, FaceDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new(0, 0, _angle + 90));
    }
}
