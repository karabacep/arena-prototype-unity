using Arena.Abilities;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Arena/Abilities/Ability Loadout", fileName = "AbilityLoadout")]
public class AbilityLoadout : ScriptableObject
{
    [Tooltip("Abilities dans l'ordre des slots (1..n)")]
    public List<AbilityData> slots = new List<AbilityData>();
    public int SlotCount => slots != null ? slots.Count : 0;

    public AbilityData Get(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return null;
        return slots[slotIndex];
    }
    public void Swap(int a, int b)
    {
        if (slots == null) return;
        if (a < 0 || b < 0 || a >= slots.Count || b >= slots.Count) return;

        (slots[a], slots[b]) = (slots[b], slots[a]);
    }

    public void Set(int index, AbilityData ability)
    {
        if (slots == null) return;
        if (index < 0 || index >= slots.Count) return;
        slots[index] = ability;
    }

}
