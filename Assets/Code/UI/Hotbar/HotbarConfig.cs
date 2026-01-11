using System.Collections.Generic;
using UnityEngine;
using Arena.Abilities;

[CreateAssetMenu(menuName = "Arena/UI/HotbarConfig", fileName = "HotbarConfig")]
public class HotbarConfig : ScriptableObject
{
    [Tooltip("Nombre de barres visibles (ex: 2)")]
    public int barCount = 2;

    [Tooltip("Longueur de chaque barre. Taille doit être >= barCount. Somme = nombre total de slots.")]
    public List<int> barLengths = new List<int> { 6, 6 };

    [Tooltip("Slots linéaires : length must equal sum(barLengths). Peut contenir null pour slot vide.")]
    public List<AbilityData> slots = new List<AbilityData>();

    public int TotalSlots
    {
        get
        {
            int sum = 0;
            if (barLengths != null)
            {
                foreach (var l in barLengths) sum += Mathf.Max(0, l);
            }
            return sum;
        }
    }

    private void OnValidate()
    {
        if (barCount <= 0) barCount = 1;
        if (barLengths == null) barLengths = new List<int> { 6 };
        while (barLengths.Count < barCount) barLengths.Add(6);
        if (barLengths.Count > barCount) barLengths.RemoveRange(barCount, barLengths.Count - barCount);

        int total = TotalSlots;
        if (slots == null) slots = new List<AbilityData>();
        while (slots.Count < total) slots.Add(null);
        if (slots.Count > total) slots.RemoveRange(total, slots.Count - total);
    }

    public void SetSlot(int slotIndex, AbilityData ability)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return;
        slots[slotIndex] = ability;
    }

    public AbilityData GetSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return null;
        return slots[slotIndex];
    }
}