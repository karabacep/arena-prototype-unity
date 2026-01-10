using System.Collections;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public enum MatchState { Waiting, Countdown, InRound, RoundEnd, MatchEnd }

    [Header("Participants")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform enemy;
    [Header("Spawns")]
    [SerializeField] private Transform playerSpawn;
    [SerializeField] private Transform enemySpawn;
    [Header("Round")]
    [SerializeField] private float roundEndDelay = 2f;
    [SerializeField] private int countdownSeconds = 3;
    [Header("Player Camera")]
    [SerializeField] private Transform playerCameraYaw; // ex: CameraYaw
    [Header("Player Visual")]
    [SerializeField] private PlayerFacing playerFacing;
    [Header("Match Rules")]
    [SerializeField] private int roundsToWin = 2;

    public int CountdownValue { get; private set; }

    public MatchState State { get; private set; } = MatchState.Waiting;
    public bool CanAct => State == MatchState.InRound;

    public int PlayerRoundsWon { get; private set; }
    public int EnemyRoundsWon { get; private set; }
    public int RoundIndex { get; private set; } = 1;

    private Health playerHealth;
    private Health enemyHealth;
    private void SnapPlayerVisualToCamera()
    {
        if (playerFacing == null || playerCameraYaw == null) return;

        Vector3 forward = playerCameraYaw.forward;
        playerFacing.SnapToDirection(forward);
    }
    private void SnapPlayerCameraToSpawn()
    {
        if (playerCameraYaw == null || playerSpawn == null) return;

        // On ne garde que le Yaw (rotation Y), pas le pitch/roll
        Vector3 e = playerSpawn.rotation.eulerAngles;
        playerCameraYaw.rotation = Quaternion.Euler(0f, e.y, 0f);
    }
    private void TeleportToSpawn(Transform fighter, Transform spawn)
    {
        if (fighter == null || spawn == null) return;

        var cc = fighter.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        fighter.SetPositionAndRotation(spawn.position, spawn.rotation);

        if (cc != null) cc.enabled = true;
    }

    private IEnumerator CountdownRoutine()
    {
        State = MatchState.Countdown;

        // Reset complet avant le GO
        ResetFighter(player);
        ResetFighter(enemy);
        TeleportToSpawn(player, playerSpawn);
        TeleportToSpawn(enemy, enemySpawn);
        SnapPlayerCameraToSpawn();
        SnapPlayerVisualToCamera();

        CountdownValue = countdownSeconds;

        while (CountdownValue > 0)
        {
            Debug.Log($"ROUND {RoundIndex} START IN {CountdownValue}");
            yield return new WaitForSeconds(1f);
            CountdownValue--;
        }

        Debug.Log("GO!");
        CountdownValue = 0;

        State = MatchState.InRound;
    }

    private void Awake()
    {
        playerHealth = player != null ? player.GetComponent<Health>() : null;
        enemyHealth = enemy != null ? enemy.GetComponent<Health>() : null;
    }

    private void OnEnable()
    {
        if (playerHealth != null) playerHealth.OnDied += OnSomeoneDied;
        if (enemyHealth != null) enemyHealth.OnDied += OnSomeoneDied;
    }

    private void OnDisable()
    {
        if (playerHealth != null) playerHealth.OnDied -= OnSomeoneDied;
        if (enemyHealth != null) enemyHealth.OnDied -= OnSomeoneDied;
    }

    private void Start()
    {
        StartRound();
    }

    public void StartRound()
    {
        StopAllCoroutines();
        StartCoroutine(CountdownRoutine());
    }

    private void OnSomeoneDied(Health dead)
    {
        if (State != MatchState.InRound) return;

        State = MatchState.RoundEnd;

        bool playerDied = (playerHealth != null && dead == playerHealth);
        bool enemyDied = (enemyHealth != null && dead == enemyHealth);

        if (playerDied && !enemyDied) EnemyRoundsWon++;
        else if (enemyDied && !playerDied) PlayerRoundsWon++;
        // si jamais double mort => rien, on gérera plus tard

        Debug.Log($"ROUND {RoundIndex} END  |  Score P:{PlayerRoundsWon} - E:{EnemyRoundsWon}");
        // Check fin de match (BO3)
        if (PlayerRoundsWon >= roundsToWin || EnemyRoundsWon >= roundsToWin)
        {
            State = MatchState.MatchEnd;
            Debug.Log($"MATCH END — Winner: {(PlayerRoundsWon > EnemyRoundsWon ? "PLAYER" : "ENEMY")}");
            return;
        }
        StartCoroutine(RoundEndRoutine());

    }

    private IEnumerator RoundEndRoutine()
    {
        yield return new WaitForSeconds(roundEndDelay);

        RoundIndex++;
        StartRound();
    }

    private void ResetFighter(Transform t)
    {
        if (t == null) return;

        // HP
        var h = t.GetComponent<Health>();
        if (h != null) h.ResetHealth();

        // Status
        var sc = t.GetComponent<Arena.Combat.StatusController>();
        if (sc != null) sc.ClearAll();

        // Abilities
        var ar = t.GetComponent<Arena.Abilities.AbilityRunner>();
        if (ar != null) ar.ResetAll();

        // Combat state
        var cs = t.GetComponent<CombatState>();
        if (cs != null) cs.ForceSetCombat(false);

        // Shield / mods
        var mods = t.GetComponent<DamageModifiers>();
        if (mods != null) mods.ResetAll();

        // (Optionnel) stop movement velocity si tu en as plus tard
    }
    public void RestartMatch()
    {
        StopAllCoroutines();

        PlayerRoundsWon = 0;
        EnemyRoundsWon = 0;
        RoundIndex = 1;

        State = MatchState.Waiting;

        StartRound();
    }
}
