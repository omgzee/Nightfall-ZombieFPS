using UnityEngine;
using UnityEngine.AI;

public class AnnaZombieHealth : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private float hitPoints = 100f;

    [Header("Drop Prefabs")]
    [SerializeField] private GameObject ammoDropPrefab;
    [SerializeField] private GameObject healthDropPrefab;
    [SerializeField][Range(0f, 1f)] private float dropChance = 0.3f;

    [Header("Audio")]
    public AudioSource deathSound;

    private Animator animator;
    public bool isDead = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        // Alert AI
        EnemyAI zombieAI = GetComponent<EnemyAI>();
        if (zombieAI != null)
        {
            zombieAI.OnDamageTaken();
        }

        hitPoints -= damage;

        // Play hit animation
        int randomHit = Random.Range(0, 4);
        animator.SetFloat("Blend", randomHit);
        animator.SetTrigger("GetHit");

        if (hitPoints <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        if (deathSound != null)
            deathSound.Play();

        animator.SetTrigger("isDead");

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
            agent.enabled = false;

        TryDropPickup();

        Destroy(gameObject, 2f);
    }

    private void TryDropPickup()
    {
        if (Random.value <= dropChance)
        {
            bool dropAmmo = Random.value < 0.5f;
            GameObject drop = dropAmmo ? ammoDropPrefab : healthDropPrefab;

            if (drop != null)
            {
                Vector3 spawnPos = transform.position + Vector3.up;
                Instantiate(drop, spawnPos, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning($"Drop prefab is missing: {(dropAmmo ? "Ammo" : "Health")} on {gameObject.name}");
            }
        }
    }
}
