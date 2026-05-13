using UnityEngine;

/// <summary>
/// Hides the doctor (NPC) renderers when the player comes within a certain distance.
/// The player avatar must be tagged "Player". Attach this script to any GameObject
/// that represents a doctor (or any NPC you want to hide when the player approaches).
/// </summary>
public class DoctorVisibility : MonoBehaviour
{
    // Distance at which the doctor starts to hide (in Unity units).
    [Header("Settings")]
    public float hideDistance = 5f;

    // Cached reference to the player GameObject.
    private Transform playerTransform;

    // All renderer components of this doctor (including children).
    private Renderer[] renderers;

    private void Awake()
    {
        // Find the player by tag once at start.
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
        else
            Debug.LogWarning("DoctorVisibility: No GameObject with tag 'Player' found in scene.");

        // Cache renderers to enable/disable quickly.
        renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool shouldHide = distance <= hideDistance;

        foreach (var r in renderers)
        {
            if (r.enabled != !shouldHide)
                r.enabled = !shouldHide;
        }
    }
}
