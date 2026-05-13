using UnityEngine;

/// <summary>
/// Controls doctor NPC visibility and attachment to the ambulance.
/// Doctors stay visible and ride along with the ambulance once the mission starts.
/// When not riding, doctors are shown normally in the scene.
/// Attach this script to any doctor NPC GameObject.
/// </summary>
public class DoctorVisibility : MonoBehaviour
{
    // Whether this doctor is currently riding in the ambulance.
    private bool isRiding = false;

    // Original parent so we can detach later if needed.
    private Transform originalParent;
    // Original local position & rotation (for restoring).
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private void Awake()
    {
        originalParent = transform.parent;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    /// <summary>
    /// Call this to make the doctor board the ambulance.
    /// The doctor stays VISIBLE and moves with the ambulance.
    /// </summary>
    public void BoardAmbulance(Transform ambulanceTransform, Vector3 seatOffset)
    {
        isRiding = true;
        transform.SetParent(ambulanceTransform);
        transform.localPosition = seatOffset;
        transform.localRotation = Quaternion.identity;

        // Ensure all renderers are enabled (doctor stays visible)
        SetRenderersEnabled(true);
    }

    /// <summary>
    /// Call this to make the doctor exit the ambulance (e.g., at hospital).
    /// </summary>
    public void ExitAmbulance()
    {
        isRiding = false;
        transform.SetParent(originalParent);
        SetRenderersEnabled(true);
    }

    /// <summary>
    /// Hide or show the doctor completely.
    /// </summary>
    public void SetVisible(bool visible)
    {
        SetRenderersEnabled(visible);
    }

    private void SetRenderersEnabled(bool enabled)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
        foreach (var r in renderers)
        {
            r.enabled = enabled;
        }
    }
}
