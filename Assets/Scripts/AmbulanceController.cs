using UnityEngine;

/// <summary>
/// Handles manual driving of the ambulance using keyboard input.
/// Uses simple raycasts to avoid driving through obstacles.
/// Space bar applies a brake that slows the ambulance to a stop.
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

    // Braking
    [Header("Brake Settings")]
    public float brakeDeceleration = 20f;

    // Internal velocity for smooth braking
    private float currentSpeed = 0f;
    private bool isBraking = false;

    private void Update()
    {
        // Get input axes
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // --- Brake with Space bar ---
        isBraking = Input.GetKey(KeyCode.Space);

        // Rotate based on horizontal input (allow turning even when braking)
        if (Mathf.Abs(h) > 0.01f)
        {
            transform.Rotate(Vector3.up, h * turnSpeed * Time.deltaTime);
        }

        // Calculate target speed based on input
        float targetSpeed = v * moveSpeed;

        if (isBraking)
        {
            // Decelerate towards zero
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, brakeDeceleration * Time.deltaTime);
        }
        else
        {
            // Accelerate towards target speed
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, moveSpeed * 2f * Time.deltaTime);
        }

        // Move forward/backward if speed is non-zero
        if (Mathf.Abs(currentSpeed) > 0.01f)
        {
            // Raycast to detect obstacle directly in front of the vehicle
            Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
            Vector3 rayDirection = currentSpeed > 0 ? transform.forward : -transform.forward;

            if (!Physics.Raycast(rayOrigin, rayDirection, obstacleCheckDistance, obstacleLayers))
            {
                transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
            }
            else
            {
                // Obstacle detected – stop
                currentSpeed = 0f;
            }
        }
    }
}
