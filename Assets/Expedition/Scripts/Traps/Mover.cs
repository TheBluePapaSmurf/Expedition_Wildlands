using UnityEngine;

public class SmoothUpDownMovement : MonoBehaviour
{
    public float amplitude = 1f; // Hoogte van de beweging
    public float frequency = 1f; // Snelheid van de beweging

    private Vector3 startPos;

    void Start()
    {
        // Bewaar de startpositie
        startPos = transform.position;
    }

    void Update()
    {
        // Bereken de nieuwe positie met behulp van een sinusfunctie
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;

        // Pas de nieuwe positie toe
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
