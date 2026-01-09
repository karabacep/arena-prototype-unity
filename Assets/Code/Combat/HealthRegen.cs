using UnityEngine;

public class HealthRegen : MonoBehaviour
{
    [SerializeField] private float regenPerSecond = 5f;

    private Health health;
    private CombatState combat;

    private void Awake()
    {
        health = GetComponent<Health>();
        combat = GetComponent<CombatState>();
    }

    private void Update()
    {
        if (health == null || combat == null) return;
        if (combat.IsInCombat) return;
        if (health.CurrentHp >= health.MaxHp) return;

        health.SetCurrentHp(
            Mathf.Min(
                health.MaxHp,
                health.CurrentHp + regenPerSecond * Time.deltaTime
            )
        );
    }
}
