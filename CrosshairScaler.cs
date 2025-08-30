using UnityEngine;
using UnityEngine.UI;

public class CrosshairScaler : MonoBehaviour
{
    [Header("Crosshair Scaling")]
    public float idleScale = 1f;
    public float moveScale = 1.3f;
    public float fireScale = 1.6f;
    public float aimScale = 0.8f;
    public float scaleSpeed = 8f;

    [Header("Transparency Settings")]
    public float transparencyWhileMovingAndShooting = 0.5f;  // Lower value for transparency when moving and firing/aiming
    public float transparencyWhileIdle = 1f;     // Full opacity when idle
    public float transparencyWhileAiming = 1f;   // Full opacity when aiming (you can adjust this if needed)

    private RectTransform crosshair;
    private Image crosshairImage;    // For transparency control
    private float targetScale;
    private float currentScale;

    public bool isMoving = false;
    public bool isFiring = false;
    public bool isAiming = false;

    private void Awake()
    {
        crosshair = GetComponent<RectTransform>();
        crosshairImage = GetComponent<Image>(); // Get the Image component for transparency control
        currentScale = idleScale;
        targetScale = idleScale;
    }

    private void Update()
    {
        // Determine target scale based on the player's actions
        if (isAiming)
            targetScale = aimScale;             // Highest priority
        else if (isFiring)
            targetScale = fireScale;
        else if (isMoving)
            targetScale = moveScale;
        else
            targetScale = idleScale;

        // Smoothly transition the crosshair scale
        currentScale = Mathf.Lerp(currentScale, targetScale, Time.deltaTime * scaleSpeed);
        crosshair.localScale = Vector3.one * currentScale;

        // Adjust transparency based on movement and firing/aiming states
        if (crosshairImage != null)
        {
            Color currentColor = crosshairImage.color;

            // If moving and firing/aiming, reduce transparency
            if (isMoving && (isFiring || isAiming))
                currentColor.a = transparencyWhileMovingAndShooting;
            // If idle or aiming only, keep full opacity
            else if (isAiming)
                currentColor.a = transparencyWhileAiming;
            else
                currentColor.a = transparencyWhileIdle;

            // Apply the new color with modified alpha
            crosshairImage.color = currentColor;
        }
    }
}
