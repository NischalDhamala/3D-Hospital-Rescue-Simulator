using UnityEngine;
using UnityEngine.AI;

public class MissionManager : MonoBehaviour
{
    [Header("References")]
    public GameObject patient;
    public ParticleSystem bloodEffect;
    public NavMeshAgent ambulanceAgent;
    public Transform hospitalDestination;
    public Transform patientInsidePosition; // एम्बुलेन्स भित्रको सिट

    private bool isPatientFallen = false;
    private bool isLoaded = false;

    void Start()
    {
        // सुरुमा रगत आउने बनाउने
        if (bloodEffect != null) bloodEffect.Play();
    }

    void Update()
    {
        // १. बिरामीलाई ढलाउन (बिस्तारै Rotation परिवर्तन गर्ने)
        if (!isPatientFallen)
        {
            // बिरामीलाई ९० डिग्री ढलाउने (बिस्तारै)
            Quaternion targetRotation = Quaternion.Euler(90, patient.transform.eulerAngles.y, 0);
            patient.transform.rotation = Quaternion.Slerp(patient.transform.rotation, targetRotation, Time.deltaTime * 0.5f);
            
            // यदि बिरामी लगभग ढलिसक्यो भने
            if (Quaternion.Angle(patient.transform.rotation, targetRotation) < 5f) 
                isPatientFallen = true;
        }

        // २. 'F' थिच्दा बिरामीलाई एम्बुलेन्समा हाल्ने र कुदाउने
        if (Input.GetKeyDown(KeyCode.F) && !isLoaded)
        {
            LoadAndDrive();
        }
    }

    void LoadAndDrive()
    {
        isLoaded = true;

        // रगत बन्द गर्ने
        if (bloodEffect != null) bloodEffect.Stop();

        // बिरामीलाई एम्बुलेन्स भित्र राख्ने
        patient.transform.position = patientInsidePosition.position;
        patient.transform.rotation = patientInsidePosition.rotation;
        patient.transform.SetParent(this.transform); // एम्बुलेन्ससँगै हिँड्ने बनाउन

        // एम्बुलेन्स कुदाउने
        if (ambulanceAgent != null && hospitalDestination != null)
        {
            ambulanceAgent.isStopped = false;
            ambulanceAgent.SetDestination(hospitalDestination.position);
            Debug.Log("Rescue Successful! Heading to Hospital.");
        }
    }
}