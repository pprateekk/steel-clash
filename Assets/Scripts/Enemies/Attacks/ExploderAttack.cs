using UnityEngine;
using System.Collections;

public class ExploderAttack : EnemyAttack
{
    [Header("Explosion Settings")]
    public float explosionDamage = 20f;
    public float explodeDelay = 5.0f;

    private bool hasExploded = false;

    public override void Attack(GameObject player)
    {
        if (hasExploded) return;

        hasExploded = true;

        PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
        if (playerStatus != null)
        {
            playerStatus.ApplyDamage(explosionDamage);
        }

        StartCoroutine(ExplodeCoroutine());
    }

    private IEnumerator ExplodeCoroutine()
    {
        yield return new WaitForSeconds(explodeDelay);

        Destroy(gameObject);
    }
}
