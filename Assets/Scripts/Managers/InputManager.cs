using UnityEngine;

public class InputManager : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");


        if (inputX != 0 || inputY != 0)
        {
            PlayerMove.Instance.IsRecievingInput = true;
            PlayerMove.Instance.direction = new Vector2(inputX, inputY);
        }
        else
        {
            PlayerMove.Instance.IsRecievingInput = false;
        }


        PlayerMove.Instance.isTurningLeft = Input.GetKey(KeyCode.Q);
        PlayerMove.Instance.isTurningRight = Input.GetKey(KeyCode.E);

        if (Input.GetKeyDown(KeyCode.Mouse0)) PlayerAction.Instance.HeldItemAction();
        if (Input.GetKeyDown(KeyCode.Alpha1)) PlayerAction.Instance.SetSelectedItemSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) PlayerAction.Instance.SetSelectedItemSlot(1);

        if (Input.GetKeyDown(KeyCode.E)) PlayerAction.Instance.Interact();
        if (Input.GetKeyDown(KeyCode.Escape)) MenuEvents.ToggleLvlMenu();
    }
}
