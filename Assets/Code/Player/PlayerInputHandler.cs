using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public float ZoomInput { get; private set; }
    public bool IsRotateHeld { get; private set; }
    public bool TargetPressed { get; private set; }
    public bool CycleTargetPressed { get; private set; }
    public bool Cast1Pressed { get; private set; }
    public bool Cast2Pressed { get; private set; }
    public bool Cast3Pressed { get; private set; }
    public bool Cast4Pressed { get; private set; }
    public bool Cast5Pressed { get; private set; }
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
    public void OnTarget(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed) TargetPressed = true;
    }

    public void OnCycleTarget(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed) CycleTargetPressed = true;
    }
    public void ConsumeOneFrameButtons()
    {
        TargetPressed = false;
        CycleTargetPressed = false;
    }

    public void ConsumeCastButtons()
    {
        Cast1Pressed = false;
        Cast2Pressed = false;
        Cast3Pressed = false;
        Cast4Pressed = false;
        Cast5Pressed = false;
    }
    public void OnCast1(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed) Cast1Pressed = true;
    }
    public void OnCast2(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed) Cast2Pressed = true;
    }
    public void OnCast3(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed) Cast3Pressed = true;
    }
    public void OnCast4(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed) Cast4Pressed = true;
    }
    public void OnCast5(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed) Cast5Pressed = true;
    }
}

