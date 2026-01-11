using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Arena/Input/Slot Bindings", fileName = "SlotBindings")]
public class SlotBindings : ScriptableObject
{
    // Exemple: "Alpha1", "Alpha2", etc.
    public List<KeyCode> keys = new List<KeyCode> { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5 };

    public KeyCode GetKey(int slot)
    {
        if (slot < 0 || slot >= keys.Count) return KeyCode.None;
        return keys[slot];
    }

    public void SetKey(int slot, KeyCode key)
    {
        if (slot < 0 || slot >= keys.Count) return;
        keys[slot] = key;
    }
    public void EnsureSize(int size, KeyCode defaultKey = KeyCode.None)
    {
        if (keys == null) keys = new List<KeyCode>();
        while (keys.Count < size) keys.Add(defaultKey);
    }

}
