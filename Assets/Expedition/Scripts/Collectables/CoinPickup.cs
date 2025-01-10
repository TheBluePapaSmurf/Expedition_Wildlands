using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int value = 1;
    public Vector3 rotationSpeed = new Vector3(0, 100, 0); // Aanpassen naar gewenste rotatiesnelheid
    private AudioSource audioSource;
    private bool hasBeenPickedUp = false;

    // Voeg een referentie naar het Particle System toe
    public ParticleSystem pickupEffect;

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
        if (other.CompareTag("Player"))
        {
            CollectCoin(other);
        }
    }

    void CollectCoin(Collider player)
    {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddCoins(value);
            hasBeenPickedUp = true;
            PlayPickupSoundAndDestroy();
        }
    }

    private void PlayPickupSoundAndDestroy()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
            HideObjectComponents();
            // Speel het particle effect af
            if (pickupEffect != null)
            {
                pickupEffect.Play();
            }
            Destroy(gameObject, audioSource.clip.length);
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
