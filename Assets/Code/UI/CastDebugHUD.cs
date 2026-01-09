using UnityEngine;
using Arena.Abilities;

public class CastDebugHUD : MonoBehaviour
{
    [SerializeField] private AbilityRunner runner;

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
        if (runner == null)
        {
            GUI.Label(new Rect(10, 60, 800, 25), "CastDebugHUD: runner manquant", style);
            return;
        }

        if (!runner.IsCasting)
        {
            float gcd = runner.GetGcdRemaining();
            if (gcd > 0f)
                GUI.Label(new Rect(10, 60, 800, 25), $"GCD: {gcd:0.00}s", style);
            return;
        }

        GUI.Label(new Rect(10, 60, 800, 25),
            $"CASTING: {runner.CastingAbility.displayName}  |  Remaining: {runner.CastRemaining:0.00}s",
            style);
    }
}
