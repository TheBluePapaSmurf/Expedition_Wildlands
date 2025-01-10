using UnityEngine;
using System.Collections;

public class SpikeTrap : MonoBehaviour
{
    public float delayBeforeActivation = 2f;
    public float warningDuration = 1f; // Duur van de waarschuwing
    public float activationDuration = 1f;
    public float spikeSpeed = 10f; // Verhoogde snelheid
    public float activationHeight = 0.6f; // Hoogte waarmee de spikes omhoog komen
    public float warningHeight = 0.3f; // Hoogte voor de waarschuwing
    private Vector3 startPosition;
    private Vector3 activatedPosition;
    private Vector3 warningPosition;
    private BoxCollider boxCollider;
    private Renderer spikeRenderer;
    public Color warningColor = Color.red; // Kleur van de spikes tijdens de waarschuwing
    private Color originalColor;

    private void Start()
    {
        startPosition = transform.position;
        activatedPosition = startPosition + new Vector3(0, activationHeight, 0);
        warningPosition = startPosition + new Vector3(0, warningHeight, 0);
        boxCollider = GetComponent<BoxCollider>();
        spikeRenderer = GetComponent<Renderer>();

        if (boxCollider == null)
        {
            Debug.LogError("No BoxCollider found on the SpikeTrap object.");
        }
        else
        {
            boxCollider.enabled = false; // Start with the collider disabled
        }

        if (spikeRenderer != null)
        {
            originalColor = spikeRenderer.material.color; // Bewaar de originele kleur
        }

        StartCoroutine(ActivateSpikeTrap());
    }

    private IEnumerator ActivateSpikeTrap()
    {
        while (true)
        {
            yield return new WaitForSeconds(delayBeforeActivation);

            // Waarschuwing vooraf
            if (spikeRenderer != null)
            {
                spikeRenderer.material.color = warningColor; // Verander de kleur naar de waarschuwingskleur
            }

            float elapsedTime = 0f;
            while (elapsedTime < warningDuration)
            {
                transform.position = Vector3.MoveTowards(transform.position, warningPosition, spikeSpeed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (spikeRenderer != null)
            {
                spikeRenderer.material.color = originalColor; // Reset de kleur na de waarschuwing
            }

            // Beweeg de spikes naar de geactiveerde positie en zet de collider aan
            while (transform.position != activatedPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, activatedPosition, spikeSpeed * Time.deltaTime);
                yield return null;
            }
            boxCollider.enabled = true; // Enable the collider when spikes are up

            yield return new WaitForSeconds(activationDuration);

            // Beweeg de spikes terug naar de startpositie en zet de collider uit
            while (transform.position != startPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, startPosition, spikeSpeed * Time.deltaTime);
                yield return null;
            }
            boxCollider.enabled = false; // Disable the collider when spikes are down

            yield return new WaitForSeconds(delayBeforeActivation);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1); // Pas de hoeveelheid schade aan indien nodig
            }
        }
    }
}
