using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BossController : MonoBehaviour
{
    public Transform target;
    public Animator animator;
    public float fieldOfView = 180f;
    public float sightRadius = 30f;
    public float attackRange = 3.0f;
    public float spawnAnimationRadius = 30f; // Distance at which spawn animation triggers

    private NavMeshAgent agent;
    private int isWalkingHash;
    private int isAttackHash;
    private int isDeadHash;
    private bool hasSpawned = false; // Track if spawn animation has played
    private bool isSpawning = false; // Track if currently in spawn animation
    private AIStatus bossStatus;
    private BossAttack attackType;
    private bool playerInRange = false;
    private Renderer[] renderers;
    private Collider[] colliders;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isAttackHash = Animator.StringToHash("isAttack");
        isDeadHash = Animator.StringToHash("isDead");

        bossStatus = GetComponent<AIStatus>();
        attackType = GetComponent<BossAttack>();

        
        renderers = GetComponentsInChildren<Renderer>(true);
        colliders = GetComponentsInChildren<Collider>(true);

        // Initially hide the boss
        SetBossVisibility(false);
        
        if (agent != null)
        {
            agent.enabled = false;
        }
        
        GameObject tmp = GameObject.FindWithTag("Player");
        if (tmp != null)
        {
            target = tmp.transform;
        }
        
        animator.enabled = false;
    }

    void Update()
    {
        if (target == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        // If boss is dead, handle death state
        if (hasSpawned && bossStatus != null && !bossStatus.isAlive())
        {
            animator.SetBool(isDeadHash, true);
            return;
        }

        // Check if player is within spawn radius and spawn animation hasn't played yet
        if (!hasSpawned && !isSpawning && distanceToPlayer <= spawnAnimationRadius)
        {
            TriggerSpawnAnimation();
            return;
        }

        
        if (hasSpawned && !isSpawning)
        {
            playerInRange = IsPlayerInRange();

            if (playerInRange)
            {
                if (distanceToPlayer > attackRange)
                {
                    agent.SetDestination(target.position);
                    bool isMoving = agent.velocity.magnitude > 0.1f;
                    animator.SetBool(isWalkingHash, isMoving);
                    animator.SetBool(isAttackHash, false);
                }
                else
                {
                    agent.ResetPath();
                    animator.SetBool(isWalkingHash, false);

                    transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));

                    if (attackType != null)
                    {
                        attackType.Attack(target.gameObject);
                    }
                }
            }
            else
            {
                agent.ResetPath();
                animator.SetBool(isWalkingHash, false);
                animator.SetBool(isAttackHash, false);
            }
        }
    }

    private void SetBossVisibility(bool visible)
    {
        foreach (Renderer rend in renderers)
        {
            rend.enabled = visible;
        }
        
        foreach (Collider col in colliders)
        {
            col.enabled = visible;
        }
    }

    private void TriggerSpawnAnimation()
    {
        isSpawning = true;

        // Make the boss visible
        SetBossVisibility(true);

        animator.enabled = true;
        animator.Play("Demon_Come-out1", 0, 0f);
        StartCoroutine(EnableAIAfterSpawn());
        Debug.Log("Boss spawn animation triggered by player proximity");
    }

    private IEnumerator EnableAIAfterSpawn()
    {
        yield return null;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        while (!stateInfo.IsName("Demon_Come-out1"))
        {
            yield return null;
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        }

        
        float animationLength = stateInfo.length;

        yield return new WaitForSeconds(animationLength * 0.95f);

        hasSpawned = true;
        isSpawning = false;
        if (agent != null && !agent.enabled)
        {
            agent.enabled = true;
        }
        if (target != null)
        {
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
        }
    }

    public void OnSpawnAnimationComplete()
    {
        hasSpawned = true;
        isSpawning = false;
        if (agent != null && !agent.enabled)
        {
            agent.enabled = true;
        }
    }

    public bool IsPlayerInRange()
    {
        float enemyPlayerDist = Vector3.Distance(transform.position, target.position);
        if (enemyPlayerDist > sightRadius)
        {
            return false;
        }
        Vector3 directionToPlayer = target.position - transform.position;
        directionToPlayer.Normalize();
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > fieldOfView * 0.5f)
        {
            return false;
        }
        return true;
    }

    
    public void OnAttackAnimationComplete()
    {
        if (attackType != null)
        {
            attackType.ResetAttackState();
        }
        animator.SetBool(isAttackHash, false);
    }
}