using UnityEngine;

public class WreckingBall : MonoBehaviour
{
    public float rotationSpeed = 1f; // Snelheid van de oscillatie
    public float rotationAngle = 45f; // Maximale hoek van de rotatie
    private float timeCounter = 0f;

    void Update()
    {
        // Verhoog de tijdsteller met de rotatiesnelheid
        timeCounter += Time.deltaTime * rotationSpeed;

        // Bereken de nieuwe rotatiehoek met behulp van een sinusfunctie
        float angle = Mathf.Sin(timeCounter) * rotationAngle;

        // Pas de rotatie toe op de Z-as
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerRagdoll playerRagdoll = PlayerRagdoll.Instance;
            if (playerRagdoll != null)
            {
                playerRagdoll.SetRagdollState(true); // Activeer de ragdoll
            }
        }
    }
}
