using UnityEngine;

/// <summary>
/// Camera follow script tailored for the Hospital Rescue Simulator.
/// The camera stays directly above the ambulance (or any target) with a configurable height
/// and smoothly follows the target's position. No LookAt is used so the view stays top‑down.
/// Attach this script to the Main Camera.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target to follow (e.g., the ambulance)")]
    public Transform target;

    [Header("Height above the target")]
    public float height = 25f;

    [Header("Horizontal offset (optional)")]
    public Vector3 horizontalOffset = Vector3.zero;

    [Header("Smooth follow speed")]
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        // Desired position is directly above the target plus any horizontal offset
        Vector3 desiredPos = target.position + horizontalOffset + Vector3.up * height;
        // Smoothly move the camera
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
        // Keep the camera looking straight down
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
