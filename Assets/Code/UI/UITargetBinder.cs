using UnityEngine;

public class UITargetBinder : MonoBehaviour
{
    [SerializeField] private TargetingSystem targeting;
    [SerializeField] private UIUnitFrame targetFrame;

    private Transform lastTarget;
    private CanvasGroup cg;

    private void Awake()
    {
        if (targetFrame == null) targetFrame = GetComponent<UIUnitFrame>();

        cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();

        SetVisible(false);
    }

    private void Update()
    {
        if (targeting == null || targetFrame == null) return;

        var t = targeting.CurrentTarget;
        if (t == lastTarget) return;

        lastTarget = t;

        if (t == null)
        {
            targetFrame.SetHealth(null);
            SetVisible(false);
            return;
        }

        targetFrame.SetHealth(t.GetComponent<Health>());
        SetVisible(true);
    }

    private void SetVisible(bool visible)
    {
        cg.alpha = visible ? 1f : 0f;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;
    }
}
