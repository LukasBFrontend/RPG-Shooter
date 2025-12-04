using UnityEngine;

public class InputManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuEvents.ToggleLvlMenu();
        }

        if (!(GameState.Status == RunState.Running))
        {
            return;
        }

        PlayerMoveInput(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            PlayerAction.Instance.HeldItemAction();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayerAction.Instance.SetSelectedItemSlot(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayerAction.Instance.SetSelectedItemSlot(1);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayerAction.Instance.Interact();
        }
    }

    void PlayerMoveInput(float inputX, float inputY)
    {
        if (inputX != 0 || inputY != 0)
        {
            PlayerMove.Instance.IsRecievingInput = true;
            PlayerMove.Instance.Direction = new Vector2(inputX, inputY);
        }
        else
        {
            PlayerMove.Instance.IsRecievingInput = false;
        }
    }
}
