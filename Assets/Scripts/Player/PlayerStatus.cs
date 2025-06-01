using UnityEngine;
using System.Collections;

public class PlayerStatus : MonoBehaviour {
	
	private float health = 100.0f;
	private float maxHealth = 100.0f;
	private bool dead = false;

	private PlayerController playerController;
    public Animator animator;

	public bool IsBlocking {get; set;} //check if the player is blocking or not
	
	public void AddHealth(float moreHealth)
	{
		health += moreHealth;
		if(health > maxHealth) //dont exceed max health
		{
			health = maxHealth;
		}
	}
	
	public float GetHealth()
	{
		return health;
	}

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
    }

    public bool isAlive() {return !dead;}
	
	public void ApplyDamage(float damage)
	{
		health -= damage;
		if (health <= 0){
			health = 0;
			StartCoroutine(Die());
		}
	}

	private IEnumerator Die()
    {
        dead = true;

        if (animator != null)
        {
            animator.SetBool("isDead", true);
        }
		
        Debug.Log("***********Player Dead!*************");

        yield return new WaitForSeconds(2.5f); //let the death anim play

		GameManager gameManager = FindObjectOfType<GameManager>();
		if (gameManager != null)
		{
			gameManager.PlayerDied();
		}

		HideCharacter();
    }

	public IEnumerator AddHealthAfterEnemyDeath(float healthAdd)
	{
		AddHealth(healthAdd);
		yield return new WaitForSeconds(0.01f);
	}
	
	void HideCharacter()
	{	
		playerController.IsControllable = false;

		foreach (var ren in GetComponentsInChildren<Renderer>())
		{
			ren.enabled = false; //disable all visuals
		}
	}
	
	void ShowCharacter()
	{
		playerController.IsControllable = true;

		foreach (var ren in GetComponentsInChildren<Renderer>())
		{
			ren.enabled = true; //disable all visuals
		}
	}
}
