using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private Transform cameraYaw; // un pivot qui représente la direction caméra (on le crée après)

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
        ApplyGravity();
        var status = GetComponent<Arena.Combat.StatusController>();
        if (status != null && status.Has(Arena.Combat.StatusType.Stun)) return;
        Move();
    }

    private void ApplyGravity()
    {
        // Petite “sticky gravity” pour rester bien au sol
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
    }

    private void Move()
    {
        Vector2 move = input != null ? input.MoveInput : Vector2.zero;

        // Direction relative à la caméra (WoW-like)
        Vector3 forward = cameraYaw != null ? cameraYaw.forward : transform.forward;
        Vector3 right = cameraYaw != null ? cameraYaw.right : transform.right;

        forward.y = 0f; right.y = 0f;
        forward.Normalize(); right.Normalize();

        Vector3 desired = (forward * move.y + right * move.x);
        if (desired.sqrMagnitude > 1f) desired.Normalize();

        Vector3 velocity = desired * moveSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }
}
