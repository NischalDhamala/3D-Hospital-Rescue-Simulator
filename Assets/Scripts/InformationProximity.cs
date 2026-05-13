using System.Collections.Generic;
using UnityEngine;

public class InformationProximity : MonoBehaviour
{
    private List<GameObject> infoBoards = new List<GameObject>();
    // Reference to ambulance (target) for distance checks
    private Transform ambulanceTarget;

    void Awake()
    {
        // Find ambulance by tag if not set manually
        GameObject amb = GameObject.FindWithTag("Ambulance");
        if (amb != null) ambulanceTarget = amb.transform;
    }

    void Update()
    {
        // If we have a target, check distance and toggle boards accordingly
        if (ambulanceTarget != null)
        {
            float dist = Vector3.Distance(ambulanceTarget.position, transform.position);
            bool shouldShow = dist <= 5f;
            SetBoardsActive(shouldShow);
        }
    }

    // Keep OnTriggerEnter/Exit for compatibility (optional)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ambulance"))
        {
            SetBoardsActive(true);

            // Tell the Ambulance MissionManager to change route
            MissionManager manager = other.GetComponent<MissionManager>();
            if (manager == null)
                manager = other.GetComponentInParent<MissionManager>();
            if (manager != null)
            {
                manager.RedirectAmbulance();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ambulance"))
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