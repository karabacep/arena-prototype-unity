using UnityEngine;

namespace Arena.UI
{
    public class ApiCastbarDebugHUD : MonoBehaviour
    {
        [SerializeField] private Arena.Abilities.AbilityRunner playerRunner;
        [SerializeField] private TargetCastProvider targetProvider;

        private GUIStyle style;

        private void Awake()
        {
            style = new GUIStyle { fontSize = 16, normal = { textColor = Color.white } };
        }

        private void OnGUI()
        {
            int y = 140;

            if (playerRunner != null)
            {
                var c = playerRunner.GetCastInfo();
                GUI.Label(new Rect(10, y, 1200, 25),
                    c.isCasting ? $"PLAYER CAST: {c.displayName} {c.remaining:0.00}s ({c.normalized:P0})"
                                : "PLAYER CAST: none",
                    style);
                y += 22;
            }

            if (targetProvider != null)
            {
                var tc = targetProvider.GetTargetCastInfo();
                GUI.Label(new Rect(10, y, 1200, 25),
                    tc.isCasting ? $"TARGET CAST: {tc.displayName} {tc.remaining:0.00}s ({tc.normalized:P0})"
                                 : "TARGET CAST: none",
                    style);
            }
        }
    }
}
