using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public float damage = 10f;
    private PlayerHealth target;  // Reference to the player's health
    public AudioClip gettingHitSound;
    AudioSource audioSource;

    void Start()
    {
        // Automatically find the player GameObject in the scene
        target = Object.FindFirstObjectByType<PlayerHealth>();
        audioSource = GetComponent<AudioSource>();
    }

    // Called by the animation event
    public void AttackOnHit()
    {
        if (target != null)
        {
            target.TakeDamage(damage);
            Debug.Log("Zombie Attack on Hit");
            if (gettingHitSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(gettingHitSound);
            }
        }
    }
}
