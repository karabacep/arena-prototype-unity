using System.Collections;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public enum MatchState { Waiting, InRound, RoundEnd }

    [Header("Participants")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform enemy;

    [Header("Round")]
    [SerializeField] private float roundEndDelay = 2f;

    public MatchState State { get; private set; } = MatchState.Waiting;

    public int PlayerRoundsWon { get; private set; }
    public int EnemyRoundsWon { get; private set; }
    public int RoundIndex { get; private set; } = 1;

    private Health playerHealth;
    private Health enemyHealth;

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
        State = MatchState.InRound;

        // Reset complet des deux côtés
        ResetFighter(player);
        ResetFighter(enemy);

        // (Optionnel) reposition : on laisse RespawnOnDeath le faire pour l’instant
        // ou on te mettra des spawn points dans carte 5/6.

        Debug.Log($"ROUND {RoundIndex} START");
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
}
