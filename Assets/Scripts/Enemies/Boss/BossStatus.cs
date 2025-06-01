using UnityEngine;
using System.Collections;

public class BossStatus : MonoBehaviour
{
    private float health = 100f;
    private float maxHealth = 100f;
    private bool dead = false;

    public Animator animator;
    private BossHealthBar healthBar;

    void Start()
    {
        animator = GetComponent<Animator>();
        healthBar = GetComponentInChildren<BossHealthBar>();
        healthBar.updateHealthBar(health, maxHealth);
    }

    public bool isAlive() { return !dead; }

    public void ApplyDamage(float damage)
    {
        if (dead) return;
        health -= damage;
        healthBar.updateHealthBar(health, maxHealth);
        Debug.Log($"Enemy took {damage} damage. Health now: {health}");
        if (health <= 0)
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
            animator.SetTrigger("dead");
            animator.Play("Demon|Death", 0, 0f);

            Debug.Log("Playing death animation");
        }
        Debug.Log("***********Enemy Dead!*************");

        yield return new WaitForSeconds(2.5f); //let the death anim play

        GameManager gameManager = FindObjectOfType<GameManager>();
		if (gameManager != null)
		{
			gameManager.BossDied();
		}

        Destroy(gameObject); // remove the enemy from the scene
    }

    public float ReturnHealth()
    {
        return health;
    }
}
