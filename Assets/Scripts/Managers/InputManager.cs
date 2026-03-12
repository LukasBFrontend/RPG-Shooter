using UnityEngine;

public class InputManager : MonoBehaviour
{
    void Update()
    {
        if (GameState.InputDisabled)
        {
            return;
        }

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
            GameState.Player.Actions.HeldItemAction();
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            GameState.Player.Actions.HeldItemFocus();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameState.Player.Actions.SetSelectedItemSlot(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameState.Player.Actions.SetSelectedItemSlot(1);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            GameState.Player.Actions.Interact();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            GameState.Player.Actions.HolsterWeapon();
        }
    }

    void PlayerMoveInput(float inputX, float inputY)
    {
        GameState.Player.Controller.Move(new Vector2(inputX, inputY));
    }
}
