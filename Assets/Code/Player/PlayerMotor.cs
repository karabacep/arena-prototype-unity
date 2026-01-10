using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private Transform cameraYaw; // pivot direction caméra

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6.0f;
    [SerializeField] private float gravity = -20f;

    private CharacterController controller;
    private float verticalVelocity;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (input == null) input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        // On ne stoppe JAMAIS l'Update complet : sinon plus de gravité => "vol"
        var mm = FindObjectOfType<MatchManager>();
        bool canAct = (mm == null) || mm.CanAct;

        ApplyGravity();

        // Stun = pas de mouvement horizontal, mais on garde gravité
        var status = GetComponent<Arena.Combat.StatusController>();
        bool stunned = status != null && status.Has(Arena.Combat.StatusType.Stun);

        Move(canAct && !stunned);
    }

    private void ApplyGravity()
    {
        // Sticky gravity pour rester au sol
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
    }

    private void Move(bool allowHorizontalMove)
    {
        // Si on ne peut pas agir, on met l'input à 0 (mais on applique quand même la gravité)
        Vector2 move = (input != null && allowHorizontalMove) ? input.MoveInput : Vector2.zero;

        // Direction relative à la caméra (WoW-like)
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
