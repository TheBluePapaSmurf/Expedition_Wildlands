using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Xml;
using BDE.Expedition.PlayerControls;

public class SaveSystem : MonoBehaviour
{
    [Header("SaveSystem mag alleen in de Main menu geplaatst worden.")]

    public GameObject playerPrefab;
    public string saveFileName = "playerData.es3";
    public int sceneIndex = 1;

    private static bool created = false;

    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void StartNewGame()
    {
        if (ES3.FileExists(saveFileName))
        {
            ES3.DeleteFile(saveFileName);
            Debug.Log("Save file deleted, starting new game");
        }
        else
        {
            Debug.Log("No save file to delete, starting new game");
        }

        // Controleer of er al een Player in de scene is
        if (GameObject.FindWithTag("Player") == null)
        {
            // Geen Player gevonden, instantieer een nieuwe
            Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("No Player was found in the scene. A new Player has been instantiated.");
        }

        SceneManager.LoadScene(sceneIndex); // Laad de startscene of een specifieke nieuwe spel scene
    }

    public void SaveGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        GameObject playerObject = GameObject.Find("Player");

        if (playerObject != null)
        {
            PlayerMovement playerMovement = playerObject.GetComponent<PlayerMovement>();
            PlayerHealth playerHealth = playerObject.GetComponent<PlayerHealth>();
            CoinCollector coinCollector = playerObject.GetComponent<CoinCollector>();

            // Save coin data
            if (coinCollector != null)
            {
                ES3.Save("coins", coinCollector.GetCoinCount(), saveFileName);
                Debug.Log($"Coins saved: {coinCollector.GetCoinCount()}");
            }

            if (playerMovement != null && playerHealth != null)
            {
                ES3.Save("sceneIndex", currentSceneIndex, saveFileName);
                ES3.Save("position", playerMovement.transform.position, saveFileName);
                playerHealth.SaveHealth(); // Opslaan van gezondheid
                Debug.Log("Game saved with player position, scene index, health, and coins.");
            }
            else
            {
                Debug.LogError("PlayerMovement or PlayerHealth component not found on the Player object.");
            }
        }
        else
        {
            Debug.LogError("No GameObject named 'Player' found.");
        }
    }

    // Laad de spelerpositie na het asynchroon laden van de opgeslagen scene
    public void LoadGame()
    {
        if (ES3.FileExists(saveFileName) && ES3.KeyExists("sceneIndex", saveFileName))
        {
            int savedSceneIndex = ES3.Load<int>("sceneIndex", saveFileName);
            StartCoroutine(LoadGameCoroutine(savedSceneIndex));
        }
        else
        {
            Debug.LogError("No saved scene data found.");
        }
    }

    private IEnumerator LoadGameCoroutine(int sceneIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);  // Wacht even om zeker te zijn dat alles geladen is.

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject == null)
        {
            playerObject = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            Debug.Log("No Player found, instantiating new Player.");
        }

        if (ES3.FileExists(saveFileName) && ES3.KeyExists("position", saveFileName))
        {
            Vector3 loadedPosition = ES3.Load<Vector3>("position", saveFileName);
            playerObject.transform.position = loadedPosition;
            Debug.Log($"Player position set to: {loadedPosition}");
        }

        PlayerHealth playerHealth = playerObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.LoadHealth();
        }
        else
        {
            Debug.LogError("PlayerHealth component not found on the Player object.");
        }

        // Load coin data
        CoinCollector coinCollector = playerObject.GetComponent<CoinCollector>();
        if (coinCollector != null)
        {
            if (ES3.KeyExists("coins", saveFileName))
            {
                int savedCoins = ES3.Load<int>("coins", saveFileName);
                coinCollector.LoadCoins(savedCoins);
                Debug.Log($"Coins loaded: {savedCoins}");
            }
        }
        else
        {
            Debug.LogError("CoinCollector component not found on the Player object.");
        }
    }
}
