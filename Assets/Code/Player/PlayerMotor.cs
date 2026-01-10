using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private Transform cameraYaw; // pivot direction caméra
    [SerializeField] private Arena.Abilities.AbilityRunner abilityRunner;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6.0f;
    [SerializeField] private float gravity = -20f;

    [Header("Manager")]
    [SerializeField] private MatchManager match;

    private CharacterController controller;
    private float verticalVelocity;

    // Cache (évite GetComponent chaque frame)
    private Arena.Combat.StatusController status;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (input == null) input = GetComponent<PlayerInputHandler>();
        if (abilityRunner == null) abilityRunner = GetComponent<Arena.Abilities.AbilityRunner>();
        if (match == null) match = FindFirstObjectByType<MatchManager>();

        status = GetComponent<Arena.Combat.StatusController>();
    }

    private void Update()
    {
        bool canAct = (match == null) || match.CanAct;

        ApplyGravity();

        bool stunned = status != null && status.Has(Arena.Combat.StatusType.Stun);

        bool isCasting = abilityRunner != null && abilityRunner.GetCastInfo().isCasting;
        bool canMoveWhileCasting = status != null && status.Has(Arena.Combat.StatusType.CastWhileMoving);

        // Micro-input = cancel (WoW-like). Rotation caméra ne cancel pas car on regarde MoveInput.
        Vector2 moveInput = input != null ? input.MoveInput : Vector2.zero;
        bool wantsMove = moveInput.sqrMagnitude > 0f;

        // Bouger pendant un cast => cancel (sauf buff)
        if (canAct && isCasting && !canMoveWhileCasting && wantsMove)
        {
            if (abilityRunner != null)
                abilityRunner.CancelCast();

            Move(false); // pas de déplacement horizontal ce frame
            return;
        }

        bool allowHorizontalMove = canAct && !stunned;
        Move(allowHorizontalMove);
    }

    private void ApplyGravity()
    {
        // Sticky gravity
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
    }

    private void Move(bool allowHorizontalMove)
    {
        Vector2 move = (input != null && allowHorizontalMove) ? input.MoveInput : Vector2.zero;

        Vector3 forward = cameraYaw != null ? cameraYaw.forward : transform.forward;
        Vector3 right = cameraYaw != null ? cameraYaw.right : transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 desired = (forward * move.y + right * move.x);
        if (desired.sqrMagnitude > 1f) desired.Normalize();

        Vector3 velocity = desired * moveSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }
}
