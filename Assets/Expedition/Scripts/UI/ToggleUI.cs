using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleUI : MonoBehaviour
{
    public GameObject uiPanel; // Referentie naar het UI Paneel dat je wilt toggle
    public KeyCode toggleKey = KeyCode.Escape;

    void Update()
    {
        if (Input.GetKeyDown(toggleKey)) // Gebruikt de gekozen KeyCode
        {
            TogglePanel();
        }
    }

    void TogglePanel()
    {
        if (uiPanel != null)
        {
            bool isActive = !uiPanel.activeSelf;
            uiPanel.SetActive(isActive); // Zet het paneel aan of uit
        }
    }
}
