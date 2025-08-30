using UnityEngine;

public class CameraRecoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    public float recoilStrength = 2f;       // How strong the recoil is
    public float returnSpeed = 5f;          // How quickly it returns
    public float recoilDuration = 0.1f;     // How long the recoil lasts

    private Vector3 targetRotation;
    private Vector3 currentRotation;
    private float recoilTimer;

    void Update()
    {
        if (recoilTimer > 0)
        {
            recoilTimer -= Time.deltaTime;
        }
        else
        {
            targetRotation = Vector3.zero; // Return to center
        }

        currentRotation = Vector3.Slerp(currentRotation, targetRotation, Time.deltaTime * returnSpeed);
        transform.localEulerAngles = currentRotation;
    }

    // Call this method when shooting
    public void ApplyRecoil(float recoilX, float recoilY, float recoilZ)
    {
        recoilTimer = recoilDuration;

        // Apply recoil based on the passed values (can modify this further for more variation)
        targetRotation += new Vector3(recoilX, recoilY, recoilZ);
    }
}
