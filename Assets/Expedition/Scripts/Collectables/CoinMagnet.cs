using UnityEngine;

public class CoinMagnet : MonoBehaviour
{
    public float attractSpeed = 5f; // Initiële snelheid waarmee de munt naar de speler beweegt
    public float speedIncreaseRate = 1f; // De snelheid waarmee de attractiesnelheid toeneemt over tijd
    private Transform playerTransform;
    private bool isAttracted = false;

    void Update()
    {
        if (isAttracted && playerTransform != null)
        {
            attractSpeed += speedIncreaseRate * Time.deltaTime; // Verhoog de snelheid over tijd
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, attractSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            isAttracted = true;  // Start aantrekking zodra de speler de triggerzone binnenkomt
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
        }
    }
}
