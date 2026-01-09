using UnityEngine;

public class DebugHUD : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private CombatState playerCombat;
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
        if (playerHealth == null || playerCombat == null || targeting == null)
        {
            GUI.Label(new Rect(10, 10, 600, 30), "DebugHUD: références manquantes", style);
            return;
        }

        int y = 10;

        GUI.Label(new Rect(10, y, 600, 30),
            $"PLAYER HP: {playerHealth.CurrentHp:0}/{playerHealth.MaxHp:0}  |  InCombat: {playerCombat.IsInCombat}", style);
        y += 22;

        Transform t = targeting.CurrentTarget;
        if (t == null)
        {
            GUI.Label(new Rect(10, y, 600, 30), "TARGET: none", style);
            return;
        }

        Health th = t.GetComponent<Health>();
        CombatState tc = t.GetComponent<CombatState>();

        string hpStr = th != null ? $"{th.CurrentHp:0}/{th.MaxHp:0}" : "no Health";
        string cStr = tc != null ? tc.IsInCombat.ToString() : "no CombatState";

        GUI.Label(new Rect(10, y, 900, 30),
            $"TARGET: {t.name}  |  HP: {hpStr}  |  InCombat: {cStr}", style);
    }
}
