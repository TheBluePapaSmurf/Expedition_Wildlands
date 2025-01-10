using UnityEngine;

public class TiltingPlatform : MonoBehaviour
{
    public float tiltSpeed = 1f; // De snelheid waarmee het platform kantelt
    public float maxTiltAngle = 15f; // De maximale kantelhoek
    private Transform player; // De speler transform
    private Quaternion initialRotation; // De beginrotatie van het platform
    private bool playerOnPlatform = false; // Boolean om bij te houden of de speler op het platform is
    private bool returningToInitialRotation = false; // Boolean om bij te houden of het platform terugkeert naar de oorspronkelijke rotatie

    void Start()
    {
        initialRotation = transform.rotation;
        // Zoek de speler door de tag "Player"
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (playerOnPlatform && player != null)
        {
            Vector3 playerPosition = player.position;
            Vector3 platformPosition = transform.position;

            // Bereken de offset van de speler ten opzichte van het platform
            Vector3 localPlayerPosition = transform.InverseTransformPoint(playerPosition);

            // Normaleer de offset om een waarde tussen -1 en 1 te krijgen
            float normalizedOffsetX = Mathf.Clamp(localPlayerPosition.x / (transform.localScale.x / 2), -1f, 1f);
            float normalizedOffsetZ = Mathf.Clamp(localPlayerPosition.z / (transform.localScale.z / 2), -1f, 1f);

            // Bereken de kantelhoeken afhankelijk van de initiële rotatie
            float tiltAngleX = normalizedOffsetZ * maxTiltAngle;
            float tiltAngleZ = -normalizedOffsetX * maxTiltAngle;

            // Controleer de initiële rotatie en pas de kantelhoeken aan indien nodig
            if (Mathf.Approximately(initialRotation.eulerAngles.z, 180f) || Mathf.Approximately(initialRotation.eulerAngles.z, -180f))
            {
                tiltAngleX = -tiltAngleX;
                tiltAngleZ = -tiltAngleZ;
            }

            // Bereken de nieuwe rotatie gebaseerd op de initiële rotatie
            Quaternion targetRotation = initialRotation * Quaternion.Euler(tiltAngleX, 0, tiltAngleZ);

            // Pas de nieuwe rotatie toe
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * tiltSpeed);
        }
        else if (returningToInitialRotation)
        {
            // Keer langzaam terug naar de oorspronkelijke rotatie
            transform.rotation = Quaternion.Lerp(transform.rotation, initialRotation, Time.deltaTime * tiltSpeed);

            // Stop met terugkeren als we dicht genoeg bij de oorspronkelijke rotatie zijn
            if (Quaternion.Angle(transform.rotation, initialRotation) < 0.1f)
            {
                transform.rotation = initialRotation;
                returningToInitialRotation = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnPlatform = true;
            returningToInitialRotation = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnPlatform = false;
            returningToInitialRotation = true;
        }
    }
}
