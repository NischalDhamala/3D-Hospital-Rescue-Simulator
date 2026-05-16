using UnityEngine;

public class AmbulanceLightController : MonoBehaviour
{
    public MeshRenderer lampRenderer;
    public AudioSource sirenAudio; // New line: for Audio Source
    [ColorUsage(true, true)] public Color activeColor = Color.blue * 4.0f;
    public float blinkSpeed = 5f;

    private MaterialPropertyBlock _propBlock;
    public bool _isStarted = false; // Public so Inspector can check it

    void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
    }

    public void ToggleAmbulance(bool status)
    {
        _isStarted = status;
        if (!_isStarted)
        {
            UpdateEmission(Color.black);
            if (sirenAudio != null) sirenAudio.Stop(); // Stop siren
        }
        else
        {
            if (sirenAudio != null) sirenAudio.Play(); // Start siren
        }
    }

    void Update()
    {
        if (_isStarted)
        {
            float lerp = Mathf.PingPong(Time.time * blinkSpeed, 1);
            Color resultColor = Color.Lerp(Color.black, activeColor, lerp);
            UpdateEmission(resultColor);
        }
    }

    void UpdateEmission(Color color)
    {
        lampRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor("_EmissionColor", color);
        lampRenderer.SetPropertyBlock(_propBlock);
    }
}