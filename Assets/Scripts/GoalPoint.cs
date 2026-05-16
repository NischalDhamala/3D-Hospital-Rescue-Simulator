using UnityEngine;

public class GoalPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hospital Reached!");
            // RescueTimer script khojera MissionComplete function chalaucha
            RescueTimer timer = FindFirstObjectByType<RescueTimer>();
            if (timer != null)
            {
                timer.MissionComplete();
            }
        }
    }
}