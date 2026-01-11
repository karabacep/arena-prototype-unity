using System;
using System.Collections.Generic;
using UnityEngine;
using Arena.UI;

namespace Arena.Abilities
{
    public class AbilityRunner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TargetingSystem targeting;
        [SerializeField] private Transform aimPoint;
        [SerializeField] private LayerMask losBlockers;
        [SerializeField] private Arena.Combat.StatusController status;
        [SerializeField] private MatchManager match;


        [Header("State")]
        public bool IsCasting { get; private set; }
        public float CastRemaining { get; private set; }
        public AbilityData CastingAbility { get; private set; }
        [Header("Loadout")]
        [SerializeField] private AbilityLoadout loadout;
        public AbilityLoadout Loadout => loadout;

        public void SetLoadout(AbilityLoadout l) => loadout = l;

        public event Action<AbilityData> OnCastStarted;
        public event Action<AbilityData> OnCastCompleted;
        public event Action<AbilityData> OnCastCancelled;
        public event System.Action<AbilityData, Transform> OnAbilityExecuted;


        private readonly Dictionary<string, float> cdReadyTime = new();
        private float gcdReadyTime;
        private float castStartedAt;
        private float castTotalTime;
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
            if (match == null) match = FindFirstObjectByType<MatchManager>();
        }

        private void Update()
        {
            TickCast();
        }
        public bool TryCastSlot(int slotIndex)
        {
            if (loadout == null) return false;
            var a = loadout.Get(slotIndex);
            if (a == null) return false;
            return TryCast(a);
        }

        public bool TryCastSlotOnTarget(int slotIndex, Transform target)
        {
            if (loadout == null) return false;
            var a = loadout.Get(slotIndex);
            if (a == null) return false;
            return TryCastOnTarget(a, target);
        }

        public bool TryCast(AbilityData ability)
        {
            if (match != null && !match.CanAct) return false;
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
            if (match != null && !match.CanAct) return false;
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
            castStartedAt = Time.time;
            castTotalTime = Mathf.Max(0.0001f, ability.castTime);
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
                        if (hp != null) hp.TakeDamage(ability.value, transform);

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
            OnAbilityExecuted?.Invoke(ability, target);

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
        private void HandleStatusChanged(Arena.Combat.StatusType type, bool active, float remainingSeconds)
        {
            if (!active) return;

            if ((type == Arena.Combat.StatusType.Stun || type == Arena.Combat.StatusType.Silence) && IsCasting)
                CancelCast();
        }
        public Arena.UI.CastInfo GetCastInfo()
        {
            if (!IsCasting || CastingAbility == null)
                return new Arena.UI.CastInfo { isCasting = false };

            float remaining = Mathf.Max(0f, CastRemaining);
            float dur = Mathf.Max(0.0001f, castTotalTime);

            float normalized = Mathf.Clamp01(1f - (remaining / dur));

            return new Arena.UI.CastInfo
            {
                isCasting = true,
                abilityId = CastingAbility.abilityId,
                displayName = CastingAbility.displayName,
                castDuration = dur,
                remaining = remaining,
                normalized = normalized,
                startedAt = castStartedAt,
                endsAt = castStartedAt + dur
            };
        }
        public CooldownInfo GetCooldownInfo(string abilityId, float cooldownDuration)
        {
            float remaining = GetCooldownRemaining(abilityId);

            return new CooldownInfo
            {
                abilityId = abilityId,
                remaining = remaining,
                duration = cooldownDuration,
                onCooldown = remaining > 0f
            };
        }

        public float GetGcdNormalized()
        {
            float gcd = GetGcdRemaining();
            return gcd <= 0f ? 0f : Mathf.Clamp01(gcd / 1.5f);
        }
        public void ResetAll()
        {
            CancelCast();              // stop cast si besoin
            cdReadyTime.Clear();       // reset cooldowns
            gcdReadyTime = 0f;         // reset gcd
            forcedCastTarget = null;   // si tu as ajouté ça (option B)
        }
    }
}
