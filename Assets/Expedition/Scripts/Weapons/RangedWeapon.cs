using UnityEngine;

public class RangedWeapon : MonoBehaviour
{
    public bool isShooting = false;
    public float fireRate = 1.0f;
    private float nextFireTime = 0.0f;

    public GameObject projectilePrefab;
    public Transform firePoint;

    public AudioClip shootSound; // Voeg een veld toe voor het geluidseffect
    private AudioSource audioSource; // Referentie naar de AudioSource component

    public ParticleSystem shootEffect; // Referentie naar het particle effect

    private EnemyHealth enemyHealth; // Referentie naar de EnemyHealth component

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Haal de AudioSource component op
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Voeg een AudioSource component toe als deze niet bestaat
        }

        // Haal de EnemyHealth component op
        enemyHealth = GetComponentInParent<EnemyHealth>();
    }

    public void BeginShooting()
    {
        isShooting = true;
        Shoot();
    }

    public void EndShooting()
    {
        isShooting = false;
    }

    void Shoot()
    {
        if (isShooting && Time.time >= nextFireTime && enemyHealth != null && enemyHealth.GetCurrentLives() > 0)
        {
            nextFireTime = Time.time + 1f / fireRate;
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            // Speel het schietgeluid af
            if (shootSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(shootSound);
            }

            // Speel het particle effect af
            if (shootEffect != null)
            {
                shootEffect.Play();
            }
        }
    }

    void Update()
    {
        if (isShooting)
        {
            Shoot();
        }
    }
}
