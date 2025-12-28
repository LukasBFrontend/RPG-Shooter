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
            Actions.Instance.HeldItemAction();
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            Actions.Instance.HeldItemFocus();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Actions.Instance.SetSelectedItemSlot(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Actions.Instance.SetSelectedItemSlot(1);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Actions.Instance.Interact();
        }
    }

    void PlayerMoveInput(float inputX, float inputY)
    {
        Player.Movement.Input = new Vector2(inputX, inputY);
    }
}
