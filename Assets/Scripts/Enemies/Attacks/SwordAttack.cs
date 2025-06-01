using UnityEngine;
using System.Collections;

public class SwordAttack : EnemyAttack
{
	private Animator animator;
	private bool canAttack = true;
	public float attackCoolDown = 1.0f;
	public float appliedDamage = 10f;

	private void Start()
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
		if (!canAttack) return;

		animator.SetTrigger("Attack");

		var playerStatus = player.GetComponent<PlayerStatus>();
		if (playerStatus != null)
		{
			float damage = appliedDamage;
			if(playerStatus.IsBlocking) //if player is blocking, it's half the damage
			{
				damage = damage * 0.5f;
			}
			playerStatus.ApplyDamage(damage);
		}

		StartCoroutine(AttackCoolDown());
	}
}
