using UnityEngine;
using Arena.Combat;

public class StatusDebugHUD : MonoBehaviour
{
    [SerializeField] private StatusController playerStatus;
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
        int y = 110;

        if (playerStatus == null)
        {
            GUI.Label(new Rect(10, y, 1000, 25), "StatusDebugHUD: playerStatus manquant", style);
            return;
        }

        GUI.Label(new Rect(10, y, 1000, 25),
            $"PLAYER STATUS: STUN {playerStatus.Remaining(StatusType.Stun):0.00}s | SILENCE {playerStatus.Remaining(StatusType.Silence):0.00}s",
            style);
        y += 22;

        if (targeting == null || targeting.CurrentTarget == null)
        {
            GUI.Label(new Rect(10, y, 1000, 25), "TARGET STATUS: none", style);
            return;
        }

        var ts = targeting.CurrentTarget.GetComponent<StatusController>();
        if (ts == null)
        {
            GUI.Label(new Rect(10, y, 1000, 25), $"TARGET STATUS: {targeting.CurrentTarget.name} (no StatusController)", style);
            return;
        }

        GUI.Label(new Rect(10, y, 1000, 25),
            $"TARGET STATUS: {targeting.CurrentTarget.name} | STUN {ts.Remaining(StatusType.Stun):0.00}s | SILENCE {ts.Remaining(StatusType.Silence):0.00}s",
            style);
    }
}
