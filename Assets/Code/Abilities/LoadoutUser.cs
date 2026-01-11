using UnityEngine;

[RequireComponent(typeof(Arena.Abilities.AbilityRunner))]
public class LoadoutUser : MonoBehaviour
{
    [SerializeField] private AbilityLoadout loadout;

    private Arena.Abilities.AbilityRunner runner;

    private void Awake()
    {
        runner = GetComponent<Arena.Abilities.AbilityRunner>();
        if (runner != null && loadout != null)
            runner.SetLoadout(loadout);
    }
}
