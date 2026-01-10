using UnityEngine;

public class MatchScoreDebugHUD : MonoBehaviour
{
    [SerializeField] private MatchManager match;

    private GUIStyle style;

    private void Awake()
    {
        style = new GUIStyle
        {
            fontSize = 18,
            normal = { textColor = Color.white }
        };
    }

    private void OnGUI()
    {
        if (match == null) return;

        GUI.Label(
            new Rect(10, 55, 700, 25),
            $"ROUND {match.RoundIndex} | SCORE P:{match.PlayerRoundsWon} - E:{match.EnemyRoundsWon} | STATE: {match.State}",
            style
        );

        if (match.State == MatchManager.MatchState.MatchEnd)
        {
            string winner = match.PlayerRoundsWon > match.EnemyRoundsWon ? "PLAYER WINS" : "ENEMY WINS";
            GUI.Label(
                new Rect(Screen.width / 2 - 150, Screen.height * 0.3f, 300, 40),
                winner,
                style
            );
        }
    }
}
