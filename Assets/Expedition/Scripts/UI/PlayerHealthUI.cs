using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public GameObject healthIconPrefab;
    public PlayerHealth playerHealth;

    private GameObject[] healthIcons;

    void Start()
    {
        InitializeHealthIcons();
    }

    void Update()
    {
        UpdateHealthIcons();
    }

    private void InitializeHealthIcons()
    {
        healthIcons = new GameObject[playerHealth.maxLives];
        for (int i = 0; i < playerHealth.maxLives; i++)
        {
            // Maak een nieuw health icon voor elke leven
            GameObject icon = Instantiate(healthIconPrefab, transform);
            healthIcons[i] = icon;
        }
    }

    private void UpdateHealthIcons()
    {
        int currentLives = playerHealth.GetCurrentLives();

        for (int i = 0; i < healthIcons.Length; i++)
        {
            // Als de index kleiner is dan de huidige levens, toon het icoon, anders verberg het
            healthIcons[i].SetActive(i < currentLives);
        }
    }
}
