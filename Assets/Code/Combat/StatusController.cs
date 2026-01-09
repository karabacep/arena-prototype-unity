using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arena.Combat
{
    public class StatusController : MonoBehaviour
    {
        private readonly Dictionary<StatusType, float> endTime = new();

        public event Action<StatusType, bool> OnStatusChanged;

        public bool Has(StatusType type)
        {
            return endTime.TryGetValue(type, out float t) && Time.time < t;
        }

        public float Remaining(StatusType type)
        {
            return endTime.TryGetValue(type, out float t) ? Mathf.Max(0f, t - Time.time) : 0f;
        }

        public void Apply(StatusType type, float duration)
        {
            float until = Time.time + Mathf.Max(0f, duration);
            bool wasActive = Has(type);

            endTime[type] = until;

            if (!wasActive)
                OnStatusChanged?.Invoke(type, true);
        }

        private void Update()
        {
            // détecte les fins de status et déclenche event
            foreach (StatusType type in (StatusType[])Enum.GetValues(typeof(StatusType)))
            {
                if (endTime.TryGetValue(type, out float t) && t > 0f && Time.time >= t)
                {
                    endTime[type] = 0f;
                    OnStatusChanged?.Invoke(type, false);
                }
            }
        }
    }
}
