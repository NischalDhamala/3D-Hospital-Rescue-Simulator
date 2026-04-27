using System.Collections.Generic;
using UnityEngine;

public class InformationProximity : MonoBehaviour
{
    private List<GameObject> infoBoards = new List<GameObject>();

    void Start()
    {
        // Find all children tagged "Information" and hide them at the start
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Information"))
            {
                infoBoards.Add(child.gameObject);
                child.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering is the Ambulance (tagged Player)
        if (other.CompareTag("Player")) 
        {
            SetBoardsActive(true);

            // Tell the Ambulance MissionManager to change route
            MissionManager manager = other.GetComponent<MissionManager>();
            if (manager != null)
            {
                manager.RedirectAmbulance();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetBoardsActive(false);
        }
    }

    private void SetBoardsActive(bool state)
    {
        foreach (GameObject board in infoBoards)
        {
            board.SetActive(state);
        }
    }
}