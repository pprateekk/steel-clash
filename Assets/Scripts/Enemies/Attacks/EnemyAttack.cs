using UnityEngine;

public abstract class EnemyAttack : MonoBehaviour
{
    public float damage = 10.0f;

    public abstract void Attack(GameObject player);
}
