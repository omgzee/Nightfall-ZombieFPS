using UnityEngine;

public class SimpleChopperLanding : MonoBehaviour
{
    public Transform landingSpot;
    public float landingDuration = 20f;

    private Vector3 startPosition;
    private float elapsedTime = 0f;
    private bool isLanding = false;

    private Animator animator;

    private void OnEnable()
    {
        startPosition = transform.position;
        elapsedTime = 0f;
        isLanding = true;

        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("Landed", false);  // Reset to landing animation
        }
    }

    void Update()
    {
        if (!isLanding) return;

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / landingDuration);

        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(startPosition.y, landingSpot.position.y, t);
        transform.position = pos;

        if (t >= 1f)
        {
            isLanding = false;

            if (animator != null)
            {
                animator.SetBool("Landed", true);  // Switch to idle state
            }
        }
    }
}
