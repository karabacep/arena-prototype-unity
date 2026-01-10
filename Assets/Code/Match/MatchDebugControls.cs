using UnityEngine;
using UnityEngine.InputSystem;

public class MatchDebugControls : MonoBehaviour
{
    [SerializeField] private MatchManager match;

    private void Awake()
    {

    }

    private void Update()
    {
        if (match == null) return;

        // New Input System ONLY
        if (match.State == MatchManager.MatchState.MatchEnd &&
            Keyboard.current != null &&
            Keyboard.current.rKey.wasPressedThisFrame)
        {
            match.RestartMatch();
        }
    }
}
