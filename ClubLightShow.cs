using UnityEngine;

public class ClubLightShow : MonoBehaviour
{
    [SerializeField] private Light[] spotlights; // Assign your 6 spotlights in Inspector

    [Header("Pulse Settings")]
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float intensityMin = 0f;
    [SerializeField] private float intensityMax = 5f;

    [Header("Color Settings")]
    [SerializeField] private bool randomizeColors = true;
    [SerializeField] private float colorChangeInterval = 1f;

    private float[] pulseOffsets;
    private float[] colorTimers;

    void Start()
    {
        pulseOffsets = new float[spotlights.Length];
        colorTimers = new float[spotlights.Length];

        for (int i = 0; i < spotlights.Length; i++)
        {
            pulseOffsets[i] = Random.Range(0f, Mathf.PI * 2); // Offset pulses
            colorTimers[i] = Random.Range(0f, colorChangeInterval);
        }
    }

    void Update()
    {
        for (int i = 0; i < spotlights.Length; i++)
        {
            Light light = spotlights[i];

            // Smooth pulse
            float pulse = Mathf.Sin(Time.time * pulseSpeed + pulseOffsets[i]) * 0.5f + 0.5f;
            light.intensity = Mathf.Lerp(intensityMin, intensityMax, pulse);

            // Optional color change
            if (randomizeColors)
            {
                colorTimers[i] -= Time.deltaTime;
                if (colorTimers[i] <= 0f)
                {
                    light.color = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.9f, 1f);
                    colorTimers[i] = colorChangeInterval;
                }
            }
        }
    }
}
