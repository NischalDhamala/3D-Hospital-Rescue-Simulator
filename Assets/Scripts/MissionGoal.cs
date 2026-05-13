using UnityEngine;

/// <summary>
/// Attach this script to the goal trigger (e.g., an empty GameObject placed in front of the hospital).
/// When the ambulance (tagged "Player") enters the trigger, it notifies the MissionManager that the mission was successful.
/// </summary>
public class MissionGoal : MonoBehaviour
{
    // Optional visual feedback when the goal is reached
    public GameObject successEffect;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the ambulance (tagged as "Player")
        if (other.CompareTag("Player") || other.CompareTag("Ambulance"))
        {
            // Find the MissionManager in the scene (assumes only one exists)
            MissionManager manager = FindObjectOfType<MissionManager>();
            if (manager != null)
            {
                manager.OnMissionSuccess();
            }
            else
            {
                Debug.LogWarning("MissionManager not found in scene when goal trigger activated.");
            }

            // Play success effect if assigned
            if (successEffect != null)
            {
                var effect = Instantiate(successEffect, transform.position, Quaternion.identity);
                // Optionally destroy after some time
                Destroy(effect, 5f);
            }
        }
    }
}
