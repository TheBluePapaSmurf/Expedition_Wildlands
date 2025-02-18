using System.Collections;
using UnityEngine;

public class SinkingPlatform : MonoBehaviour
{
    public float delayBeforeFall = 2f; // Tijd dat het platform trilt voordat het valt
    public float destroyTime = 3f; // Tijd voordat het platform verdwijnt na het vallen
    public float shakeIntensity = 0.1f; // Hoe sterk het platform trilt
    public float shakeSpeed = 50f; // Hoe snel het platform trilt

    private Rigidbody rb;
    private bool isTriggered = false;
    private Vector3 originalPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Zorgt ervoor dat het platform niet beweegt voordat het trilt
        rb.useGravity = false; // Gravity staat uit totdat het platform daadwerkelijk valt
        originalPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            StartCoroutine(ShakeAndFall());
        }
    }

    IEnumerator ShakeAndFall()
    {
        float timer = 0;
        while (timer < delayBeforeFall)
        {
            // Trillen op de X en Z-as
            float shakeX = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensity;
            float shakeZ = Mathf.Cos(Time.time * shakeSpeed) * shakeIntensity;

            transform.position = originalPosition + new Vector3(shakeX, 0, shakeZ);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition; // Zet het platform terug naar de originele positie
        rb.isKinematic = false; // Zet physics aan
        rb.useGravity = true; // Laat het platform vallen door zwaartekracht

        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject); // Verwijder het platform na vallen
    }
}