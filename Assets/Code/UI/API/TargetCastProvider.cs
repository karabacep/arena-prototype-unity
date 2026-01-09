using UnityEngine;
using Arena.Abilities;

namespace Arena.UI
{
    public class TargetCastProvider : MonoBehaviour
    {
        [SerializeField] private TargetingSystem targeting;

        private AbilityRunner cachedRunner;
        private Transform cachedTarget;

        private void Awake()
        {
            if (targeting == null) targeting = GetComponent<TargetingSystem>();
        }

        public CastInfo GetTargetCastInfo()
        {
            if (targeting == null) return new CastInfo { isCasting = false };

            Transform t = targeting.CurrentTarget;
            if (t == null) return new CastInfo { isCasting = false };

            if (t != cachedTarget)
            {
                cachedTarget = t;
                cachedRunner = t.GetComponent<AbilityRunner>();
            }

            if (cachedRunner == null) return new CastInfo { isCasting = false };

            return cachedRunner.GetCastInfo();
        }
    }
}
