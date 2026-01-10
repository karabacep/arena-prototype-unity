using UnityEngine;

public class MatchDebugControls : MonoBehaviour
{
    [SerializeField] private MatchManager match;

    private void Awake()
    {
        if (match == null) match = FindObjectOfType<MatchManager>();
    }

    private void Update()
    {
        if (match == null) return;

        if (match.State == MatchManager.MatchState.MatchEnd && Input.GetKeyDown(KeyCode.R))
        {
            match.RestartMatch();
        }
    }
}
