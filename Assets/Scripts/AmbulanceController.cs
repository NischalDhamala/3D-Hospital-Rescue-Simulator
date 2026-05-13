using UnityEngine;

/// <summary>
/// Handles manual driving of the ambulance using keyboard input.
/// Uses simple raycasts to avoid driving through obstacles.
/// </summary>
public class AmbulanceController : MonoBehaviour
{
    // Movement speed in units per second
    public float moveSpeed = 8f;
    // Rotation speed (degrees per second)
    public float turnSpeed = 120f;
    // Distance to check for obstacles ahead
    public float obstacleCheckDistance = 2f;
    // LayerMask for obstacles (default to everything except the ambulance itself)
    public LayerMask obstacleLayers = ~0;

    private void Update()
    {
        // Get input axes
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Rotate based on horizontal input
        if (Mathf.Abs(h) > 0.01f)
        {
            transform.Rotate(Vector3.up, h * turnSpeed * Time.deltaTime);
        }

        // Move forward/backward based on vertical input
        if (Mathf.Abs(v) > 0.01f)
        {
            // Raycast to detect obstacle directly in front of the vehicle
            Vector3 rayOrigin = transform.position + Vector3.up * 0.5f; // slightly above ground
            Vector3 rayDirection = transform.forward;
            if (!Physics.Raycast(rayOrigin, rayDirection, obstacleCheckDistance, obstacleLayers))
            {
                // No obstacle, move forward
                transform.Translate(Vector3.forward * v * moveSpeed * Time.deltaTime);
            }
            else
            {
                // Obstacle detected – stop forward motion
                // Optional: play a sound or UI feedback here
            }
        }
    }
}
