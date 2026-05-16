using UnityEngine;

public class GoalPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hospital Reached!");
            // Find RescueTimer script and call MissionComplete
            RescueTimer timer = FindFirstObjectByType<RescueTimer>();
            if (timer != null)
            {
                timer.MissionComplete();
            }
        }
    }
}