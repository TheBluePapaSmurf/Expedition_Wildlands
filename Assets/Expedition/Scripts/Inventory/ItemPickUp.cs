using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    [SerializeField] private InventoryItem item; // Het item dat opgepakt wordt
    [SerializeField] private int quantity = 1;   // Aantal dat opgepakt wordt
    [SerializeField] private ParticleSystem pickupEffect; // Optioneel: een particle effect
    private AudioSource audioSource;
    private bool hasBeenPickedUp = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Zorg ervoor dat het Particle System niet speelt bij het starten
        if (pickupEffect != null && pickupEffect.isPlaying)
        {
            pickupEffect.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasBeenPickedUp)
        {
            CollectItem(other);
        }
    }

    private void CollectItem(Collider player)
    {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddItem(item, quantity);
            hasBeenPickedUp = true;
            PlayPickupSoundAndDestroy();
        }
    }

    private void PlayPickupSoundAndDestroy()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play(); // Speel het geluid af
            HideObjectComponents(); // Verberg visuele en fysieke componenten
            if (pickupEffect != null)
            {
                pickupEffect.Play(); // Speel het particle effect af
            }
            Destroy(gameObject, audioSource.clip.length); // Verwijder het object na de lengte van het geluid
        }
        else
        {
            Destroy(gameObject); // Geen geluid? Verwijder het object meteen
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
