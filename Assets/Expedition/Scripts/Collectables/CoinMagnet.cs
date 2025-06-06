using UnityEngine;

public class CoinMagnet : MonoBehaviour
{
    [Header("Magnet Settings")]
    public float attractSpeed = 5f;
    public float speedIncreaseRate = 1f;
    public float collectDistance = 0.5f; // Distance at which coin is collected

    [Header("Coin Properties")]
    public int coinValue = 1;

    [Header("Effects")]
    public GameObject collectEffect;

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
        if (isAttracted && playerTransform != null && !isCollected)
        {
            // Move towards player with increasing speed
            attractSpeed += speedIncreaseRate * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, attractSpeed * Time.deltaTime);

            // Check if close enough to collect
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= collectDistance)
            {
                CollectCoin();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            playerTransform = other.transform;
            isAttracted = true;
        }
    }

    private void CollectCoin()
    {
        if (isCollected) return;

        isCollected = true;

        // Add coin to player's inventory/currency
        var coinCollector = playerTransform.GetComponent<CoinCollector>();
        if (coinCollector != null)
        {
            coinCollector.CollectCoin(coinValue);
        }

        // Hide visual components immediately
        HideObjectComponents();

        // Play collection effects
        PlayCollectionEffects();

        // Calculate proper destroy delay based on audio length
        float destroyDelay = 0.1f; // Default short delay

        if (audioSource != null && audioSource.clip != null)
        {
            destroyDelay = audioSource.clip.length;
        }

        // Destroy the coin after audio finishes
        Destroy(gameObject, destroyDelay);
    }

    private void PlayCollectionEffects()
    {
        // Spawn collection effect
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // Play collection sound ONLY from AudioSource component
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    private void HideObjectComponents()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null) renderer.enabled = false;

        Collider collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = false;
    }
}
