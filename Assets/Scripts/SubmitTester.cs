using UnityEngine;
using UnityEngine.InputSystem;

public class SubmitTester : MonoBehaviour
{
    public InputActionReference submitAction;

    void OnEnable()
    {
        submitAction.action.performed += OnSubmit;
    }

    void OnDisable()
    {
        submitAction.action.performed -= OnSubmit;
    }

    void OnSubmit(InputAction.CallbackContext ctx)
    {
        Debug.Log("SUBMIT fired");
    }
}
