using UnityEngine;
using TMPro;

public class CoinDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText; // Sleep hier het Text component in de Inspector
    private PlayerInventory playerInventory;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerInventory = player.GetComponent<PlayerInventory>();
            if (playerInventory != null)
            {
                playerInventory.onInventoryLoaded += UpdateCoinText; // Abonneer op het laden event
            }
        }

        UpdateCoinText(); // Initialiseer de muntentekst
    }

    void Update()
    {
        // Werk de tekst bij bij elke frame update
        UpdateCoinText();
    }

    private void UpdateCoinText()
    {
        if (playerInventory != null)
        {
            coinText.text = $"{playerInventory.Coins}";
        }
    }

    void OnDestroy()
    {
        if (playerInventory != null)
        {
            playerInventory.onInventoryLoaded -= UpdateCoinText; // Zorg ervoor dat je afmeldt voor het event
        }
    }
}
