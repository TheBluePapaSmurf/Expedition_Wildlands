using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private const string saveFileName = "playerData.es3";

    public int Coins { get; private set; } = 0; // Aantal munten in de inventory

    public delegate void OnInventoryLoaded();
    public event OnInventoryLoaded onInventoryLoaded;


    public void AddCoins(int amount)
    {
        Coins += amount;
        Debug.Log("Coins added. Total coins: " + Coins);
    }

    public void SaveInventory()
    {
        ES3.Save("coins", Coins, saveFileName);
        Debug.Log("Inventory saved. Coins: " + Coins);
    }

    public void LoadInventory()
    {
        if (ES3.KeyExists("coins", saveFileName))
        {
            Coins = ES3.Load<int>("coins", saveFileName);
            onInventoryLoaded?.Invoke();
        }
        Debug.Log("Inventory loaded. Coins: " + Coins);
    }
}
