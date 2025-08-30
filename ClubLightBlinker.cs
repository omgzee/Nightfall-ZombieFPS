using UnityEngine;

public class ClubLightBlinker : MonoBehaviour
{
    [SerializeField] private Light[] lights;        // Assign your 6 spotlights in the Inspector
    [SerializeField] private float minInterval = 0.1f;
    [SerializeField] private float maxInterval = 0.5f;
    [SerializeField] private bool randomBlink = true;

    private float timer;

    void Start()
    {
        timer = Random.Range(minInterval, maxInterval);
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            BlinkLights();
            timer = Random.Range(minInterval, maxInterval);
        }
    }

    void BlinkLights()
    {
        foreach (Light light in lights)
        {
            if (randomBlink)
            {
                light.enabled = Random.value > 0.5f; // Random on/off
            }
            else
            {
                light.enabled = !light.enabled; // Toggle
            }
        }
    }
}
