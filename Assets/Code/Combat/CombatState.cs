using System;
using UnityEngine;

public class CombatState : MonoBehaviour
{
    [SerializeField] private float outOfCombatDelay = 5f;

    public bool IsInCombat { get; private set; }
    public event Action<bool> OnCombatStateChanged;

    private float lastCombatTime;

    private void Update()
    {
        if (IsInCombat && Time.time - lastCombatTime >= outOfCombatDelay)
        {
            SetCombat(false);
        }
    }

    public void NotifyCombat()
    {
        lastCombatTime = Time.time;
        if (!IsInCombat)
            SetCombat(true);
    }

    private void SetCombat(bool value)
    {
        if (IsInCombat == value) return;
        IsInCombat = value;
        OnCombatStateChanged?.Invoke(IsInCombat);
        Debug.Log($"{name} InCombat = {IsInCombat}");
    }
}
