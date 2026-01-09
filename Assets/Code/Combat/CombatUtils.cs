using UnityEngine;

public static class CombatUtils
{
    public static bool IsInRange(Transform a, Transform b, float range)
    {
        if (a == null || b == null) return false;
        float sqr = (b.position - a.position).sqrMagnitude;
        return sqr <= range * range;
    }

    public static bool HasLineOfSight(Transform fromAim, Transform toAim, LayerMask blockers)
    {
        if (fromAim == null || toAim == null) return false;

        Vector3 from = fromAim.position;
        Vector3 to = toAim.position;
        Vector3 dir = to - from;
        float dist = dir.magnitude;

        if (dist < 0.01f) return true;
        dir /= dist;

        // True si aucun obstacle entre from et to
        return !Physics.Raycast(from, dir, dist, blockers, QueryTriggerInteraction.Ignore);
    }
}
