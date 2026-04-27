using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class MissionManager : MonoBehaviour
{
    [Header("References")]
    public GameObject patient;
    public ParticleSystem bloodEffect;
    public NavMeshAgent ambulanceAgent;
    public Transform patientInsidePosition; 

    [Header("Pathfinding")]
    public List<Transform> waypoints; 
    private int currentWaypointIndex = 0;

    private bool isPatientFallen = false;
    private bool isLoaded = false;

    void Start()
    {
        if (bloodEffect != null) bloodEffect.Play();
        
        // Ensure agent is stopped at the very start
        if (ambulanceAgent != null) 
            ambulanceAgent.isStopped = true;
    }

    void Update()
    {
        // 1. Handle Patient Animation
        HandlePatientFalling();

        // 2. Press F to start
        if (Input.GetKeyDown(KeyCode.F) && !isLoaded)
        {
            LoadAndDrive();
        }
    }

    void HandlePatientFalling()
    {
        if (patient != null && !isPatientFallen)
        {
            // Gradually tilt patient 90 degrees
            Quaternion targetRotation = Quaternion.Euler(90, patient.transform.eulerAngles.y, 0);
            patient.transform.rotation = Quaternion.Slerp(patient.transform.rotation, targetRotation, Time.deltaTime * 0.5f);
            
            if (Quaternion.Angle(patient.transform.rotation, targetRotation) < 5f) 
                isPatientFallen = true;
        }
    }

    void LoadAndDrive()
    {
        isLoaded = true;

        if (bloodEffect != null) bloodEffect.Stop();

        // Put patient inside ambulance
        if (patient != null && patientInsidePosition != null)
        {
            patient.transform.position = patientInsidePosition.position;
            patient.transform.rotation = patientInsidePosition.rotation;
            patient.transform.SetParent(this.transform);
        }

        // Start driving to the first waypoint (The Barrier)
        if (ambulanceAgent != null && waypoints.Count > 0)
        {
            ambulanceAgent.isStopped = false;
            ambulanceAgent.SetDestination(waypoints[0].position);
            Debug.Log("Driving to Obstacle...");
        }
    }

    // This is called by the InformationProximity script automatically
    public void RedirectAmbulance()
    {
        if (waypoints.Count >= 2)
        {
            currentWaypointIndex = 1; // Index 1 is the Hospital
            ambulanceAgent.SetDestination(waypoints[1].position);
            Debug.Log("Path blocked! Heading to Hospital: " + waypoints[1].name);
        }
    }
}