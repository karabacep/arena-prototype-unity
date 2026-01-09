using UnityEngine;

public class TargetRingFollower : MonoBehaviour
{
    [SerializeField] private TargetingSystem targeting;
    [SerializeField] private float yHeight = 0.05f;

    private Renderer rend;

    private void Awake()
    {
        rend = GetComponentInChildren<Renderer>();
        if (rend != null) rend.enabled = false; // caché au départ
    }

    private void Update()
    {
        if (targeting == null || rend == null) return;

        Transform t = targeting.CurrentTarget;

        if (t == null)
        {
            rend.enabled = false;
            return;
        }

        rend.enabled = true;

        Vector3 p = t.position;
        p.y = yHeight;
        transform.position = p;
    }
}
