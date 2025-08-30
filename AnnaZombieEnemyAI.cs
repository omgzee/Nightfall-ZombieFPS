using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnnaZombieEnemyAI : MonoBehaviour, IEnemyAI
{
    Transform target;
    [SerializeField] float chaseRange = 5f;
    [SerializeField] float turnSpeed = 5f;
    [SerializeField] float runRange = 2f;
    [SerializeField] int totalAttackAnimations = 1;

    NavMeshAgent navMeshAgent;
    Animator animator;
    EnemyHealth enemyHealth;

    float distanceTarget = Mathf.Infinity;
    bool isProvoked = false;
    bool hasScreamed = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyHealth = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogWarning("Player not found! Make sure the player GameObject is tagged 'Player'.");
            }
        }

        navMeshAgent.updateRotation = false;
    }

    void Update()
    {
        if (enemyHealth.isDead)
        {
            enabled = false;
            navMeshAgent.enabled = false;
            return;
        }

        distanceTarget = Vector3.Distance(transform.position, target.position);

        if (isProvoked)
        {
            EngageTarget();
        }
        else if (distanceTarget <= chaseRange)
        {
            isProvoked = true;
        }
    }

    private void EngageTarget()
    {
        FaceTarget();

        if (distanceTarget >= navMeshAgent.stoppingDistance)
        {
            ChaseTarget();
        }
        else
        {
            AttackTarget();
        }
    }

    private void ChaseTarget()
    {
        if (!navMeshAgent.enabled) return;

        if (Vector3.Distance(navMeshAgent.destination, target.position) > 0.5f)
        {
            navMeshAgent.SetDestination(target.position);
        }

        if (!hasScreamed)
        {
            animator.SetTrigger("Scream");
            hasScreamed = true;

            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);

            Debug.Log("Enemy is SCREAMING");

            StartCoroutine(DelayWalking(2.8f)); // Adjust based on scream animation length
        }
        else if (distanceTarget <= runRange)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", true);
            animator.SetBool("isAttacking", false);
            Debug.Log("Enemy is RUNNING");
        }
        else
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);
            Debug.Log("Enemy is WALKING");
        }
    }

    private IEnumerator DelayWalking(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool("isWalking", true);
    }

    private void AttackTarget()
    {
        if (!animator.GetBool("isAttacking"))
        {
            float randomBlend = Random.Range(0, totalAttackAnimations);
            animator.SetFloat("Blend", randomBlend);
            animator.SetBool("isAttacking", true);
            Debug.Log("Enemy is ATTACKING with blend value: " + randomBlend);
        }

        navMeshAgent.ResetPath();
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
    }

    private void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    public void ResetAttack()
    {
        animator.SetBool("isAttacking", false);
    }

    public void OnDamageTaken()
    {
        isProvoked = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
