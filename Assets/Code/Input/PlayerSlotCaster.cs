using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[RequireComponent(typeof(Arena.Abilities.AbilityRunner))]
public class PlayerSlotCaster : MonoBehaviour
{
    [SerializeField] private SlotBindings bindings;

    private Arena.Abilities.AbilityRunner runner;

    private void Awake()
    {
        runner = GetComponent<Arena.Abilities.AbilityRunner>();
    }

    private void Start()
    {
        if (bindings != null && runner != null && runner.Loadout != null)
        {
            bindings.EnsureSize(runner.Loadout.SlotCount, KeyCode.None);
        }
    }

    private void Update()
    {
        if (bindings == null || runner == null || runner.Loadout == null) return;
        if (Keyboard.current == null) return;

        int count = runner.Loadout.SlotCount;

        for (int i = 0; i < count; i++)
        {
            var keyCode = bindings.GetKey(i);
            if (keyCode == KeyCode.None) continue;

            var key = ToKey(keyCode);
            if (key != null && key.wasPressedThisFrame)
            {
                runner.TryCastSlot(i);
            }
        }
    }

    private static KeyControl ToKey(KeyCode kc)
    {
        // Mapping minimal des touches courantes (Alpha1..Alpha0, QWERASDF, Space, etc.)
        // On étendra au besoin.
        switch (kc)
        {
            case KeyCode.Alpha1: return Keyboard.current.digit1Key;
            case KeyCode.Alpha2: return Keyboard.current.digit2Key;
            case KeyCode.Alpha3: return Keyboard.current.digit3Key;
            case KeyCode.Alpha4: return Keyboard.current.digit4Key;
            case KeyCode.Alpha5: return Keyboard.current.digit5Key;
            case KeyCode.Alpha6: return Keyboard.current.digit6Key;
            case KeyCode.Alpha7: return Keyboard.current.digit7Key;
            case KeyCode.Alpha8: return Keyboard.current.digit8Key;
            case KeyCode.Alpha9: return Keyboard.current.digit9Key;
            case KeyCode.Alpha0: return Keyboard.current.digit0Key;

            case KeyCode.Q: return Keyboard.current.qKey;
            case KeyCode.W: return Keyboard.current.wKey;
            case KeyCode.E: return Keyboard.current.eKey;
            case KeyCode.R: return Keyboard.current.rKey;
            case KeyCode.A: return Keyboard.current.aKey;
            case KeyCode.S: return Keyboard.current.sKey;
            case KeyCode.D: return Keyboard.current.dKey;
            case KeyCode.F: return Keyboard.current.fKey;

            case KeyCode.Space: return Keyboard.current.spaceKey;

            default: return null;
        }
    }
}
