using System;
using System.Collections.Generic;
using UnityEngine;
using Arena.Abilities;

[Serializable]
public class HotbarPersistence
{
    [SerializeField] private string prefsKey = "hotbar_v1";

    [Serializable]
    private class HotbarData
    {
        public List<string> abilityIds = new List<string>();
    }

    // Sauvegarde : stocke la liste d'abilityId (null => empty string)
    public void Save(HotbarConfig config, AbilityRunner resolver)
    {
        if (config == null) return;
        var d = new HotbarData();
        int total = config.TotalSlots;
        for (int i = 0; i < total; i++)
        {
            var a = config.GetSlot(i);
            d.abilityIds.Add(a != null ? a.abilityId : "");
        }
        string json = JsonUtility.ToJson(d);
        PlayerPrefs.SetString(prefsKey, json);
        PlayerPrefs.Save();
        Debug.Log($"HotbarPersistence: saved {d.abilityIds.Count} slots");
    }

    // Load : résout les abilityId via le Loadout du playerRunner (unique source)
    public bool Load(HotbarConfig config, AbilityRunner resolver)
    {
        if (config == null || resolver == null || resolver.Loadout == null) return false;
        if (!PlayerPrefs.HasKey(prefsKey)) return false;

        string json = PlayerPrefs.GetString(prefsKey);
        if (string.IsNullOrEmpty(json)) return false;

        try
        {
            var d = JsonUtility.FromJson<HotbarData>(json);
            if (d == null || d.abilityIds == null) return false;

            int total = config.TotalSlots;
            for (int i = 0; i < total && i < d.abilityIds.Count; i++)
            {
                string id = d.abilityIds[i];
                if (string.IsNullOrEmpty(id))
                {
                    config.SetSlot(i, null);
                    continue;
                }

                // Résoudre via loadout
                AbilityData found = null;
                for (int j = 0; j < resolver.Loadout.SlotCount; j++)
                {
                    var a = resolver.Loadout.Get(j);
                    if (a != null && a.abilityId == id) { found = a; break; }
                }

                config.SetSlot(i, found);
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"HotbarPersistence: load failed: {e.Message}");
            return false;
        }
    }

    public void Reset()
    {
        PlayerPrefs.DeleteKey(prefsKey);
    }
}