using UnityEngine;
using UnityEngine.InputSystem;

namespace Arena.Abilities
{
    public class EnemyCasterTest : MonoBehaviour
    {
        [SerializeField] private AbilityRunner runner;
        [SerializeField] private AbilityData fireball;
        [SerializeField] private Transform target; // Player

        private void Awake()
        {
            if (runner == null) runner = GetComponent<AbilityRunner>();
        }

        private void Update()
        {
            if (runner == null || fireball == null || target == null) return;

            // Appuie sur K pour faire caster l'ennemi
            if (Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame)
            {
                runner.TryCastOnTarget(fireball, target);
            }
        }
    }
}
