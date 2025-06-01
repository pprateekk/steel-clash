using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float damage = 5f;
    public float arrowSpeed = 20f;
    public float arrowLife = 5f;
    private float checkLifeTime;

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * arrowSpeed * Time.deltaTime;

        checkLifeTime += Time.deltaTime;
        if(checkLifeTime > arrowLife)
        {
            Destroy(gameObject);
        }

    }

    void OnTriggerEnter(Collider aCollider)
    {
        PlayerStatus playerStat = aCollider.GetComponent<PlayerStatus>();
        if(playerStat != null)
        {
            if(!playerStat.IsBlocking)
            {
                playerStat.ApplyDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
