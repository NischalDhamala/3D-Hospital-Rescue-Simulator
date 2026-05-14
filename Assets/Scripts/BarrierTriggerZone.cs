using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this script to an EMPTY GAMEOBJECT placed in front of a barrier.
/// Add a Box Collider to that GameObject and enable "Is Trigger".
/// Drag the info-board/cube GameObjects into 'Info Boards' in the Inspector.
/// When the ambulance enters this zone, the info boards appear.
/// When the ambulance exits, they disappear.
/// </summary>
public class BarrierTriggerZone : MonoBehaviour
{
    [Header("Info Boards to Show/Hide")]
    [Tooltip("Drag the info-cube or UI panel GameObjects here")]
    [SerializeField] private List<GameObject> infoBoards = new List<GameObject>();



    void Awake()
    {
        // Ensure this GameObject has a trigger collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            // Auto-add a BoxCollider if none exists
            BoxCollider box = gameObject.AddComponent<BoxCollider>();
            box.isTrigger = true;
            Debug.LogWarning($"[BarrierTriggerZone] No Collider found on '{gameObject.name}'. " +
                             "Auto-added a BoxCollider. Please resize it in the Inspector.");
        }
        else if (!col.isTrigger)
        {
            col.isTrigger = true;
            Debug.LogWarning($"[BarrierTriggerZone] Collider on '{gameObject.name}' was not a trigger. " +
                             "Auto-enabled 'Is Trigger'.");
        }

        // Hide all boards at start
        SetBoardsActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Detect ambulance via MissionManager component instead of tag
        MissionManager manager = other.GetComponent<MissionManager>();
        if (manager == null) manager = other.GetComponentInParent<MissionManager>();

        if (manager == null) return;

        SetBoardsActive(true);



        Debug.Log($"[BarrierTriggerZone] Ambulance entered zone: {gameObject.name}");
    }

    private void OnTriggerExit(Collider other)
    {
        MissionManager manager = other.GetComponent<MissionManager>();
        if (manager == null) manager = other.GetComponentInParent<MissionManager>();

        if (manager != null)
        {
            SetBoardsActive(false);
            Debug.Log($"[BarrierTriggerZone] Ambulance exited zone: {gameObject.name}");
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
