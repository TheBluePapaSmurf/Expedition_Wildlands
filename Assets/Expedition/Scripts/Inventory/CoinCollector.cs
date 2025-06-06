using UnityEngine;
using UnityEngine.Events;

public class CoinCollector : MonoBehaviour
{
    [Header("Currency Settings")]
    public int currentCoins = 0;

    [Header("Events")]
    public UnityEvent<int> OnCoinsChanged;
    public UnityEvent<int> OnCoinCollected;

    // Event for CoinDisplay compatibility
    public System.Action onInventoryLoaded;

    // Property for CoinDisplay compatibility
    public int Coins => currentCoins;

    private const string COINS_SAVE_KEY = "playerCoins";

    void Start()
    {
        // Don't load coins here if using SaveSystem - it will handle loading
        // LoadCoins();
        UpdateCoinDisplay();

        // Trigger the inventory loaded event for UI compatibility
        onInventoryLoaded?.Invoke();
    }

    public void CollectCoin(int amount)
    {
        currentCoins += amount;

        // Trigger events
        OnCoinCollected?.Invoke(amount);
        OnCoinsChanged?.Invoke(currentCoins);

        Debug.Log($"Collected {amount} coins! Total: {currentCoins}");

        // Update UI
        UpdateCoinDisplay();

        // Trigger inventory loaded event for UI updates
        onInventoryLoaded?.Invoke();
    }

    public bool SpendCoins(int amount)
    {
        if (currentCoins >= amount)
        {
            currentCoins -= amount;
            OnCoinsChanged?.Invoke(currentCoins);
            UpdateCoinDisplay();

            // Trigger inventory loaded event for UI updates
            onInventoryLoaded?.Invoke();
            return true;
        }
        return false;
    }

    public int GetCoinCount()
    {
        return currentCoins;
    }

    // Method for SaveSystem to load coins
    public void LoadCoins(int amount)
    {
        currentCoins = amount;
        OnCoinsChanged?.Invoke(currentCoins);
        UpdateCoinDisplay();

        // Trigger inventory loaded event for UI updates
        onInventoryLoaded?.Invoke();

        Debug.Log($"Coins loaded from save: {currentCoins}");
    }

    private void UpdateCoinDisplay()
    {
        // This will be handled by CoinDisplay script automatically
        // No need for UIManager reference
    }

    // Legacy save/load methods for backup (if not using SaveSystem)
    private void SaveCoins()
    {
        PlayerPrefs.SetInt(COINS_SAVE_KEY, currentCoins);
        PlayerPrefs.Save();
    }

    private void LoadCoinsFromPlayerPrefs()
    {
        currentCoins = PlayerPrefs.GetInt(COINS_SAVE_KEY, 0);
        onInventoryLoaded?.Invoke();
    }
}
