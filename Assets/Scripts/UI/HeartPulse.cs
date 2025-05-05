using UnityEngine;

public class UI_ScalePulse : MonoBehaviour
{
    [Header("Scale Settings")]
    [SerializeField] float pulseSpeed = 2f;         // Speed of pulsing
    [SerializeField] float scaleAmount = 0.1f;      // How much it scales
    [SerializeField] Vector3 baseScale = new Vector3(1f, 1f, 1f); // Original scale

    void Start()
    {
        // Ensure it starts at base scale
        transform.localScale = baseScale;
    }

    void Update()
    {
        float scaleOffset = Mathf.Sin(Time.time * pulseSpeed) * scaleAmount;
        transform.localScale = baseScale + Vector3.one * scaleOffset;
    }
}
