using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 5f;
    public int damage = 1; // Hoeveelheid schade die het projectiel veroorzaakt

    void Start()
    {
        // Vernietig de projectile na een bepaalde tijd om geheugen vrij te maken
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Beweeg de projectile naar voren op basis van zijn snelheid
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Controleer of de projectiel de speler raakt
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage); // Breng schade toe aan de speler
        }

        // Vernietig de projectile bij botsing
        Destroy(gameObject);
    }
}
