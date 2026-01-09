using UnityEngine;
using Arena.Abilities;

public class TargetCastDebugHUD : MonoBehaviour
{
    [SerializeField] private TargetingSystem targeting;

    private GUIStyle style;

    private void Awake()
    {
        style = new GUIStyle
        {
            fontSize = 16,
            normal = { textColor = Color.white }
        };
    }

    private void OnGUI()
    {
        if (targeting == null)
        {
            GUI.Label(new Rect(10, 85, 1000, 25), "TargetCastDebugHUD: targeting manquant", style);
            return;
        }

        Transform t = targeting.CurrentTarget;
        if (t == null)
        {
            GUI.Label(new Rect(10, 85, 1000, 25), "TARGET CAST: none (no target)", style);
            return;
        }

        AbilityRunner r = t.GetComponent<AbilityRunner>();
        if (r == null)
        {
            GUI.Label(new Rect(10, 85, 1000, 25), $"TARGET CAST: {t.name} (no AbilityRunner)", style);
            return;
        }

        if (!r.IsCasting)
        {
            GUI.Label(new Rect(10, 85, 1000, 25), $"TARGET CAST: {t.name} (not casting)", style);
            return;
        }

        GUI.Label(new Rect(10, 85, 1000, 25),
            $"TARGET CAST: {t.name} casting {r.CastingAbility.displayName} | {r.CastRemaining:0.00}s",
            style);
    }
}
