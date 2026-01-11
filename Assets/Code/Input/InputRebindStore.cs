using UnityEngine;
using UnityEngine.InputSystem;

public class InputRebindStore : MonoBehaviour
{
    [SerializeField] private InputActionAsset actionsAsset;
    [SerializeField] private string playerPrefsKey = "rebinds_v1";

    private void Awake()
    {
        Load();
    }

    public void Save()
    {
        if (actionsAsset == null) return;
        string json = actionsAsset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(playerPrefsKey, json);
        PlayerPrefs.Save();
    }

    public void Load()
    {
        if (actionsAsset == null) return;
        if (!PlayerPrefs.HasKey(playerPrefsKey)) return;

        string json = PlayerPrefs.GetString(playerPrefsKey);
        actionsAsset.LoadBindingOverridesFromJson(json);
    }

    public void ResetAll()
    {
        if (actionsAsset == null) return;
        actionsAsset.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey(playerPrefsKey);
    }

    // Rebind 1 binding d'une action (par défaut bindingIndex=0)
    public InputActionRebindingExtensions.RebindingOperation StartRebind(
        string mapName,
        string actionName,
        int bindingIndex,
        System.Action onComplete = null,
        System.Action onCancel = null)
    {
        var map = actionsAsset.FindActionMap(mapName, true);
        var action = map.FindAction(actionName, true);

        action.Disable();

        var op = action.PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnComplete(o =>
            {
                o.Dispose();
                action.Enable();
                Save();
                onComplete?.Invoke();
            })
            .OnCancel(o =>
            {
                o.Dispose();
                action.Enable();
                onCancel?.Invoke();
            });

        op.Start();
        return op;
    }
}
