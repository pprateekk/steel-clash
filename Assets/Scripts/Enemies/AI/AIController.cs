using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
	public Transform target;
    public Animator animator;
    public float fieldOfView = 180f;
    public float sightRadius = 30f;

    private NavMeshAgent agent;
    private int isWalkingHash;
    private Vector3 lastPosition;

    private bool playerInRange = false;

    public float attackRange = 6.0f;
    private EnemyAttack attackType;
    private AIStatus enemyStatus;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        lastPosition = transform.position;
        attackType = GetComponent<EnemyAttack>();
        enemyStatus = GetComponent<AIStatus>();


        GameObject tmp = GameObject.FindWithTag("Player");
		if (tmp != null){
			target=tmp.transform;
		}
    }

    void Update()
    {
        if (enemyStatus != null && !enemyStatus.isAlive()) //if enemy is dead, return
        {
            return;
        }

        if (target == null)
        {
            return;
        }

        playerInRange = IsPlayerInRange();
        float enemyPlayerDist = Vector3.Distance(transform.position, target.position);

        if(attackType is ArcherAttack) //if the enemy is an archer, dont move and use the ArcherAttack
        {
            if(playerInRange)
            {
                transform.LookAt(target);
                attackType.Attack(target.gameObject);
            }
        }
        else //for all the other enemies
        {
            if(playerInRange && enemyPlayerDist > attackRange) //if the player in range but not within the attack radius
            {
                agent.SetDestination(target.position);
            }
            else if (playerInRange && enemyPlayerDist <= attackRange) //if player in range and within the attack rad, attack depending on what enemy it is
            {
                agent.ResetPath();
                transform.LookAt(target);

                if(attackType != null)
                {
                    attackType.Attack(target.gameObject);
                }
            }
            else //idle
            {
                agent.ResetPath();
            }
        }        

        bool isMoving = agent.velocity.magnitude > 0.1f;
        animator.SetBool(isWalkingHash, isMoving);
    }

    public bool IsPlayerInRange()
	{
		//check if the player is within in sight radius
        float enemyPlayerDist = Vector3.Distance(transform.position, target.position);
        if(enemyPlayerDist > sightRadius)
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
}

