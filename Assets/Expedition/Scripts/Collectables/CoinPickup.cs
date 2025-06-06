using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [Header("Coin Settings")]
    public int value = 1;
    public Vector3 rotationSpeed = new Vector3(0, 100, 0); // Aanpassen naar gewenste rotatiesnelheid

    [Header("Effects")]
    public ParticleSystem pickupEffect;

    private AudioSource audioSource;
    private bool hasBeenPickedUp = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Zorg ervoor dat het Particle System niet speelt bij het starten
        if (pickupEffect != null && pickupEffect.isPlaying)
        {
            pickupEffect.Stop();
        }
    }

    void Update()
    {
        // Roteer de munt constant
        if (!hasBeenPickedUp)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenPickedUp)
        {
            CollectCoin(other);
        }
    }

    void CollectCoin(Collider player)
    {
        // Updated to use CoinCollector instead of PlayerInventory
        CoinCollector coinCollector = player.GetComponent<CoinCollector>();
        if (coinCollector != null)
        {
            coinCollector.CollectCoin(value);
            hasBeenPickedUp = true;
            PlayPickupSoundAndDestroy();
        }
        else
        {
            Debug.LogError("CoinCollector component not found on Player!");
        }
    }

    private void PlayPickupSoundAndDestroy()
    {
        // Hide visual components immediately
        HideObjectComponents();

        // Play effects
        PlayPickupEffects();

        // Destroy after audio finishes (or immediately if no audio)
        float destroyDelay = 0.1f; // Default short delay

        if (audioSource != null && audioSource.clip != null)
        {
            destroyDelay = audioSource.clip.length;
        }

        Destroy(gameObject, destroyDelay);
    }

    private void PlayPickupEffects()
    {
        // Play audio effect from AudioSource component only
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }

        // Play particle effect
        if (pickupEffect != null)
        {
            pickupEffect.Play();
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
