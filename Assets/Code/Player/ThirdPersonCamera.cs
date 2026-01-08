using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;              // Player
    [SerializeField] private Transform yawPivot;            // CameraYaw
    [SerializeField] private Transform pitchPivot;          // CameraPitch
    [SerializeField] private PlayerInputHandler input;

    [Header("Rotation")]
    [SerializeField] private float yawSpeed = 0.15f;
    [SerializeField] private float pitchSpeed = 0.12f;
    [SerializeField] private float pitchMin = -35f;
    [SerializeField] private float pitchMax = 65f;

    [Header("Zoom")]
    [SerializeField] private float distance = 6f;
    [SerializeField] private float minDistance = 2.5f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float zoomSpeed = 0.6f;

    [Header("Follow")]
    [SerializeField] private Vector3 followOffset = new Vector3(0f, 1.6f, 0f);

    private Transform cam;

    private void Awake()
    {
        cam = Camera.main != null ? Camera.main.transform : null;
        if (input == null && target != null) input = target.GetComponent<PlayerInputHandler>();
    }

    private void LateUpdate()
    {
        if (target == null || yawPivot == null || pitchPivot == null || cam == null) return;

        // Follow
        transform.position = target.position + followOffset;

        // Zoom (scroll Y)
        float zoom = input != null ? input.ZoomInput : 0f;
        if (Mathf.Abs(zoom) > 0.01f)
        {
            distance = Mathf.Clamp(distance - zoom * zoomSpeed, minDistance, maxDistance);
        }

        // Rotate only while RMB held
        if (input != null && input.IsRotateHeld)
        {
            Vector2 look = input.LookInput;

            float yaw = look.x * yawSpeed;
            float pitch = -look.y * pitchSpeed;

            yawPivot.Rotate(0f, yaw, 0f, Space.Self);

            Vector3 euler = pitchPivot.localEulerAngles;
            float currentPitch = NormalizeAngle(euler.x);
            currentPitch = Mathf.Clamp(currentPitch + pitch, pitchMin, pitchMax);
            pitchPivot.localEulerAngles = new Vector3(currentPitch, 0f, 0f);
        }

        // Position camera at distance
        cam.localPosition = new Vector3(0f, 0f, -distance);
        cam.localRotation = Quaternion.identity;
    }

    private static float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        return angle;
    }
}
