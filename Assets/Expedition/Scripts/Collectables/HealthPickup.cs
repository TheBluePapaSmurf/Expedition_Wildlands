using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [Header("Health Settings")]
    public int healAmount = 1;

    [Header("Effects")]
    public GameObject healEffect;
    public AudioClip healSound;

    [Header("Magnet Settings")]
    public float attractSpeed = 3f;
    public float attractRange = 2f;
    public float collectDistance = 0.5f;

    private Transform playerTransform;
    private bool isAttracted = false;
    private bool isCollected = false;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (playerTransform != null && !isCollected)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            // Start attracting if player is close enough
            if (distanceToPlayer <= attractRange && !isAttracted)
            {
                isAttracted = true;
            }

            // Move towards player if attracted
            if (isAttracted)
            {
                transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, attractSpeed * Time.deltaTime);

                // Collect if close enough
                if (distanceToPlayer <= collectDistance)
                {
                    CollectHealth();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            playerTransform = other.transform;

            // Try immediate collection if player walks into it
            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null && playerHealth.GetCurrentLives() < playerHealth.maxLives)
            {
                CollectHealth();
            }
        }
    }

    private void CollectHealth()
    {
        if (isCollected) return;

        var playerHealth = playerTransform.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            // Only collect if player needs health
            if (playerHealth.GetCurrentLives() < playerHealth.maxLives)
            {
                isCollected = true;
                playerHealth.Heal(healAmount);

                // Play effects
                PlayHealEffects();

                Debug.Log($"Player healed for {healAmount} health!");

                // Destroy pickup
                Destroy(gameObject, 0.1f);
            }
        }
    }

    private void PlayHealEffects()
    {
        // Spawn heal effect
        if (healEffect != null)
        {
            Instantiate(healEffect, transform.position, Quaternion.identity);
        }

        // Play heal sound
        if (healSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(healSound);
        }
    }
}
