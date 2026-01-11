using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Arena.Abilities;

[RequireComponent(typeof(Button))]
public class HotbarSlotButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private TMP_Text label;

    public int SlotIndex { get; private set; }
    private AbilityData ability;
    private System.Action<int> onClick;

    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(HandleClick);
        if (icon == null) icon = transform.Find("Icon")?.GetComponent<Image>();
        if (cooldownOverlay == null) cooldownOverlay = transform.Find("Cooldown")?.GetComponent<Image>();
        if (label == null) label = transform.Find("Label")?.GetComponent<TMP_Text>();
    }

    public void Initialize(int index, AbilityData initial, System.Action<int> clickCallback)
    {
        SlotIndex = index;
        ability = initial;
        onClick = clickCallback;
        Refresh();
    }

    public void SetAbility(AbilityData a)
    {
        ability = a;
        Refresh();
    }

    private void Refresh()
    {
        if (ability != null)
        {
            if (icon != null) icon.sprite = ability.icon;
            if (label != null) label.text = ability.displayName;
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
        }
        else
        {
            if (icon != null) icon.sprite = null;
            if (label != null) label.text = "";
            if (cooldownOverlay != null) cooldownOverlay.fillAmount = 0f;
        }
    }

    private void HandleClick()
    {
        onClick?.Invoke(SlotIndex);
    }
}