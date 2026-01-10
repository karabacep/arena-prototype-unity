using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Arena.UI;

public class UITargetCastBar : MonoBehaviour
{
    [SerializeField] private TargetCastProvider provider;

    [Header("UI")]
    [SerializeField] private Image fill;
    [SerializeField] private TMP_Text label;

    private CanvasGroup cg;

    private void Awake()
    {
        if (fill == null) fill = transform.Find("Fill")?.GetComponent<Image>();
        if (label == null) label = transform.Find("Label")?.GetComponent<TMP_Text>();

        cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();

        SetVisible(false);
    }

    private void Update()
    {
        if (provider == null) { SetVisible(false); return; }

        CastInfo info = provider.GetTargetCastInfo();

        if (!info.isCasting)
        {
            SetVisible(false);
            return;
        }

        SetVisible(true);

        if (fill != null) fill.fillAmount = Mathf.Clamp01(info.normalized);
        if (label != null) label.text = $"{info.displayName}  {info.remaining:0.0}/{info.castDuration:0.0}s";

    }

    private void SetVisible(bool visible)
    {
        cg.alpha = visible ? 1f : 0f;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;
    }
}
