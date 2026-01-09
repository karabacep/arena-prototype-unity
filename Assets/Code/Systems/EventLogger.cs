using UnityEngine;
using Arena.Abilities;
using Arena.Combat;

public class EventLogger : MonoBehaviour
{
    [SerializeField] private AbilityRunner runner;
    [SerializeField] private Health health;
    [SerializeField] private StatusController status;

    private void Awake()
    {
        if (runner == null) runner = GetComponent<AbilityRunner>();
        if (health == null) health = GetComponent<Health>();
        if (status == null) status = GetComponent<StatusController>();
    }

    private void OnEnable()
    {
        if (runner != null)
        {
            runner.OnCastStarted += a => Debug.Log($"{name} CAST START: {a.displayName}");
            runner.OnCastCompleted += a => Debug.Log($"{name} CAST COMPLETE: {a.displayName}");
            runner.OnCastCancelled += a => Debug.Log($"{name} CAST CANCEL: {a.displayName}");
            runner.OnAbilityExecuted += (a, t) => Debug.Log($"{name} EXECUTE: {a.displayName} -> {(t ? t.name : "none")}");
        }

        if (health != null)
        {
            health.OnDamaged += (victim, attacker, raw, finalDmg) =>
                Debug.Log($"{victim.name} DAMAGED by {(attacker ? attacker.name : "unknown")}: {raw} -> {finalDmg}");

            health.OnDied += h => Debug.Log($"{h.name} DIED");
        }

        if (status != null)
        {
            status.OnStatusChanged += (type, active, remaining) =>
                Debug.Log($"{name} STATUS {(active ? "ON" : "OFF")} {type} ({remaining:0.00}s)");
        }
    }

    private void OnDisable()
    {
        // pas vital en proto, mais propre : non implémenté pour aller vite
    }
}
