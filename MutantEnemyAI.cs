using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MutantEnemyAI : MonoBehaviour, IEnemyAI
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
        else if (distanceTarget <= chaseRange|| CanSeePlayer()) // ✅ USE VISION CHECK
        {
            isProvoked = true;
        }
    }

    bool CanSeePlayer()
    {
        Vector3 dirToPlayer = (target.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToPlayer);

        if (angle < 180f) 
        {
            Ray ray = new Ray(transform.position + Vector3.up, dirToPlayer);
            if (Physics.Raycast(ray, out RaycastHit hit, chaseRange))
            {
                if (hit.transform.CompareTag("Player"))
                    return true;
            }
        }
        return false;
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

        // Check if path is valid before setting destination
        NavMeshPath path = new NavMeshPath();
        if (navMeshAgent.CalculatePath(target.position, path) && path.status == NavMeshPathStatus.PathComplete)
        {
            navMeshAgent.SetDestination(target.position);

            if (distanceTarget <= runRange)
            {
                navMeshAgent.speed = 4f;
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", true);
                animator.SetBool("isAttacking", false);
                Debug.Log("Enemy is RUNNING");
            }
            else
            {
                navMeshAgent.speed = 2f;
                animator.SetBool("isWalking", true);
                animator.SetBool("isRunning", false);
                animator.SetBool("isAttacking", false);
                Debug.Log("Enemy is WALKING");
            }
        }
        else
        {
            // ❌ No valid path - stop moving and maybe idle or look angry
            navMeshAgent.ResetPath();
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isAttacking", false);
            Debug.Log("Enemy can't reach target - path blocked");
        }
    }


    private void AttackTarget()
    {
        if (!animator.GetBool("isAttacking"))
        {
            // Choose a random attack animation using Blend values: e.g., 0 = attack2RLSpike, 1 = attack4
            float randomBlend = Random.Range(0, totalAttackAnimations); // totalAttackAnimations should match # of clips in the blend tree
            animator.SetFloat("Blend", randomBlend);
            animator.SetBool("isAttacking", true);

            Debug.Log("Enemy is ATTACKING with blend value: " + randomBlend);
        }

        navMeshAgent.ResetPath(); // Stop moving
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
    }



    private void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
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
