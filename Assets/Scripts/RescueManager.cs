using UnityEngine;

public class RescueManager : MonoBehaviour
{
    public GameObject rescueMessage; // Press F wala
    public GameObject startAmbulanceMessage; // Press M wala

    void Start()
    {
        // Game suru huda 'F' wala matrai dekhaune, 'M' wala hide garne
        rescueMessage.SetActive(true);
        startAmbulanceMessage.SetActive(false);
    }

    void Update()
    {
        // STEP 1: Yedi 'F' thichyo vane
        if (rescueMessage.activeSelf && Input.GetKeyDown(KeyCode.F))
        {
            rescueMessage.SetActive(false); // F wala hide भयो
            startAmbulanceMessage.SetActive(true); // Aba M wala pop-up भयो
            Debug.Log("Patient Rescued! Now start the ambulance.");
        }

        // STEP 2: Yedi 'M' thichyo vane (jaba tyo active hunchha)
        else if (startAmbulanceMessage.activeSelf && Input.GetKeyDown(KeyCode.M))
        {
            startAmbulanceMessage.SetActive(false); // M wala pani hide भयो
            Debug.Log("Ambulance Started!");
            
            // Yaha timro ambulance start garne logic halna sakchau
        }
    }
}