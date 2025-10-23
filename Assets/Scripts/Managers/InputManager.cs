using UnityEngine;

public class InputManager : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

        PlayerMove.Instance.direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        PlayerMove.Instance.isTurningLeft = Input.GetKey(KeyCode.Q);
        PlayerMove.Instance.isTurningRight = Input.GetKey(KeyCode.E);

        if (Input.GetKeyDown(KeyCode.Mouse0)) PlayerAction.Instance.FireWeapon();
    }
}
