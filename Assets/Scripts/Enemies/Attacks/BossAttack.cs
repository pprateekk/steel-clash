using UnityEngine;
using System.Collections;

public class BossAttack : EnemyAttack
{
    private Animator animator;
    private bool canAttack = true;
    private bool attackInProgress = false;
    public float attackCoolDown = 2.0f;
    public float appliedDamage = 15f;
    public float poundDamage = 25f;
    public float attackRange = 3.0f;
    public float firstHitTime = 0.14f;
    public float poundHitTime = 0.34f;
    private int isAttackHash;
    private int isPoundHash;
    [Range(0, 1)]
    public float regularAttackChance = 0.5f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        isAttackHash = Animator.StringToHash("isAttack");
        isPoundHash = Animator.StringToHash("isPound");
    }

    private IEnumerator AttackCoolDown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCoolDown);
        canAttack = true;
    }

    public override void Attack(GameObject player)
    {
        if (!canAttack || attackInProgress) return;
        attackInProgress = true;

        bool usePoundAttack = Random.value > regularAttackChance;

        if (usePoundAttack)
        {
            animator.SetBool(isPoundHash, true);
            StartCoroutine(TrackPoundAttackProgress(player));
        }
        else
        {
            animator.SetBool(isAttackHash, true);
            StartCoroutine(TrackRegularAttackProgress(player));
        }

        StartCoroutine(AttackCoolDown());
    }

    private IEnumerator TrackRegularAttackProgress(GameObject player)
    {
        animator.Play("Demon|Punch2", 0, 0f);
        yield return new WaitForSeconds(1.0f);
        ApplyDamageIfInRange(player, appliedDamage);
        yield return new WaitForSeconds(0.1f);
        animator.SetBool(isAttackHash, false);
        attackInProgress = false;
    }

    private IEnumerator TrackPoundAttackProgress(GameObject player)
    {
        animator.Play("Demon|Punch3", 0, 0f);
        yield return new WaitForSeconds(1.2f);
        ApplyDamageIfInRange(player, poundDamage);
        yield return new WaitForSeconds(0.1f);
        animator.SetBool(isPoundHash, false);
        attackInProgress = false;
    }

    private void ApplyDamageIfInRange(GameObject player, float damageAmount)
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= attackRange)
            {
                var playerStatus = player.GetComponent<PlayerStatus>();
                if (playerStatus != null)
                {
                    float damage = damageAmount;
                    if (playerStatus.IsBlocking)
                    {
                        damage = damage * 0.5f;
                    }
                    playerStatus.ApplyDamage(damage);
                    Debug.Log("Applied damage: " + damage);
                }
            }
        }
    }

    public void ResetAttackState()
    {
        animator.SetBool(isAttackHash, false);
        animator.SetBool(isPoundHash, false);
        attackInProgress = false;
    }
}
