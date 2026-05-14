using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Attach this to a barrier/obstacle GameObject.
/// Assign the info-cube child GameObjects in the Inspector via 'Info Boards'.
/// When the ambulance comes within 'proximityDistance' metres, the cubes become visible.
/// </summary>
public class InformationProximity : MonoBehaviour
{
    [Header("Info Display")]
    [Tooltip("Drag the info-cube (information board) GameObjects here in the Inspector")]
    [SerializeField] private List<GameObject> infoBoards = new List<GameObject>();

    [Header("Proximity Settings")]
    [Tooltip("Distance in metres at which the info boards pop up (increase this to show popup earlier)")]
    [SerializeField] private float proximityDistance = 8f;
    
    [Tooltip("If true, the info boards show up automatically when the ambulance is nearby. Set to FALSE if you only want to use the Empty Object Trigger Zone.")]
    [SerializeField] private bool useProximity = true;

    // Reference to ambulance transform for distance checks
    private Transform ambulanceTarget;
    private bool lastShowState = false;

    void Awake()
    {
        // Start with all boards hidden
        SetBoardsActive(false);
    }

    void Start()
    {
        RefreshAmbulanceTarget();
    }

    void Update()
    {
        // Lazily find ambulance if not set yet
        if (ambulanceTarget == null)
        {
            RefreshAmbulanceTarget();
            return;
        }

        float dist = Vector3.Distance(ambulanceTarget.position, transform.position);
        bool shouldShow = useProximity && (dist <= proximityDistance);

        // Only call SetBoardsActive when state changes (avoids spam)
        if (shouldShow != lastShowState)
        {
            lastShowState = shouldShow;
            SetBoardsActive(shouldShow);
        }
    }

    /// <summary>
    /// Try to find the ambulance by tag first, then fall back to finding
    /// the MissionManager's NavMeshAgent (in case tag wasn't set yet).
    /// </summary>
    private void RefreshAmbulanceTarget()
    {
        MissionManager mm = FindFirstObjectByType<MissionManager>();
        if (mm == null) return;

        // Prefer the MissionManager's own transform (it's on the ambulance)
        ambulanceTarget = mm.transform;

        // If the ambulance agent is a separate object, use that instead
        if (mm.ambulanceAgent != null)
        {
            ambulanceTarget = mm.ambulanceAgent.transform;
        }
    }

    // Trigger-based fallback (fires when ambulance physically enters collider)
    private void OnTriggerEnter(Collider other)
    {
        // Detect ambulance via MissionManager component instead of tag
        MissionManager manager = other.GetComponent<MissionManager>();
        if (manager == null) manager = other.GetComponentInParent<MissionManager>();

        if (manager != null)
        {
            SetBoardsActive(true);
            lastShowState = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MissionManager manager = other.GetComponent<MissionManager>();
        if (manager == null) manager = other.GetComponentInParent<MissionManager>();

        if (manager != null)
        {
            SetBoardsActive(false);
            lastShowState = false;
        }
    }

    private void SetBoardsActive(bool state)
    {
        foreach (GameObject board in infoBoards)
        {
            if (board != null)
                board.SetActive(state);
        }
    }
}
