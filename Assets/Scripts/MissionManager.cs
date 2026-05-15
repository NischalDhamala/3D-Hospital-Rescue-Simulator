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

    public GameObject patient;
    public ParticleSystem bloodEffect;
    public NavMeshAgent ambulanceAgent;
    public Transform patientInsidePosition; 


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
        // Collect all Doctor-tagged objects (but do NOT detach or hide them yet)
        GameObject[] doctorObjs = GameObject.FindGameObjectsWithTag("Doctor");
        foreach (var d in doctorObjs)
        {
            doctors.Add(d);
        }

        if (bloodEffect != null) bloodEffect.Play();
        // Ensure agent is stopped at the very start
        if (ambulanceAgent != null)
            ambulanceAgent.isStopped = true;

        // Auto‑create UI Canvas & Texts if they are not assigned
        SetupUIAndCamera();
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

        // 5. Brake with Space bar — works in BOTH AI and manual mode
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!manualMode && ambulanceAgent != null)
                ambulanceAgent.isStopped = true;
            // Manual mode brake is handled inside AmbulanceController via Space key check
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (!manualMode && ambulanceAgent != null && missionRunning)
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

        // --- Put patient inside ambulance (hidden) ---
        if (patient != null && patientInsidePosition != null)
        {
            patient.transform.SetParent(this.transform);
            patient.transform.position = patientInsidePosition.position;
            patient.transform.rotation = patientInsidePosition.rotation;
            // Hide patient renderers (patient is lying inside)
            SetRenderersEnabled(patient, false);
        }

        // --- Hide the player avatar completely (player is "inside" the ambulance) ---
        if (player != null)
        {
            // Parent player to ambulance so it moves together
            player.transform.SetParent(this.transform);
            player.transform.localPosition = Vector3.zero;
            player.transform.localRotation = Quaternion.identity;

            // Deactivate the entire player — this stops ALL scripts, renderers,
            // animators, and the CharacterController in one go, preventing
            // "CharacterController.Move called on inactive controller" errors.
            player.SetActive(false);
        }

        // --- Hide doctors in place (they do NOT follow the ambulance) ---
        for (int i = 0; i < doctors.Count; i++)
        {
            var doc = doctors[i];
            if (doc == null) continue;

            // Disable NavMeshAgent so the doctor stops wandering
            var docAgent = doc.GetComponent<NavMeshAgent>();
            if (docAgent != null) docAgent.enabled = false;

            // Hide via DoctorVisibility component if present
            var dv = doc.GetComponent<DoctorVisibility>();
            if (dv != null)
            {
                dv.SetVisible(false);
            }
            else
            {
                // Fallback: disable all renderers
                SetRenderersEnabled(doc, false);
            }
        }

        Debug.Log("Patient loaded. Ready for manual driving or AI toggle.");
    }

    /// <summary>
    /// Helper to enable/disable all renderers on a GameObject and its children.
    /// </summary>
    private void SetRenderersEnabled(GameObject go, bool enabled)
    {
        if (go == null) return;
        var renderers = go.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
            r.enabled = enabled;
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