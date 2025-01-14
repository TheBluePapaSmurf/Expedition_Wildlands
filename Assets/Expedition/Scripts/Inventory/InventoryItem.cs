using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    [SerializeField]
    private string id;

    public string ID => id; // Eigenschap om de ID te verkrijgen
    public string ItemName; // Naam van het item
    public bool isStackable; // Nieuw: Geeft aan of het item stackable is
    public Sprite Icon; // Optioneel: icon voor het item

    private void OnValidate()
    {
        // Controleer of er al een ID bestaat
        if (string.IsNullOrEmpty(id))
        {
            // Genereer een nieuwe unieke ID
            id = Guid.NewGuid().ToString();
            Debug.Log($"Generated new ID for {name}: {id}");
        }
    }
}
