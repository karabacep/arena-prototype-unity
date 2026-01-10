using UnityEngine;

public class MatchCountdownDebugHUD : MonoBehaviour
{
    [SerializeField] private MatchManager match;

    private GUIStyle style;

    // "GO" affiché jusqu'à cette heure (unscaled pour être robuste)
    private float goDisplayUntil = -1f;

    // Pour détecter le passage Countdown -> InRound
    private MatchManager.MatchState lastState = MatchManager.MatchState.Waiting;

    private void Awake()
    {
        style = new GUIStyle
        {
            fontSize = 72,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.yellow }
        };

        if (match != null)
            lastState = match.State;
    }

    private void OnGUI()
    {
        if (match == null) return;

        float centerY = Screen.height * 0.4f; // un peu au-dessus du centre

        // Détecte le moment exact où on passe en InRound (début du round)
        if (match.State == MatchManager.MatchState.InRound &&
            lastState != MatchManager.MatchState.InRound)
        {
            goDisplayUntil = Time.unscaledTime + 1.0f; // "GO" pendant 1 seconde
        }

        // Affiche le countdown
        if (match.State == MatchManager.MatchState.Countdown && match.CountdownValue > 0)
        {
            GUI.Label(
                new Rect(0, centerY - 40, Screen.width, 80),
                match.CountdownValue.ToString(),
                style
            );
        }

        // Affiche "GO" seulement pendant 1 seconde
        if (goDisplayUntil > 0f && Time.unscaledTime <= goDisplayUntil)
        {
            GUI.Label(
                new Rect(0, centerY - 40, Screen.width, 80),
                "GO",
                style
            );
        }
        else if (goDisplayUntil > 0f && Time.unscaledTime > goDisplayUntil)
        {
            goDisplayUntil = -1f; // stop l'affichage
        }

        lastState = match.State;
    }
}
