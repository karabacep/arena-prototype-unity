using System;
using System.Collections.Generic;
using UnityEngine;
using Arena.UI;

namespace Arena.Combat
{
    public class StatusController : MonoBehaviour
    {
        private readonly Dictionary<StatusType, float> endTime = new();

        public event System.Action<StatusType, bool, float> OnStatusChanged;
        // (type, active, remainingSeconds)

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
                OnStatusChanged?.Invoke(type, true, duration);
        }

        private void Update()
        {
            // détecte les fins de status et déclenche event
            foreach (StatusType type in (StatusType[])Enum.GetValues(typeof(StatusType)))
            {
                if (endTime.TryGetValue(type, out float t) && t > 0f && Time.time >= t)
                {
                    endTime[type] = 0f;
                    OnStatusChanged?.Invoke(type, false, 0f);
                }
            }
        }
        public List<StatusInfo> GetActiveStatuses()
        {
            var list = new List<StatusInfo>();

            foreach (var kvp in endTime)
            {
                float remaining = kvp.Value - Time.time;
                if (remaining > 0f)
                {
                    list.Add(new StatusInfo
                    {
                        statusId = kvp.Key.ToString(),
                        remaining = remaining,
                        active = true
                    });
                }
            }

            return list;
        }
    }
}
