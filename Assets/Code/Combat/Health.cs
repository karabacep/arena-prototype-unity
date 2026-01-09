using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHp = 100f;
    private CombatState combatState;

    public float CurrentHp { get; private set; }
    public float MaxHp => maxHp;

    public bool IsDead => CurrentHp <= 0f;

    public event Action<Health> OnDied;
    public event Action<Health, float> OnDamaged;

    private void Awake()
    {
        combatState = GetComponent<CombatState>();
        CurrentHp = maxHp;
    }

    public void ResetHealth()
    {
        CurrentHp = maxHp;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        CurrentHp = Mathf.Max(0f, CurrentHp - amount);
        combatState?.NotifyCombat();
        OnDamaged?.Invoke(this, amount);

        if (IsDead)
            OnDied?.Invoke(this);
    }
    public void SetCurrentHp(float value)
    {
        CurrentHp = Mathf.Clamp(value, 0f, maxHp);
    }

}
