using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] float healAmount = 25f;
    [SerializeField] AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);

            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            Destroy(gameObject); // Remove pickup after use
        }
    }
}
