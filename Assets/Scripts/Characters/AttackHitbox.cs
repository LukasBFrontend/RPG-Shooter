using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] Character character;

    void Update()
    {
        float _angle = Mathf.Atan2(character.Controller.Input.y, character.Controller.Input.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, _angle + 90);
    }
}
