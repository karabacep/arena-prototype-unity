using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIUnitFrame : MonoBehaviour
{
    [Header("Bindings")]
    [SerializeField] private Health health;

    [Header("UI")]
    [SerializeField] private Image fill;
    [SerializeField] private TMP_Text label;

    private void Awake()
    {
        // Auto-assign si tu as bien "Fill" et "Label" dans l'objet
        if (fill == null)
            fill = transform.Find("Fill")?.GetComponent<Image>();

        if (label == null)
            label = transform.Find("Label")?.GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (health == null || fill == null) return;

        var info = health.GetHealthInfo(); // API carte 2

        fill.fillAmount = info.normalized;

        if (label != null)
            label.text = $"{info.current:0}/{info.max:0}";
    }

    public void SetHealth(Health h)
    {
        health = h;

        // Optionnel : vider l'affichage si pas de target
        if (health == null)
        {
            if (fill != null) fill.fillAmount = 0f;
            if (label != null) label.text = "";
        }
    }
}
