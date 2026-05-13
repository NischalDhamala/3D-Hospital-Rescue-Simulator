using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.UI;


public class MissionManager : MonoBehaviour
{
    [Header("References")]
    // Reference to the player avatar (e.g., FirstPersonController) that should stay still when ambulance moves
    public GameObject player;
    // Reference to the ambulance (the NavMeshAgent is already assigned as ambulanceAgent)
    // public NavMeshAgent ambulanceAgent; // already exists
    public GameObject patient;
    public ParticleSystem bloodEffect;
    public NavMeshAgent ambulanceAgent;
    public Transform patientInsidePosition; 

    [Header("Pathfinding")]
    private int currentWaypointIndex = 0;
    public List<Transform> waypoints;
    // List to keep track of all Doctor NPCs so we can hide them when the ambulance starts
    private List<GameObject> doctors = new List<GameObject>();

    // Mission timer (90 seconds)
    private float missionTimer = 90f;
    private bool missionRunning = true;

    // UI Elements
    [Header("UI")]
    public UnityEngine.UI.Text timerText;
    public UnityEngine.UI.Text statusText;

    // Control mode
    private bool manualMode = false;
    private AmbulanceController manualController;

    private bool isPatientFallen = false;
    private bool isLoaded = false;

    void Start()
    {
        // Detach any Doctor objects from the ambulance hierarchy so they don't move together
        GameObject[] doctorObjs = GameObject.FindGameObjectsWithTag("Doctor");
        foreach (var d in doctorObjs)
        {
            d.transform.parent = null; // ensure they are not children of the ambulance
            doctors.Add(d);
        }

        if (bloodEffect != null) bloodEffect.Play();
        // Ensure agent is stopped at the very start
        if (ambulanceAgent != null)
            ambulanceAgent.isStopped = true;

        // Auto‑create UI Canvas & Texts if they are not assigned
        SetupUIAndCamera();
        if (ambulanceAgent != null) ambulanceAgent.gameObject.tag = "Ambulance";
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

        // 3. Toggle manual/AI mode with M key
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleControlMode();
        }

        // 4. Mission timer
        if (missionRunning)
        {
            missionTimer -= Time.deltaTime;
            if (timerText != null)
                timerText.text = $"Time: {Mathf.CeilToInt(missionTimer)}";

            if (missionTimer <= 0f)
            {
                missionRunning = false;
                OnMissionFailed();
            }
        }

        // 5. Brake with Space bar (only in AI mode)
        if (!manualMode && Input.GetKeyDown(KeyCode.Space))
        {
            if (ambulanceAgent != null)
                ambulanceAgent.isStopped = true;
        }
        if (!manualMode && Input.GetKeyUp(KeyCode.Space))
        {
            if (ambulanceAgent != null && missionRunning)
                ambulanceAgent.isStopped = false;
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

        // Disable the player avatar movement so only the ambulance moves
        if (player != null)
        {
            // Try to disable common controller components (StarterAssets, CharacterController, etc.)
            var fps = player.GetComponent<StarterAssets.FirstPersonController>();
            if (fps != null) fps.enabled = false;
            var cc = player.GetComponent<UnityEngine.CharacterController>();
            if (cc != null) cc.enabled = false;
            // Optionally hide the player model
            var renderers = player.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers) r.enabled = false;
        }

        // 1. Move player into ambulance (hide avatar) and hide patient/doctor renderers
        if (player != null)
        {
            // Move player inside the ambulance at the same seat as patient
            player.transform.position = patientInsidePosition.position;
            player.transform.rotation = patientInsidePosition.rotation;
            player.transform.SetParent(this.transform);
            // Hide player mesh renderers
            var pRends = player.GetComponentsInChildren<Renderer>();
            foreach (var r in pRends) r.enabled = false;
        }

        // Hide patient renderers (patient is now inside ambulance)
        if (patient != null)
        {
            var pRends = patient.GetComponentsInChildren<Renderer>();
            foreach (var r in pRends) r.enabled = false;
        }

        // Hide all doctors when ambulance starts moving
        foreach (var doc in doctors)
        {
            var dRends = doc.GetComponentsInChildren<Renderer>();
            foreach (var r in dRends) r.enabled = false;
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
    
    // Toggle between AI NavMesh and manual control
    private void ToggleControlMode()
    {
        manualMode = !manualMode;
        if (manualMode)
        {
            // Switch to manual
            if (ambulanceAgent != null)
                ambulanceAgent.enabled = false;
            if (manualController == null)
                manualController = gameObject.AddComponent<AmbulanceController>();
            manualController.enabled = true;
            if (statusText != null)
                statusText.text = "Manual Mode";
        }
        else
        {
            // Switch to AI
            if (manualController != null)
                manualController.enabled = false;
            if (ambulanceAgent != null)
                ambulanceAgent.enabled = true;
            if (statusText != null)
                statusText.text = "AI Mode";
        }
    }

    // ------------------------------------------------------------
    // Auto‑setup UI Canvas, Text components and Camera follow
    // ------------------------------------------------------------
    private void SetupUIAndCamera()
    {
        // ----- UI Canvas -----
        if (timerText == null || statusText == null)
        {
            // Create Canvas
            GameObject canvasGO = new GameObject("MissionCanvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            // Timer Text (top‑left)
            GameObject timerGO = new GameObject("TimerText");
            timerGO.transform.SetParent(canvasGO.transform);
            Text timer = timerGO.AddComponent<Text>();
            timer.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            timer.fontSize = 48;
            timer.alignment = TextAnchor.UpperLeft;
            timer.color = Color.white;
            RectTransform rtTimer = timerGO.GetComponent<RectTransform>();
            rtTimer.anchorMin = new Vector2(0, 1);
            rtTimer.anchorMax = new Vector2(0, 1);
            rtTimer.pivot = new Vector2(0, 1);
            rtTimer.anchoredPosition = new Vector2(20, -20);

            // Status Text (top‑center)
            GameObject statusGO = new GameObject("StatusText");
            statusGO.transform.SetParent(canvasGO.transform);
            Text status = statusGO.AddComponent<Text>();
            status.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            status.fontSize = 36;
            status.alignment = TextAnchor.UpperCenter;
            status.color = Color.yellow;
            RectTransform rtStatus = statusGO.GetComponent<RectTransform>();
            rtStatus.anchorMin = new Vector2(0.5f, 1);
            rtStatus.anchorMax = new Vector2(0.5f, 1);
            rtStatus.pivot = new Vector2(0.5f, 1);
            rtStatus.anchoredPosition = new Vector2(0, -20);

            // Assign to fields
            timerText = timer;
            statusText = status;
        }

        // ----- Camera Follow -----
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            CameraFollow cf = mainCam.GetComponent<CameraFollow>();
            if (cf == null)
                cf = mainCam.gameObject.AddComponent<CameraFollow>();
            cf.target = this.transform; // follow the ambulance (this GameObject)
        }
    }
    
    // Called by MissionGoal when ambulance reaches the goal
    public void OnMissionSuccess()
    {
        missionRunning = false;
        if (statusText != null)
            statusText.text = "Mission Successful!";
        if (ambulanceAgent != null)
            ambulanceAgent.isStopped = true;
        Debug.Log("Mission Successful!");
    }

    private void OnMissionFailed()
    {
        missionRunning = false;
        if (statusText != null)
            statusText.text = "Mission Failed!";
        if (ambulanceAgent != null)
            ambulanceAgent.isStopped = true;
        Debug.Log("Mission Failed!");
    }
}