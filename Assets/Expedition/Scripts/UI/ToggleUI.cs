using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToggleUI : MonoBehaviour
{
    [System.Serializable]
    public class UIPanel
    {
        public GameObject panel; // Het UI-paneel dat je wilt togglen
        public KeyCode toggleKey; // De toets om het paneel te togglen
        public List<GameObject> relatedObjects; // Gerelateerde GameObjects
        public UnityEvent onToggleOn; // Acties bij het openen/activeren
        public UnityEvent onToggleOff; // Acties bij het sluiten/deactiveren
        public bool isToggled = false; // Houdt de toggle-status bij
    }

    public List<UIPanel> uiPanels = new List<UIPanel>();

    void Update()
    {
        foreach (var uiPanel in uiPanels)
        {
            if (Input.GetKeyDown(uiPanel.toggleKey))
            {
                TogglePanelAndObjects(uiPanel);
            }
        }
    }

    void TogglePanelAndObjects(UIPanel uiPanel)
    {
        if (uiPanel.panel != null)
        {
            // Wissel de actieve status van het paneel
            uiPanel.isToggled = !uiPanel.isToggled;
            uiPanel.panel.SetActive(uiPanel.isToggled);

            // Voer de juiste UnityEvent uit
            if (uiPanel.isToggled)
            {
                uiPanel.onToggleOn?.Invoke(); // Bij openen/activeren
            }
            else
            {
                uiPanel.onToggleOff?.Invoke(); // Bij sluiten/deactiveren
            }

            // Toggle de gerelateerde GameObjects
            if (uiPanel.relatedObjects != null)
            {
                foreach (var obj in uiPanel.relatedObjects)
                {
                    if (obj != null)
                    {
                        // Wissel de actieve status van elk gerelateerd object
                        obj.SetActive(!obj.activeSelf);
                    }
                }
            }
        }
    }

}
