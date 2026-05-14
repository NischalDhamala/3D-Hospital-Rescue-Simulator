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
        // Detect ambulance via MissionManager component instead of tag
        MissionManager manager = other.GetComponent<MissionManager>();
        if (manager == null) manager = other.GetComponentInParent<MissionManager>();

        if (manager != null)
        {
            manager.OnMissionSuccess();

            // Play success effect if assigned
            if (successEffect != null)
            {
                var effect = Instantiate(successEffect, transform.position, Quaternion.identity);
                Destroy(effect, 5f);
            }
        }
    }
}
