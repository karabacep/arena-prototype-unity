using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TargetingSystem targeting;
    [SerializeField] private Transform aimPoint;

    [Header("Attack")]
    [SerializeField] private float range = 3f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackInterval = 1.8f;

    [Header("Line of Sight")]
    [SerializeField] private LayerMask losBlockers;

    private float timer;
    private CombatState myCombat;


    private void Awake()
    {
        myCombat = GetComponent<CombatState>();
        if (targeting == null) targeting = GetComponent<TargetingSystem>();
        if (aimPoint == null)
        {
            Transform t = transform.Find("AimPoint");
            if (t != null) aimPoint = t;
        }
    }

    private void Update()
    {
        var mm = FindObjectOfType<MatchManager>();
        if (mm != null && !mm.CanAct) return;
        if (targeting == null || aimPoint == null) return;
        var status = GetComponent<Arena.Combat.StatusController>();
        if (status != null && status.Has(Arena.Combat.StatusType.Stun)) return;
        Transform target = targeting.CurrentTarget;
        if (target == null) return;

        // Récupère l’aimPoint de la cible
        Transform targetAim = target.Find("AimPoint");
        if (targetAim == null) return;

        bool inRange = CombatUtils.IsInRange(transform, target, range);
        bool hasLos = CombatUtils.HasLineOfSight(aimPoint, targetAim, losBlockers);

        if (!inRange || !hasLos) return;

        timer += Time.deltaTime;
        if (timer >= attackInterval)
        {
            timer = 0f;

            Health hp = target.GetComponent<Health>();
            if (hp != null)
            {
                hp.TakeDamage(damage, transform);
                myCombat?.NotifyCombat();
                CombatState targetCombat = target.GetComponent<CombatState>();
                targetCombat?.NotifyCombat();
            }

            }
        }
}
