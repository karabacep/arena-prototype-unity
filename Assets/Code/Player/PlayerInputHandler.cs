using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public float ZoomInput { get; private set; }
    public bool IsRotateHeld { get; private set; }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }
    public void OnZoom(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        ZoomInput = context.ReadValue<float>();
    }

    public void OnRotateCamera(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        IsRotateHeld = context.ReadValueAsButton();
    }
}

