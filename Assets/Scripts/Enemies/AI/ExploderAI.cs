using UnityEngine;
using UnityEngine.AI;

public class ExploderAI : MonoBehaviour
{
    private float sightRadius = 30f;      
    private float explosionRange = 6f;  

    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;

    private ExploderAttack exploderAttack;
    private bool hasExploded = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        exploderAttack = GetComponent<ExploderAttack>();

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (hasExploded) return;

        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= explosionRange) //explode once near the explosion range
        {
            agent.ResetPath();
            animator.SetBool("walk", false);
            animator.SetTrigger("attack01");

            exploderAttack.Attack(player.gameObject);

            hasExploded = true;
        }
        else if (distanceToPlayer <= sightRadius) //walk toward the player once in sight
        {
            agent.SetDestination(player.position);

            bool isMoving = agent.velocity.magnitude > 0.1f;
            animator.SetBool("walk", isMoving);
        }
        else //idle
        {
            agent.ResetPath();
            animator.SetBool("walk", false);
        }
    }
}
