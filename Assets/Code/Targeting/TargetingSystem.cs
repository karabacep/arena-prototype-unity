using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class TargetingSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private Camera mainCam;

    [Header("Targeting")]
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private float maxClickDistance = 100f;
    [SerializeField] private float tabMaxDistance = 30f;

    public Transform CurrentTarget { get; private set; }

    private void Awake()
    {
        if (input == null) input = GetComponent<PlayerInputHandler>();
        if (mainCam == null) mainCam = Camera.main;
    }

    private void Update()
    {
        if (input == null) return;

        if (input.TargetPressed)
            TryTargetFromMouse();

        if (input.CycleTargetPressed)
            CycleTarget();

        input.ConsumeOneFrameButtons();
    }

    private void TryTargetFromMouse()
    {
        if (mainCam == null) return;
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCam.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, maxClickDistance, targetMask, QueryTriggerInteraction.Ignore))
        {
            SetTarget(hit.transform);
        }
    }


    private void CycleTarget()
    {
        // Cherche tous les targetables dans un rayon, puis prend le "plus proche"
        Collider[] cols = Physics.OverlapSphere(transform.position, tabMaxDistance, targetMask, QueryTriggerInteraction.Ignore);
        if (cols.Length == 0)
        {
            SetTarget(null);
            return;
        }

        List<Transform> candidates = new List<Transform>(cols.Length);
        foreach (var c in cols)
        {
            if (c.transform == transform) continue;
            candidates.Add(c.transform);
        }

        if (candidates.Count == 0)
        {
            SetTarget(null);
            return;
        }

        // Si pas de target : plus proche
        if (CurrentTarget == null)
        {
            SetTarget(GetClosest(candidates));
            return;
        }

        // Sinon : cycle “prochain” selon un tri par angle + distance (simple et efficace)
        Transform next = GetNextByAngle(candidates, CurrentTarget);
        SetTarget(next);
    }

    private Transform GetClosest(List<Transform> list)
    {
        Transform best = null;
        float bestDist = float.MaxValue;
        foreach (var t in list)
        {
            float d = (t.position - transform.position).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = t;
            }
        }
        return best;
    }

    private Transform GetNextByAngle(List<Transform> list, Transform current)
    {
        Vector3 forward = transform.forward;
        Vector3 origin = transform.position;

        // Référence angle du current
        float currentAngle = SignedAngleOnY(forward, (current.position - origin));

        Transform best = null;
        float bestAngleDelta = float.MaxValue;

        foreach (var t in list)
        {
            if (t == current) continue;

            float a = SignedAngleOnY(forward, (t.position - origin));
            float delta = a - currentAngle;
            if (delta <= 0f) delta += 360f;

            if (delta < bestAngleDelta)
            {
                bestAngleDelta = delta;
                best = t;
            }
        }

        // Si rien trouvé (rare), fallback closest
        return best != null ? best : GetClosest(list);
    }

    private float SignedAngleOnY(Vector3 from, Vector3 to)
    {
        from.y = 0; to.y = 0;
        if (from.sqrMagnitude < 0.0001f || to.sqrMagnitude < 0.0001f) return 0f;
        from.Normalize(); to.Normalize();
        return Vector3.SignedAngle(from, to, Vector3.up);
    }

    private void SetTarget(Transform t)
    {
        CurrentTarget = t;
        // Plus tard: event OnTargetChanged(CurrentTarget)
    }
    public void ClearTarget()
    {
        SetTarget(null);
    }

}
