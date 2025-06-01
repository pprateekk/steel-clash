using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float attackDistance = 6.0f;
    public CharacterController controller;
    private PlayerStatus status;
    private float originalAttackValue = 5.0f;
    public Animator animator;

    private float gravity = -9.81f;
    private float moveSpeed = 15.0f;
    public float rotateSpeed = 100.0f;
    private float jumpHeight = 1;
    float yVelocity = 0;

    // private Animation animation;
    private bool isControllable = true;
    private GameObject[] enemies;
    private bool doneDamage = false;

    private int isWalkingHash;
    private int isRunningHash;
    private int isBlockingHash;
    private int isDeadHash;
    private int attackTriggerHash;
    private int kickTriggerHash;


    private bool isBoosted = false;
    private float attackValue = 5.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        status = GetComponent<PlayerStatus>();
        // animation = GetComponent<Animation>();
        animator = GetComponent<Animator>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isBlockingHash  = Animator.StringToHash("isBlocking");
        isDeadHash = Animator.StringToHash("isDead");
        
        attackTriggerHash = Animator.StringToHash("Attack");
        kickTriggerHash = Animator.StringToHash("Kick");
    }

    public bool IsControllable
    {
        get { return isControllable; }
        set { isControllable = value; }
    }

    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width - 100, 0, 100, 50), " " + status.GetHealth().ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if (!isControllable)
        {
            return;
        }

        if(status.GetHealth() <= 0)
        {
            animator.SetBool(isDeadHash, true);
            isControllable = false;
            return;
        }

        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        bool moveForward = Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow);
        bool runForward = Input.GetKey("left shift");

        animator.SetBool(isWalkingHash, moveForward); //walking if w pressed or up/down arrow keys

        animator.SetBool(isRunningHash, (moveForward && runForward)); //isRunning true if w and left shift pressed


        Vector3 forwardMove = transform.forward * verticalInput * moveSpeed; //move forward

        if(moveForward && runForward)
        {
            forwardMove = transform.forward * verticalInput * (moveSpeed * 3);
        }

        //rotate 
        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            //rotate around y
            float rot = horizontalInput * rotateSpeed * Time.deltaTime;
            transform.Rotate(0, rot, 0);
        }

        Vector3 playerVelocity = forwardMove;

        // jump
        if (controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            //Debug.Log("Jump");
            yVelocity = Mathf.Sqrt(jumpHeight * -2f * (gravity));
        }

        //Apply  gravity
        yVelocity += gravity * Time.deltaTime;

        playerVelocity.y = yVelocity;

        controller.Move(playerVelocity * Time.deltaTime);

        //attack 
        if(Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger(attackTriggerHash);
            int closestEnemyInd = FindClosest();
            if (closestEnemyInd >= 0)
            {
                if(enemies[closestEnemyInd] != null)
                {
                    AIStatus aiStatus = enemies[closestEnemyInd].GetComponent<AIStatus>();
                    if(aiStatus != null)
                    {
                        aiStatus.ApplyDamage(attackValue);
                    }
                    else
                    {
                        // If no AIStatus found, check for BossStatus
                        BossStatus bossStatus = enemies[closestEnemyInd].GetComponent<BossStatus>();
                        if (bossStatus != null)
                        {
                            bossStatus.ApplyDamage(attackValue);
                        }
                    }
                }
            }
        }

        //kick
        if(Input.GetKeyDown(KeyCode.K))
        {
            animator.SetTrigger(kickTriggerHash);
        }

        //block
        bool isBlocking = Input.GetKey(KeyCode.LeftAlt);
        animator.SetBool(isBlockingHash, isBlocking);
        status.IsBlocking = isBlocking;
    }

    int FindClosest()
    {
        // getting all the eneies found
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null)
            {
                Debug.LogWarning($"[FindClosest] enemies[{i}] is null!");
            }
            else
            {
                Debug.Log($"[FindClosest] enemies[{i}] = {enemies[i].name}");
            }
        }

        float minDistance = 20000;
        int closest = -1;
        for (int i = 0; i < enemies.Length; i++)
        {
            // check if the enemy was killed not not assigned
            if (enemies[i] == null)
            {
                Debug.LogWarning($"[FindClosest] Enemy at index {i} is null, skipping...");
                continue;
            }

            AIStatus enemyStatus = enemies[i].GetComponent<AIStatus>();
            BossStatus bossStatus = enemies[i].GetComponent<BossStatus>();
            if (enemyStatus == null && bossStatus == null)
            {
                Debug.LogWarning($"[FindClosest] Enemy {enemies[i].name} does NOT have AIStatus or BossStatus, skipping...");
                continue;
            }

            // if dead, skip it
            bool isAlive;
            if (enemyStatus != null)
            {
                isAlive = enemyStatus.isAlive();
            }
            else
            {
                isAlive = bossStatus.isAlive();
            }

            if (!isAlive)
            {
                Debug.Log($"[FindClosest] Enemy {enemies[i].name} is not alive, skipping...");
                continue;
            }

            float dist = Vector3.Distance(enemies[i].transform.position, transform.position);
            Debug.Log($"[FindClosest] Enemy {enemies[i].name}, dist = {dist}");

            // check if the enbemy is within the attackDistance 
            if (dist <= attackDistance)
            {
                Debug.Log($"[FindClosest] Enemy {enemies[i].name} is within {attackDistance} distance");
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = i;
                }
            }
        }

        // print which ones the closest
        if (closest >= 0)
        {
            Debug.Log($"[FindClosest] Closest enemy is {enemies[closest].name} at distance {minDistance}");
        }
        else
        {
            Debug.Log("[FindClosest] No  enemy found within attackDistance");
        }

        return closest;
    }

    public IEnumerator AttackBoostRoutine(float attackTimes, float duration)
    {
        if(!isBoosted)
        {
            isBoosted = true;

            originalAttackValue = attackValue;

            attackValue = attackValue * attackTimes; //which is 2x for now

            yield return new WaitForSeconds(duration);

            attackValue = originalAttackValue; //restore the original attack value 
            isBoosted = false;
        }
    }
}
