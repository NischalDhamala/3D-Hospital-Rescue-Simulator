using UnityEngine;

public class RescueManager : MonoBehaviour
{
    public GameObject rescueMessage; // The "Press F" message
    public GameObject startAmbulanceMessage; // The "Press M" message

    void Start()
    {
        // Show only 'F' message at game start, hide 'M' message
        rescueMessage.SetActive(true);
        startAmbulanceMessage.SetActive(false);
    }

    void Update()
    {
        // STEP 1: If 'F' is pressed
        if (rescueMessage.activeSelf && Input.GetKeyDown(KeyCode.F))
        {
            rescueMessage.SetActive(false); // F message hidden
            startAmbulanceMessage.SetActive(true); // Now show M message
            Debug.Log("Patient Rescued! Now start the ambulance.");
        }

        // STEP 2: If 'M' is pressed (when active)
        else if (startAmbulanceMessage.activeSelf && Input.GetKeyDown(KeyCode.M))
        {
            startAmbulanceMessage.SetActive(false); // M message also hidden
            Debug.Log("Ambulance Started!");
            
            // Add ambulance start logic here
        }
    }
}