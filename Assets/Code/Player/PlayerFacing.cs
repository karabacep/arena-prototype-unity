using UnityEngine;

public class PlayerFacing : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private Transform cameraYaw;
    [SerializeField] private Transform visual;

    private void Awake()
    {
        if (input == null) input = GetComponent<PlayerInputHandler>();
    }

    private void Update()
    {
        if (input == null || cameraYaw == null) return;

        if (input.IsRotateHeld)
        {
            Vector3 forward = cameraYaw.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude > 0.0001f)
                visual.rotation = Quaternion.LookRotation(forward);
        }
    }
    public void SnapToDirection(Vector3 forward)
    {
        if (visual == null) return;

        forward.y = 0f;
        if (forward.sqrMagnitude < 0.0001f) return;

        visual.rotation = Quaternion.LookRotation(forward.normalized);
    }

}
