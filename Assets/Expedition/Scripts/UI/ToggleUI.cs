using System.Collections.Generic;
using UnityEngine;

public class ToggleUI : MonoBehaviour
{
    [System.Serializable]
    public class UIPanel
    {
        public GameObject panel; // Het UI-paneel dat je wilt togglen
        public KeyCode toggleKey; // De toets om het paneel te togglen
        public List<GameObject> relatedObjects; // Gerelateerde GameObjects
    }

    public List<UIPanel> uiPanels = new List<UIPanel>(); // Lijst van UI-panelen en toetsen

    void Update()
    {
        // Loop door alle UI-panelen in de lijst
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
            // Toggle het paneel
            bool isPanelActive = !uiPanel.panel.activeSelf;
            uiPanel.panel.SetActive(isPanelActive);

            // Toggle de gerelateerde GameObjects
            if (uiPanel.relatedObjects != null)
            {
                foreach (var obj in uiPanel.relatedObjects)
                {
                    if (obj != null)
                    {
                        // Wissel de actieve status van elk gerelateerd object
                        bool isObjectActive = obj.activeSelf;
                        obj.SetActive(!isObjectActive);
                    }
                }
            }
        }
    }
}
