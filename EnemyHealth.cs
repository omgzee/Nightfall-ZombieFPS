using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private float hitPoints = 100f;

    [Header("Drop Prefabs")]
    [SerializeField] private GameObject[] ammoDropPrefabs;
    [SerializeField] private GameObject healthDropPrefab;
    [SerializeField][Range(0f, 1f)] private float dropChance = 0.3f;

    [Header("Audio")]
    [SerializeField] private AudioClip[] deathSounds;
    [SerializeField] private AudioSource audioSource;

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private ObjectiveManager objectiveManager;
    public bool isDead = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        objectiveManager = FindObjectOfType<ObjectiveManager>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.spatialBlend = 1f;
                audioSource.playOnAwake = false;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        if (hitPoints <= 0f) return;

        IEnemyAI enemyAI = GetComponent<IEnemyAI>();
        enemyAI?.OnDamageTaken();

        hitPoints -= damage;

        if (animator != null)
        {
            int randomHit = Random.Range(0, 4);
            animator.SetFloat("Blend", randomHit);
            animator.SetTrigger("GetHit");
        }

        if (hitPoints <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        PlayDeathSound();

        animator?.SetTrigger("isDead");

        if (navMeshAgent != null)
            navMeshAgent.enabled = false;

        objectiveManager?.AddKill();

        TryDropPickup();

        Destroy(gameObject, 2.5f);  // keep your fixed delay here
    }

    private void PlayDeathSound()
    {
        if (deathSounds.Length > 0 && audioSource != null)
        {
            AudioClip clip = deathSounds[Random.Range(0, deathSounds.Length)];
            if (clip != null)
            {
                audioSource.pitch = Random.Range(0.95f, 1.05f);
                audioSource.PlayOneShot(clip);
                Debug.Log("Playing death sound: " + clip.name);
            }
            else
            {
                Debug.LogWarning("Selected death sound clip is null!");
            }
        }
        else
        {
            Debug.LogWarning("No death sounds assigned or AudioSource missing!");
        }
    }

    private void TryDropPickup()
    {
        if (Random.value <= dropChance)
        {
            bool dropAmmo = Random.value < 0.5f;

            GameObject drop = null;

            if (dropAmmo && ammoDropPrefabs.Length > 0)
            {
                int index = Random.Range(0, ammoDropPrefabs.Length);
                drop = ammoDropPrefabs[index];
            }
            else
            {
                drop = healthDropPrefab;
            }

            if (drop != null)
            {
                Vector3 spawnPos = transform.position + Vector3.up;
                Instantiate(drop, spawnPos, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Drop prefab is missing on " + gameObject.name);
            }
        }
    }
}
