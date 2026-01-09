using System.Collections;
using UnityEngine;

public class RespawnOnDeath : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private float respawnDelay = 3f;

    [Header("Spawn")]
    [SerializeField] private Transform spawnPoint;

    private CharacterController cc;
    private Vector3 fallbackSpawnPos;
    private Quaternion fallbackSpawnRot;

    private void Awake()
    {
        if (health == null) health = GetComponent<Health>();
        cc = GetComponent<CharacterController>();

        fallbackSpawnPos = transform.position;
        fallbackSpawnRot = transform.rotation;
    }

    private void OnEnable()
    {
        if (health != null)
            health.OnDied += HandleDied;
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnDied -= HandleDied;
    }

    private void HandleDied(Health h)
    {
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        // “Death state” minimal : on disable le rendu si présent
        SetRenderers(false);

        yield return new WaitForSeconds(respawnDelay);

        Vector3 pos = spawnPoint != null ? spawnPoint.position : fallbackSpawnPos;
        Quaternion rot = spawnPoint != null ? spawnPoint.rotation : fallbackSpawnRot;

        // Reset position propre (CharacterController)
        if (cc != null) cc.enabled = false;
        transform.SetPositionAndRotation(pos, rot);
        if (cc != null) cc.enabled = true;

        if (health != null) health.ResetHealth();

        SetRenderers(true);
    }

    private void SetRenderers(bool enabled)
    {
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = enabled;
    }
}
