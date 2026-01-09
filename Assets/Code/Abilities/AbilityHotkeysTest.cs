using UnityEngine;

namespace Arena.Abilities
{
    public class AbilityHotkeysTest : MonoBehaviour
    {
        [SerializeField] private PlayerInputHandler input;
        [SerializeField] private AbilityRunner runner;
        [SerializeField] private AbilityData ability1; // Fireball
        [SerializeField] private AbilityData ability2; // Kick
        [SerializeField] private AbilityData ability3; // Stun
        [SerializeField] private AbilityData ability4; // Silence
        [SerializeField] private AbilityData ability5; // Shield



        private void Awake()
        {
            if (input == null) input = GetComponent<PlayerInputHandler>();
            if (runner == null) runner = GetComponent<AbilityRunner>();
        }

        private void Update()
        {
            if (input == null || runner == null) return;

            if (input.Cast1Pressed)
            {
                runner.TryCast(ability1);
                input.ConsumeCastButtons();
            }
            if (input.Cast2Pressed)
            {
                runner.TryCast(ability2);
                input.ConsumeCastButtons();
            }
            if (input.Cast3Pressed)
            {
                runner.TryCast(ability3);
                input.ConsumeCastButtons();
            }

            if (input.Cast4Pressed)
            {
                runner.TryCast(ability4);
                input.ConsumeCastButtons();
            }

            if (input.Cast5Pressed)
            {
                runner.TryCast(ability5);
                input.ConsumeCastButtons();
            }

        }
    }
}
