using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingWrapper : MonoBehaviour
{
    private SaveSystem saveSystem;

    private void Awake()
    {
        saveSystem = FindObjectOfType<SaveSystem>();
        if (saveSystem == null)
        {
            Debug.LogError("SaveSystem component not found in the scene.");
        }
    }

    public void SaveGameState()
    {
        if (saveSystem != null)
        {
            saveSystem.SaveGame();
        }
        else
        {
            Debug.LogError("SaveSystem is not set. Cannot save game state.");
        }
    }

    public void LoadGameState()
    {
        if (saveSystem != null)
        {
            saveSystem.LoadGame();
        }
        else
        {
            Debug.LogError("SaveSystem is not set. Cannot load game state.");
        }
    }

    public void StartNewGame()
    {
        if (saveSystem != null)
        {
            saveSystem.StartNewGame();
        }
        else
        {
            Debug.LogError("SaveSystem is not set. Cannot start new game.");
        }
    }
}
