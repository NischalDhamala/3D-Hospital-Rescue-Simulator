using UnityEngine;

/// <summary>
/// Camera follow script for the Hospital Rescue Simulator.
/// Positions the camera behind and above the target (ambulance) at a 3/4 perspective angle,
/// smoothly following both position and rotation so the road ahead is always visible.
/// Attach this script to the Main Camera.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target to follow (e.g., the ambulance)")]
    public Transform target;

    [Header("Distance behind the target")]
    public float followDistance = 14f;

    [Header("Height above the target")]
    public float height = 12f;

    [Header("Tilt angle (degrees, how much to look down)")]
    [Range(10f, 80f)]
    public float tiltAngle = 12f;

    [Header("Smooth follow speed")]
    public float smoothSpeed = 9f;

    [Header("Rotation smooth speed")]
    public float rotSmoothSpeed = 7f;
    // Velocity used by SmoothDamp for position smoothing
    private Vector3 velocity;

    // Auto‑assign the ambulance as the target if not set in the inspector
    void Awake()
    {
        if (target == null)
        {
            MissionManager mm = FindFirstObjectByType<MissionManager>();
            if (mm != null) target = mm.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Use only the Y rotation of the target so camera follows horizontal turns
        float targetYaw = target.eulerAngles.y;
        Quaternion yawRotation = Quaternion.Euler(0f, targetYaw, 0f);

        // Offset: back by followDistance, up by height (in target's local horizontal space)
        Vector3 offset = yawRotation * new Vector3(0f, height, -followDistance);
        Vector3 desiredPos = target.position + offset;

        // Smoothly move the camera
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, 1f / smoothSpeed);

        // Camera looks at a point slightly above the target (not the ground, for better feel)
        Vector3 lookTarget = target.position + Vector3.up * 1.5f;
        Quaternion desiredRot = Quaternion.LookRotation(lookTarget - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, rotSmoothSpeed * Time.deltaTime);
    }
}
