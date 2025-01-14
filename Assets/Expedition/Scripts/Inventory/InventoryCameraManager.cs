using Cinemachine;
using UnityEngine;

public class InventoryCameraManager : MonoBehaviour
{
    [Header("Cinemachine Virtual Cameras")]
    public CinemachineVirtualCamera playerVirtualCamera;  // Virtual Camera van de speler
    public CinemachineVirtualCamera inventoryVirtualCamera; // Virtual Camera voor de inventory

    [Header("Inventory UI Reference")]
    public GameObject inventoryPanel;  // Verwijzing naar de inventory UI

    private bool isInventoryOpen = false;

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        // Activeer of deactiveer de inventory UI
        inventoryPanel.SetActive(isInventoryOpen);

        // Wissel de priority van de Virtual Cameras
        if (isInventoryOpen)
        {
            playerVirtualCamera.Priority = 5; // Lager maken zodat de inventory camera wordt geactiveerd
            inventoryVirtualCamera.Priority = 10; // Hoger maken zodat deze de controle overneemt
        }
        else
        {
            playerVirtualCamera.Priority = 10; // Speler camera terug activeren
            inventoryVirtualCamera.Priority = 5; // Inventory camera uitschakelen
        }
    }
}