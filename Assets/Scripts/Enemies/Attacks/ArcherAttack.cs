using UnityEngine;
using System.Collections;

public class ArcherAttack : EnemyAttack
{
    public GameObject arrowPrefab;
    private Animator animator;
    public Transform arrowStartPoint; //where the arrow will spawn from, from the GameObject inside the enemy
    private bool canAttack = true;
    public float attackCoolDown = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private IEnumerator AttackCoolDown()
	{
		canAttack = false;
		yield return new WaitForSeconds(attackCoolDown);
		canAttack = true;
	}

	public override void Attack(GameObject player)
    {
        if(!canAttack) return;

        animator.SetTrigger("Attack");

        if(arrowPrefab != null && arrowStartPoint != null) //spawn the arrow
        {
            Vector3 directionToPlayer = (player.transform.position - arrowStartPoint.position).normalized;
            GameObject arrowObj = Instantiate(arrowPrefab, arrowStartPoint.position, Quaternion.LookRotation(directionToPlayer));

            Arrow arrowScript = arrowObj.GetComponent<Arrow>();
            if(arrowScript != null)
            {
                arrowScript.damage = 5f;
            }
        }
        else
        {
            Debug.Log("PREFAB ARROW IS NULL");
        }

        StartCoroutine(AttackCoolDown());
    }
}
