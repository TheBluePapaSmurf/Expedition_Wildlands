using UnityEngine;

public class RicoHelmet : MonoBehaviour
{
    public int damageAmount = 5; // Schade die de helm veroorzaakt

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);
            }
        }

        // Vernietig de helm na botsing met een vijand of andere objecten
        Destroy(gameObject);
    }
}
