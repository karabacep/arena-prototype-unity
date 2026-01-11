using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Arena.Abilities;

public class AbilityPicker : MonoBehaviour
{
    [SerializeField] private RectTransform listRoot;
    [SerializeField] private GameObject entryPrefab; // simple button prefab with Icon and Label children
    [SerializeField] private Button closeButton;

    private Action<AbilityData> onSelected;

    private void Awake()
    {
        if (closeButton != null) closeButton.onClick.AddListener(() => Hide());
        gameObject.SetActive(false);
    }

    public void Show(List<AbilityData> choices, Action<AbilityData> callback)
    {
        onSelected = callback;
        ClearList();
        foreach (var a in choices)
        {
            var go = Instantiate(entryPrefab, listRoot);
            var btn = go.GetComponent<Button>();
            var img = go.transform.Find("Icon")?.GetComponent<Image>();
            var txt = go.transform.Find("Label")?.GetComponent<TMP_Text>();

            if (img != null) img.sprite = a?.icon;
            if (txt != null) txt.text = a != null ? a.displayName : "Empty";

            btn.onClick.AddListener(() =>
            {
                onSelected?.Invoke(a);
                Hide();
            });
        }
        gameObject.SetActive(true);
    }

    private void ClearList()
    {
        if (listRoot == null) return;
        for (int i = listRoot.childCount - 1; i >= 0; i--)
            Destroy(listRoot.GetChild(i).gameObject);
    }

    public void Hide()
    {
        onSelected = null;
        ClearList();
        gameObject.SetActive(false);
    }
}