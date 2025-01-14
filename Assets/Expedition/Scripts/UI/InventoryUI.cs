using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel; // De UI van de inventory
    public GameObject slotPrefab; // Prefab voor een leeg slot
    public Transform slotParent; // De Grid Layout Group

    private List<GameObject> slots = new List<GameObject>(); // Lijst met alle slots
    private bool isInitialized = false;

    [Header("Inventory Settings")]
    public int maxSlots = 20; // Maximaal aantal slots

    [SerializeField] private PlayerInventory playerInventory;

    // Vul de inventory UI met lege slots bij start
    private void Start()
    {
        InitializeUI();

        if (playerInventory != null)
        {
            // Koppel het event
            playerInventory.onInventoryChanged += UpdateUI;

            // Zorg ervoor dat de UI wordt bijgewerkt bij het starten
            UpdateUI(playerInventory.Inventory);
        }

        if (slotPrefab == null)
        {
            Debug.LogError("SlotPrefab is not assigned in the Inspector!");
            return;
        }

        if (slotParent == null)
        {
            Debug.LogError("SlotParent is not assigned in the Inspector!");
            return;
        }

        slots = new List<GameObject>();

        for (int i = 0; i < maxSlots; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotParent);
            if (newSlot == null)
            {
                Debug.LogError("Failed to instantiate SlotPrefab!");
                continue;
            }

            slots.Add(newSlot);

            // Controleer of de prefab de juiste componenten heeft
            var itemIcon = newSlot.transform.Find("ItemIcon");
            var quantityTextBG = newSlot.transform.Find("ItemIcon/QuantityTextBG");
            var quantityText = newSlot.transform.Find("ItemIcon/QuantityTextBG/QuantityText");

            if (itemIcon == null)
            {
                Debug.LogError($"SlotPrefab is missing 'ItemIcon' for slot {i}!");
            }
            if (quantityTextBG == null)
            {
                Debug.LogError($"SlotPrefab is missing 'QuantityTextBG' for slot {i}!");
            }
            if (quantityText == null)
            {
                Debug.LogError($"SlotPrefab is missing 'QuantityText' for slot {i}!");
            }
        }
    }

    private void OnEnable()
    {
        if (isInitialized)
        {
            UpdateUI(playerInventory.Inventory);
        }
    }

    private void InitializeUI()
    {
        if (playerInventory != null)
        {
            UpdateUI(playerInventory.Inventory);
            isInitialized = true; // Markeer dat de UI is geïnitialiseerd
        }
        else
        {
            Debug.LogWarning("PlayerInventory is not assigned!");
        }
    }

    public void UpdateUI(List<(InventoryItem item, int quantity)> inventory)
    {
        // Wis de bestaande UI-slots
        foreach (var slot in slots)
        {
            var emptyIcon = slot.transform.Find("InventoryBG")?.gameObject;
            var itemIcon = slot.transform.Find("ItemIcon")?.gameObject;
            var quantityTextBG = slot.transform.Find("ItemIcon/QuantityTextBG")?.gameObject;
            var quantityText = slot.transform.Find("ItemIcon/QuantityTextBG/QuantityText")?.GetComponent<TextMeshProUGUI>();

            if (emptyIcon != null) emptyIcon.SetActive(true); // Toon leeg slot
            if (itemIcon != null) itemIcon.SetActive(false); // Verberg item
            if (quantityTextBG != null) quantityTextBG.SetActive(false); // Verberg hoeveelheid
            if (quantityText != null) quantityText.text = ""; // Reset hoeveelheid
        }

        // Vul de UI met de huidige inventory-inhoud
        for (int i = 0; i < inventory.Count && i < slots.Count; i++)
        {
            var slot = slots[i];
            var emptyIcon = slot.transform.Find("InventoryBG")?.gameObject;
            var itemIcon = slot.transform.Find("ItemIcon")?.gameObject;
            var quantityTextBG = slot.transform.Find("ItemIcon/QuantityTextBG")?.gameObject;
            var quantityText = slot.transform.Find("ItemIcon/QuantityTextBG/QuantityText")?.GetComponent<TextMeshProUGUI>();

            if (emptyIcon != null) emptyIcon.SetActive(false); // Verberg leeg slot
            if (itemIcon != null)
            {
                itemIcon.SetActive(true); // Toon item
                var iconImage = itemIcon.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = inventory[i].item.Icon; // Stel sprite in
                }
            }

            if (quantityTextBG != null) quantityTextBG.SetActive(true); // Toon hoeveelheid achtergrond
            if (quantityText != null) quantityText.text = inventory[i].quantity.ToString(); // Stel hoeveelheid in
        }
    }



    // Update de inventory UI wanneer een item wordt opgepakt
    public void AddItemToUI(Sprite itemSprite, int quantity)
    {
        // Controleer of het item al in een slot zit
        foreach (var slot in slots)
        {
            var itemIcon = slot.transform.Find("ItemIcon")?.gameObject;
            var quantityTextBG = slot.transform.Find("ItemIcon/QuantityTextBG")?.gameObject;
            var quantityText = slot.transform.Find("ItemIcon/QuantityTextBG/QuantityText")?.GetComponent<TextMeshProUGUI>();

            if (itemIcon != null && itemIcon.activeSelf) // Controleer of het slot al een item heeft
            {
                var iconImage = itemIcon.GetComponent<Image>();
                if (iconImage != null && iconImage.sprite == itemSprite) // Controleer of het dezelfde sprite is
                {
                    // Update de hoeveelheid
                    if (quantityText != null)
                    {
                        int currentQuantity = int.Parse(quantityText.text);
                        quantityText.text = (currentQuantity + quantity).ToString();
                        Debug.Log($"Updated quantity for item {itemSprite.name} to {currentQuantity + quantity}");
                    }
                    return; // Stop hier, geen nieuw slot vullen
                }
            }
        }

        // Voeg het item toe aan een nieuw slot als het nog niet bestaat
        foreach (var slot in slots)
        {
            var emptyIcon = slot.transform.Find("InventoryBG")?.GetComponent<Image>();
            var itemIcon = slot.transform.Find("ItemIcon")?.gameObject;
            var quantityTextBG = slot.transform.Find("ItemIcon/QuantityTextBG")?.gameObject;
            var quantityText = slot.transform.Find("ItemIcon/QuantityTextBG/QuantityText")?.GetComponent<TextMeshProUGUI>();

            if (emptyIcon != null && emptyIcon.gameObject.activeSelf) // Zoek een leeg slot
            {
                emptyIcon.gameObject.SetActive(false); // Verberg leeg slot
                itemIcon?.SetActive(true); // Toon item-icoon

                var iconImage = itemIcon.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = itemSprite; // Stel sprite in
                    Debug.Log($"Set sprite to: {itemSprite.name}");
                }

                if (quantityTextBG != null)
                {
                    quantityTextBG.SetActive(true); // Toon de achtergrond
                }

                if (quantityText != null)
                {
                    quantityText.text = quantity.ToString(); // Stel hoeveelheid in
                    Debug.Log($"Set quantity to: {quantity}");
                }
                return; // Stop hier na het vullen van één slot
            }
        }

        Debug.LogWarning("No empty slots available!");
    }
}