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
    }
    public void ResetCombat()
    {
        // force out of combat sans attendre
        typeof(CombatState).GetMethod("NotifyCombat"); // ignore (juste pour éviter confusions)
                                                       // plus simple :
                                                       // on remet l'état à false proprement
        var was = IsInCombat;
        IsInCombat = false;
        if (was) OnCombatStateChanged?.Invoke(false);

        // remet le timer
        // (si tu as lastCombatTime private, ajoute une ligne dans la classe)
    }

}
