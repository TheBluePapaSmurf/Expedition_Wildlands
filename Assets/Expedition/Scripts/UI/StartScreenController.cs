using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreenController : MonoBehaviour
{
    public GameObject uiPanel;

    void Update()
    {
        if (Input.anyKeyDown) // Gebruikt de gekozen KeyCode
        {
            TogglePanel();
            this.gameObject.SetActive(false);
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
