using Arena.Abilities;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerAbilityInput : MonoBehaviour
{
    [SerializeField] private AbilityRunner runner;

    private ArenaControls controls;
    private Action<InputAction.CallbackContext>[] handlers;

    private const int SlotCount = 12;

    private void Awake()
    {
        if (runner == null)
            runner = GetComponent<AbilityRunner>();

        controls = new ArenaControls();
        handlers = new Action<InputAction.CallbackContext>[SlotCount];
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();

        for (int i = 0; i < SlotCount; i++)
        {
            int slot = i; // capture safe

            // CastSlot1..CastSlot12
            string actionName = $"CastSlot{slot + 1}";
            var action = controls.Gameplay.Get().FindAction(actionName, throwIfNotFound: true);

            Action<InputAction.CallbackContext> handler = _ => runner.TryCastSlot(slot);
            handlers[slot] = handler;

            action.performed += handler;
        }
    }

    private void OnDisable()
    {
        if (controls == null) return;

        for (int i = 0; i < SlotCount; i++)
        {
            int slot = i;
            string actionName = $"CastSlot{slot + 1}";

            var action = controls.Gameplay.Get().FindAction(actionName, throwIfNotFound: false);
            var handler = handlers[slot];

            if (action != null && handler != null)
                action.performed -= handler;

            handlers[slot] = null;
        }

        controls.Gameplay.Disable();
    }
}
