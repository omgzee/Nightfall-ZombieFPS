using UnityEngine;

public class FloatBob : MonoBehaviour
{
    [SerializeField] float amplitude = 0.25f;  // How high it bounces
    [SerializeField] float frequency = 1f;     // How fast it bounces

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = startPos + new Vector3(0, y, 0);
    }
}
