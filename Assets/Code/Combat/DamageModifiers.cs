using UnityEngine;

public class DamageModifiers : MonoBehaviour
{
    private float shieldEndTime;
    private float shieldMultiplier = 1f; // 1 = normal

    public void ApplyShield(float duration, float damageMultiplier)
    {
        shieldEndTime = Time.time + duration;
        shieldMultiplier = Mathf.Clamp(damageMultiplier, 0f, 1f);
    }

    public float ModifyIncomingDamage(float dmg)
    {
        if (Time.time <= shieldEndTime)
            return dmg * shieldMultiplier;
        return dmg;
    }
    public void ResetAll()
    {
        ApplyShield(0f, 1f);
    }

}
