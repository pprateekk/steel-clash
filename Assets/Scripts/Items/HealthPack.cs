using UnityEngine;

public class HealthPack : MonoBehaviour
{
    public float addHealth = 5f;

    private void OnTriggerEnter(Collider aCollider)
    {
        if(aCollider.CompareTag("Player"))
        {
            PlayerStatus playerStat = aCollider.GetComponent<PlayerStatus>();
            if (playerStat != null)
            {
                playerStat.AddHealth(addHealth); //once the player touches the health pack, increase the player's health
            }

            Destroy(gameObject); //once increased, remove from the scene
        }
    }
}
