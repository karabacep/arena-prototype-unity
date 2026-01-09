using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arena.Abilities
{
    public class AbilityRunner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TargetingSystem targeting;
        [SerializeField] private Transform aimPoint;
        [SerializeField] private LayerMask losBlockers;
        [SerializeField] private Arena.Combat.StatusController status;


        [Header("State")]
        public bool IsCasting { get; private set; }
        public float CastRemaining { get; private set; }
        public AbilityData CastingAbility { get; private set; }

        public event Action<AbilityData> OnCastStarted;
        public event Action<AbilityData> OnCastCompleted;
        public event Action<AbilityData> OnCastCancelled;

        private readonly Dictionary<string, float> cdReadyTime = new();
        private float gcdReadyTime;
        private Transform forcedCastTarget;


        private void Awake()
        {
            if (status == null) status = GetComponent<Arena.Combat.StatusController>();
            if (status != null)
                status.OnStatusChanged += HandleStatusChanged;
            if (targeting == null) targeting = GetComponent<TargetingSystem>();
            if (aimPoint == null)
            {
                var t = transform.Find("AimPoint");
                if (t != null) aimPoint = t;
            }
        }

        private void Update()
        {
            TickCast();
        }

        public bool TryCast(AbilityData ability)
        {
            if (ability == null) return false;
            if (status != null && (status.Has(Arena.Combat.StatusType.Stun) || status.Has(Arena.Combat.StatusType.Silence)))
                return false;
            if (IsCasting) return false;
            if (Time.time < gcdReadyTime) return false;
            if (Time.time < GetCooldownReadyTime(ability.abilityId)) return false;

            Transform target = targeting != null ? targeting.CurrentTarget : null;
            if (ability.requiresTarget && target == null) return false;
            if (ability.requiresTarget)
            {
                if (!CombatUtils.IsInRange(transform, target, ability.range)) return false;

                if (ability.requiresLineOfSight)
                {
                    Transform targetAim = target.Find("AimPoint");
                    if (targetAim == null) return false;

                    if (!CombatUtils.HasLineOfSight(aimPoint, targetAim, losBlockers)) return false;
                }
            }

            // OK -> lancer
            if (ability.castType == AbilityCastType.Instant || ability.castTime <= 0f)
            {
                Execute(ability, target);
                PutOnCooldowns(ability);
                return true;
            }
            else
            {
                StartCast(ability);
                return true;
            }
        }
        public bool TryCastOnTarget(AbilityData ability, Transform target)
        {
            if (ability == null) return false;
            if (status != null && (status.Has(Arena.Combat.StatusType.Stun) || status.Has(Arena.Combat.StatusType.Silence)))
                return false;
            if (IsCasting) return false;
            if (Time.time < gcdReadyTime) return false;
            if (Time.time < GetCooldownReadyTime(ability.abilityId)) return false;

            if (ability.requiresTarget && target == null) return false;

            if (ability.requiresTarget)
            {
                if (!CombatUtils.IsInRange(transform, target, ability.range)) return false;

                if (ability.requiresLineOfSight)
                {
                    Transform targetAim = target.Find("AimPoint");
                    if (targetAim == null) return false;

                    if (!CombatUtils.HasLineOfSight(aimPoint, targetAim, losBlockers)) return false;
                }
            }

            // Instant
            if (ability.castType == AbilityCastType.Instant || ability.castTime <= 0f)
            {
                Execute(ability, target);
                PutOnCooldowns(ability);
                return true;
            }

            // Cast : mémorise la cible pour la fin
            forcedCastTarget = target;
            StartCast(ability);
            return true;
        }

        private void StartCast(AbilityData ability)
        {
            IsCasting = true;
            CastingAbility = ability;
            CastRemaining = ability.castTime;
            OnCastStarted?.Invoke(ability);
        }

        private void TickCast()
        {
            if (!IsCasting) return;

            CastRemaining -= Time.deltaTime;
            if (CastRemaining > 0f) return;

            // cast terminé
            Transform target = forcedCastTarget != null
                ? forcedCastTarget
                : (targeting != null ? targeting.CurrentTarget : null);


            Execute(CastingAbility, target);
            forcedCastTarget = null;
            PutOnCooldowns(CastingAbility);

            OnCastCompleted?.Invoke(CastingAbility);

            IsCasting = false;
            CastingAbility = null;
            CastRemaining = 0f;
        }

        public void CancelCast()
        {
            if (!IsCasting) return;
            OnCastCancelled?.Invoke(CastingAbility);
            IsCasting = false;
            CastingAbility = null;
            CastRemaining = 0f;
            forcedCastTarget = null;
        }

        private void Execute(AbilityData ability, Transform target)
        {
            if (ability.requiresTarget && target == null) return;

            switch (ability.effectType)
            {
                case AbilityEffectType.Damage:
                    if (target != null)
                    {
                        Health hp = target.GetComponent<Health>();
                        if (hp != null) hp.TakeDamage(ability.value);

                        // combat state (si présent)
                        GetComponent<CombatState>()?.NotifyCombat();
                        target.GetComponent<CombatState>()?.NotifyCombat();
                    }
                    break;
                case AbilityEffectType.Interrupt:
                    if (target != null)
                    {
                        var targetRunner = target.GetComponent<Arena.Abilities.AbilityRunner>();
                        if (targetRunner != null && targetRunner.IsCasting)
                        {
                            targetRunner.CancelCast();
                        }

                        GetComponent<CombatState>()?.NotifyCombat();
                        target.GetComponent<CombatState>()?.NotifyCombat();
                    }
                    break;
                case AbilityEffectType.ApplyStun:
                    if (target != null)
                    {
                        var sc = target.GetComponent<Arena.Combat.StatusController>();
                        if (sc != null) sc.Apply(Arena.Combat.StatusType.Stun, ability.duration);
                        GetComponent<CombatState>()?.NotifyCombat();
                        target.GetComponent<CombatState>()?.NotifyCombat();
                    }
                    break;

                case AbilityEffectType.ApplySilence:
                    if (target != null)
                    {
                        var sc = target.GetComponent<Arena.Combat.StatusController>();
                        if (sc != null) sc.Apply(Arena.Combat.StatusType.Silence, ability.duration);
                        GetComponent<CombatState>()?.NotifyCombat();
                        target.GetComponent<CombatState>()?.NotifyCombat();
                    }
                    break;

                case AbilityEffectType.DefensiveShield:
                    {
                        var mods = GetComponent<DamageModifiers>();
                        if (mods != null) mods.ApplyShield(ability.duration, 0.5f); // 50% damage
                        GetComponent<CombatState>()?.NotifyCombat();
                    }
                    break;
            }
        }

        private void PutOnCooldowns(AbilityData ability)
        {
            cdReadyTime[ability.abilityId] = Time.time + ability.cooldown;
            gcdReadyTime = Time.time + ability.globalCooldown;
        }

        public float GetCooldownReadyTime(string abilityId)
        {
            return cdReadyTime.TryGetValue(abilityId, out float t) ? t : 0f;
        }

        public float GetCooldownRemaining(string abilityId)
        {
            float ready = GetCooldownReadyTime(abilityId);
            return Mathf.Max(0f, ready - Time.time);
        }

        public float GetGcdRemaining()
        {
            return Mathf.Max(0f, gcdReadyTime - Time.time);
        }
        private void HandleStatusChanged(Arena.Combat.StatusType type, bool active)
        {
            if (!active) return;

            if (type == Arena.Combat.StatusType.Stun && IsCasting)
                CancelCast();

            if (type == Arena.Combat.StatusType.Silence && IsCasting)
                CancelCast(); // option: en WoW silence coupe aussi, on le fait simple
        }

    }
}
