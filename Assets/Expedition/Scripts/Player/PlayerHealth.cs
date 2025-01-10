using UnityEngine;
using System.Collections;
using System;

public class PlayerHealth : MonoBehaviour
{
    // Aantal levens van de speler
    public int maxLives = 4;
    private int currentLives;

    public float blinkIntensity;
    public float blinkDuration;
    public float blinkInterval = 0.1f; // Interval between blinks

    private const string saveFileName = "playerData.es3";

    // Gebeurtenis wanneer speler sterft
    public delegate void PlayerDied();
    public static event PlayerDied OnPlayerDied;

    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Color[] originalColors;
    private Color[] originalEmissionColors;

    void Awake()
    {
        LoadHealth();  // Optioneel, afhankelijk van waar je wilt dat laden plaatsvindt
    }

    void Start()
    {
        // Zet levens bij aanvang van het spel
        currentLives = maxLives;

        SetCurrentMeshRenderer();

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

    void Update()
    {
        // Debug Keys
        if (Input.GetKeyDown(KeyCode.X))
        {
            TakeDamage(1); // Breng 1 punt schade toe als 'X' wordt ingedrukt
            Debug.Log("Debug Key 'X' pressed: Took 1 point of damage.");
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(1); // Genees 1 punt als 'H' wordt ingedrukt
            Debug.Log("Debug Key 'H' pressed: Healed 1 point of health.");
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
            Debug.Log("Speler heeft nog " + currentLives + " levens over.");
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
        OnPlayerDied?.Invoke();
        // Voeg hier extra code toe, bijvoorbeeld om een 'game over' scherm te tonen
    }

    // Functie om levens bij te vullen (optioneel)
    public void Heal(int amount)
    {
        if (currentLives < maxLives)
        {
            currentLives = Mathf.Min(currentLives + amount, maxLives);
            Debug.Log("Speler is genezen, levens: " + currentLives);
        }
    }

    public void SaveHealth()
    {
        ES3.Save<int>("health", currentLives, saveFileName);
    }

    public void LoadHealth()
    {
        if (ES3.KeyExists("health", saveFileName))
        {
            currentLives = ES3.Load<int>("health", saveFileName);
            Debug.Log("Health loaded: " + currentLives);
        }
        else
        {
            currentLives = maxLives;
            Debug.Log("No health data found, setting to max.");
        }
    }

    public void SetCurrentMeshRenderer()
    {
        skinnedMeshRenderer = CharacterManager.Instance.GetCurrentMeshRenderer();

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
}
