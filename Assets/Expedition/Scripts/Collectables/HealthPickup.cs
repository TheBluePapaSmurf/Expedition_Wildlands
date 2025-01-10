using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 1;
    private AudioSource audioSource;
    private bool hasBeenPickedUp = false;

    // Beweging parameters
    public float floatAmplitude = 0.5f;  // Amplitude van de beweging (hoe hoog het object beweegt)
    public float floatFrequency = 1f;    // Frequentie van de beweging (hoe snel het object beweegt)

    private Vector3 startPosition;       // Beginpositie van het object

    // Voeg een referentie naar het Particle System toe
    public ParticleSystem pickupEffect;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        startPosition = transform.position;  // Stel de startpositie in

        // Zorg ervoor dat het Particle System niet speelt bij het starten
        if (pickupEffect != null && pickupEffect.isPlaying)
        {
            pickupEffect.Stop();
        }
    }

    void Update()
    {
        // Beweeg het object op en neer
        if (!hasBeenPickedUp)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !hasBeenPickedUp)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                if (playerHealth.GetCurrentLives() < playerHealth.maxLives)
                {
                    playerHealth.Heal(healAmount);
                    hasBeenPickedUp = true;
                    PlayPickupSoundAndDestroy();
                }
                else
                {
                    Debug.Log("Speler heeft al volledige gezondheid.");
                }
            }
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
        Collider collider = GetComponent<Collider>();

        if (renderer != null)
            renderer.enabled = false;
        if (collider != null)
            collider.enabled = false;
    }
}
