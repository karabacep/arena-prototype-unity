using System.Collections.Generic;
using UnityEngine;
using Arena.Abilities;
using UnityEngine.UI;

public class HotbarUI : MonoBehaviour
{
    [Header("Config & Data")]
    [SerializeField] private HotbarConfig config;
    [SerializeField] private AbilityRunner playerRunner; // source des abilities disponibles (Loadout)

    [Header("UI Prefabs")]
    [SerializeField] private GameObject slotButtonPrefab; // prefab with HotbarSlotButton
    [SerializeField] private RectTransform barContainer; // parent where bars will be created
    [SerializeField] private AbilityPicker picker;

    [Header("Persistence (optional)")]
    [SerializeField] private HotbarPersistence persistence;

    private List<GameObject> barObjects = new List<GameObject>();
    private List<HotbarSlotButton> slotButtons = new List<HotbarSlotButton>();

    private void Awake()
    {
        if (config == null) { Debug.LogWarning("HotbarUI: config manquant"); return; }
        if (playerRunner == null) playerRunner = GetComponentInParent<AbilityRunner>();
        BuildUI();
    }

    private void BuildUI()
    {
        foreach (var b in barObjects) Destroy(b);
        barObjects.Clear();
        slotButtons.Clear();

        int slotCursor = 0;
        for (int bar = 0; bar < config.barCount; bar++)
        {
            int length = config.barLengths[bar];
            var barGO = new GameObject($"Hotbar_{bar}", typeof(RectTransform));
            barGO.transform.SetParent(barContainer, false);
            var h = barGO.AddComponent<HorizontalLayoutGroup>();
            h.spacing = 6;
            barObjects.Add(barGO);

            for (int s = 0; s < length; s++)
            {
                int slotIndex = slotCursor++;
                var btnGO = Instantiate(slotButtonPrefab, barGO.transform);
                var slotComp = btnGO.GetComponent<HotbarSlotButton>();
                slotComp.Initialize(slotIndex, config.GetSlot(slotIndex), OnSlotClicked);
                slotButtons.Add(slotComp);
            }
        }
    }

    private void OnSlotClicked(int slotIndex)
    {
        List<AbilityData> choices = new List<AbilityData>();
        if (playerRunner != null && playerRunner.Loadout != null)
        {
            for (int i = 0; i < playerRunner.Loadout.SlotCount; i++)
            {
                var a = playerRunner.Loadout.Get(i);
                if (a != null && !choices.Contains(a)) choices.Add(a);
            }
        }

        choices.Insert(0, null); // option clear

        picker.Show(choices, selected =>
        {
            config.SetSlot(slotIndex, selected);
            var btn = slotButtons.Find(b => b != null && b.SlotIndex == slotIndex);
            btn?.SetAbility(selected);

            // sauvegarde si persistence fournie
            if (persistence != null)
                persistence.Save(config, playerRunner);
        });
    }
}