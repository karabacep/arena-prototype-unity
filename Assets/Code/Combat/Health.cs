using System;
using UnityEngine;
using Arena.UI;


public class Health : MonoBehaviour
{
    [SerializeField] private float maxHp = 100f;
    private CombatState combatState;

    public float CurrentHp { get; private set; }
    public float MaxHp => maxHp;

    public bool IsDead => CurrentHp <= 0f;

    public event Action<Health> OnDied;
    public event System.Action<Health, Transform, float, float> OnDamaged;
    // (victimHealth, attacker, rawDamage, finalDamage)

    private void Awake()
    {
        combatState = GetComponent<CombatState>();
        CurrentHp = maxHp;
    }

    public void ResetHealth()
    {
        CurrentHp = maxHp;
    }

    public void TakeDamage(float rawDamage, Transform attacker = null)
    {
        if (IsDead) return;

        float finalDamage = rawDamage;

        var mods = GetComponent<DamageModifiers>();
        if (mods != null) finalDamage = mods.ModifyIncomingDamage(finalDamage);

        CurrentHp = Mathf.Max(0f, CurrentHp - finalDamage);

        OnDamaged?.Invoke(this, attacker, rawDamage, finalDamage);

        combatState?.NotifyCombat();

        if (IsDead)
            OnDied?.Invoke(this);
    }
    public void SetCurrentHp(float value)
    {
        CurrentHp = Mathf.Clamp(value, 0f, maxHp);
    }
    public HealthInfo GetHealthInfo()
    {
        return new HealthInfo
        {
            current = CurrentHp,
            max = maxHp,
            normalized = maxHp <= 0f ? 0f : CurrentHp / maxHp
        };
    }

}
