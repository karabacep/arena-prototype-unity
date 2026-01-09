using UnityEngine;

namespace Arena.Abilities
{
    [CreateAssetMenu(menuName = "Arena/Ability", fileName = "NewAbility")]
    public class AbilityData : ScriptableObject
    {
        [Header("Identity")]
        public string abilityId = "fireball";
        public string displayName = "Fireball";
        public Sprite icon;

        [Header("Cast")]
        public AbilityCastType castType = AbilityCastType.Cast;
        [Min(0f)] public float castTime = 1.5f;
        [Min(0f)] public float cooldown = 8f;
        [Min(0f)] public float globalCooldown = 1.5f;

        [Header("Targeting")]
        public bool requiresTarget = true;
        [Min(0f)] public float range = 30f;
        public bool requiresLineOfSight = true;

        [Header("Effect")]
        public AbilityEffectType effectType = AbilityEffectType.Damage;
        [Min(0f)] public float value = 25f; // dégâts

        [Header("Status")]
        [Min(0f)] public float duration = 0f; // pour stun/silence/shield

    }
}
