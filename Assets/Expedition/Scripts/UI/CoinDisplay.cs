using TMPro;
using UnityEngine;

public class CoinDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText; // Sleep hier het Text component in de Inspector
    private CoinCollector coinCollector;

    void Start()
    {
        FindCoinCollector();
        UpdateCoinText(); // Initialiseer de muntentekst
    }

    void FindCoinCollector()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            coinCollector = player.GetComponent<CoinCollector>();
            if (coinCollector != null)
            {
                // Subscribe to events for automatic updates
                coinCollector.onInventoryLoaded += UpdateCoinText;
                coinCollector.OnCoinsChanged.AddListener(OnCoinsChanged);
            }
            else
            {
                Debug.LogError("CoinCollector component not found on Player!");
            }
        }
        else
        {
            Debug.LogError("Player GameObject not found!");
        }
    }

    private void OnCoinsChanged(int newCoinCount)
    {
        UpdateCoinText();
    }

    private void UpdateCoinText()
    {
        if (coinCollector != null && coinText != null)
        {
            coinText.text = $"{coinCollector.Coins}";
        }
    }

    void OnDestroy()
    {
        if (coinCollector != null)
        {
            // Unsubscribe from events to prevent memory leaks
            coinCollector.onInventoryLoaded -= UpdateCoinText;
            coinCollector.OnCoinsChanged.RemoveListener(OnCoinsChanged);
        }
    }

    // Public method to manually refresh the display
    public void RefreshDisplay()
    {
        if (coinCollector == null)
        {
            FindCoinCollector();
        }
        UpdateCoinText();
    }
}
