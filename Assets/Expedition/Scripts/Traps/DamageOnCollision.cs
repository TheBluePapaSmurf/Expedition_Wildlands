using UnityEngine;

public class DamageOnCollision : MonoBehaviour
{
    public int damageAmount = 1; // De hoeveelheid schade die wordt toegebracht

    private void OnCollisionEnter(Collision collision)
    {
        // Controleer of de speler de collider raakt
        if (collision.gameObject.CompareTag("Player"))
        {
            // Verkrijg het PlayerHealth component van de speler
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Breng schade toe aan de speler
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Controleer of de speler de trigger raakt
        if (other.CompareTag("Player"))
        {
            // Verkrijg het PlayerHealth component van de speler
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Breng schade toe aan de speler
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }
}
