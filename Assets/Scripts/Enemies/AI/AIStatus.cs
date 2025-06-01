using UnityEngine;
using System.Collections;

public enum EnemyType
{
	Default, 
	SirStabsALot, 
	ArcherMcStabby
}

public class AIStatus : MonoBehaviour {
	
	private float health = 50.0f;

	private float maxHealth = 50.0f;

	private bool dead = false;
	private PlayerStatus playerStatus;
    public Animator animator;

	private HealthBar healthBar;

	public EnemyType enemyType = EnemyType.Default;
	
	void Start()
	{
        animator = GetComponent<Animator>();
		healthBar = GetComponentInChildren<HealthBar>();
		healthBar.UpdateHealthBar(health, maxHealth);
	}
	
	public bool isAlive() {return !dead;}	

	public void ApplyDamage(float damage){

		if(dead) return;
		health -= damage;
		healthBar.UpdateHealthBar(health, maxHealth);
		Debug.Log($"Enemy took {damage} damage. Health now: {health}");
		if (health <= 0 )
		{
			StartCoroutine(Die());
		}
	}

	private IEnumerator Die()
    {
        dead = true;
        health = 0f;

        if (animator != null)
        {
            animator.SetBool("isDead", true);
        }
        Debug.Log("***********Enemy Dead!*************");

        yield return new WaitForSeconds(2.5f); //let the death anim play

		if(enemyType == EnemyType.SirStabsALot) //when SirStabsALot dies, increse player's health by 15
		{
			GameObject playerObj = GameObject.FindWithTag("Player");
			if(playerObj != null)
			{
				PlayerStatus playerStat = playerObj.GetComponent<PlayerStatus>();
				if (playerStat != null)
				{
        			Destroy(gameObject); // remove the enemy from the scene
					// playerStatus.AddHealth(15f);
					playerStat.StartCoroutine(playerStat.AddHealthAfterEnemyDeath(15f));
				}
			}
		}
		else if(enemyType == EnemyType.ArcherMcStabby) //if it's the archer that dies, player gets an attack boost which is 2x the orignal attack damage, for 60s
		{
			GameObject playerObj = GameObject.FindWithTag("Player");
			if(playerObj != null)
			{
				PlayerController playerController = playerObj.GetComponent<PlayerController>();
				if(playerController != null)
				{
			        Destroy(gameObject); // remove the enemy from the scene

					playerController.StartCoroutine(playerController.AttackBoostRoutine(2f, 60f));
				}
			}
		}
        Destroy(gameObject); // remove the enemy from the scene
    }

	public float ReturnHealth()
	{
		return health;
	}
}
