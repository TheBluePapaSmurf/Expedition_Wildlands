using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public int maxLives = 4;
    private int currentLives;

    public float blinkIntensity;
    public float blinkDuration;
    public float blinkInterval = 0.1f; // Interval between blinks

    // Gebeurtenis wanneer speler sterft
    public delegate void EnemyDied();
    public static event EnemyDied OnEnemyDied;

    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
    private Color[] originalColors;
    private Color[] originalEmissionColors;

    private Animator animator;
    private EnemyAI enemyAI;
    public ParticleSystem deathParticleEffect; // Reference to the particle system
    public GameObject weapon; // Reference to the weapon

    void Start()
    {
        // Zet levens bij aanvang van het spel
        currentLives = maxLives;

        animator = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();

        if (skinnedMeshRenderer != null)
        {
            // Cache the original colors and emission colors of the materials
            originalColors = new Color[skinnedMeshRenderer.materials.Length];
            originalEmissionColors = new Color[skinnedMeshRenderer.materials.Length];

            for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
            {
                originalColors[i] = skinnedMeshRenderer.materials[i].color;
                originalEmissionColors[i] = skinnedMeshRenderer.materials[i].GetColor("_EmissionColor");
            }
        }
    }

    public int GetCurrentLives()
    {
        return currentLives;
    }

    // Functie om schade toe te brengen
    public void TakeDamage(int damage)
    {
        currentLives -= damage;

        // Controleer of speler dood is
        if (currentLives <= 0)
        {
            currentLives = 0;
            Die();
        }
        else
        {
            Debug.Log("Enemy heeft nog " + currentLives + " levens over.");
        }

        // Start the blink coroutine
        StartCoroutine(BlinkEffect());
    }

    private IEnumerator BlinkEffect()
    {
        float endTime = Time.time + blinkDuration;

        while (Time.time < endTime)
        {
            foreach (var material in skinnedMeshRenderer.materials)
            {
                // Blink on
                material.color = Color.white * blinkIntensity;
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", Color.white);
            }

            yield return new WaitForSeconds(blinkInterval);

            foreach (var material in skinnedMeshRenderer.materials)
            {
                // Blink off
                material.color = originalColors[Array.IndexOf(skinnedMeshRenderer.materials, material)];
                material.SetColor("_EmissionColor", originalEmissionColors[Array.IndexOf(skinnedMeshRenderer.materials, material)]);
                material.DisableKeyword("_EMISSION");
            }

            yield return new WaitForSeconds(blinkInterval);
        }

        // Ensure the original colors are set after blinking
        for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
        {
            var material = skinnedMeshRenderer.materials[i];
            material.color = originalColors[i];
            material.SetColor("_EmissionColor", originalEmissionColors[i]);
            material.DisableKeyword("_EMISSION");
        }
    }

    // Functie voor de speler-dood toestand
    private void Die()
    {
        Debug.Log("Speler is dood!");
        OnEnemyDied?.Invoke();

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        if (enemyAI != null)
        {
            enemyAI.enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
        }

        StartCoroutine(PlayDeathParticleEffect());
    }

    private IEnumerator PlayDeathParticleEffect()
    {
        yield return new WaitForSeconds(2f);

        if (deathParticleEffect != null)
        {
            // Disable components
            skinnedMeshRenderer.enabled = false;
            animator.enabled = false;
            GetComponent<Collider>().enabled = false;

            // Disable the weapon
            if (weapon != null)
            {
                weapon.SetActive(false);
            }

            // Play the particle effect
            deathParticleEffect.Play();

            // Wait for the particle effect to finish
            yield return new WaitWhile(() => deathParticleEffect.isPlaying);

            // Destroy the game object after the particle effect has finished
            Destroy(gameObject);
        }
    }
}
