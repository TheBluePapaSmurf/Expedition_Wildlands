using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    private const string saveFileName = "playerData.es3";

    public int Coins { get; private set; } = 0; // Aantal munten in de inventory
    public List<(InventoryItem item, int quantity)> Inventory { get; private set; } = new List<(InventoryItem, int)>();

    public delegate void OnInventoryLoaded();
    public event OnInventoryLoaded onInventoryLoaded;
    public InventoryUI inventoryUI;

    public delegate void OnInventoryChanged(List<(InventoryItem item, int quantity)> inventory);
    public event OnInventoryChanged onInventoryChanged;

    private void Awake()
    {
        Inventory = new List<(InventoryItem item, int quantity)>();
    }

    public void AddCoins(int amount)
    {
        Coins += amount;
        Debug.Log("Coins added. Total coins: " + Coins);
    }

    public void AddItem(InventoryItem item, int quantity)
    {
        if (item.isStackable)
        {
            var existingItem = Inventory.Find(i => i.item.ID == item.ID);
            if (existingItem.item != null)
            {
                Inventory[Inventory.IndexOf(existingItem)] = (existingItem.item, existingItem.quantity + quantity);
            }
            else
            {
                Inventory.Add((item, quantity));
            }
        }
        else
        {
            Inventory.Add((item, quantity));
        }

        // Roep het event aan om de UI te updaten
        onInventoryChanged?.Invoke(Inventory);
    }

    public void SaveInventory()
    {
        ES3.Save("coins", Coins, saveFileName);
        ES3.Save("inventory", Inventory, saveFileName); // Opslaan van de lijst
        Debug.Log("Inventory saved. Coins: " + Coins);
    }

    public void LoadInventory()
    {
        if (ES3.KeyExists("coins", saveFileName))
        {
            Coins = ES3.Load<int>("coins", saveFileName);
        }

        if (ES3.KeyExists("inventory", saveFileName))
        {
            Inventory = ES3.Load<List<(InventoryItem, int)>>("inventory", saveFileName);
        }

        onInventoryLoaded?.Invoke();
        Debug.Log("Inventory loaded. Coins: " + Coins);
    }
}
