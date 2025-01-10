using UnityEngine;

public class UniqueIdentifier : MonoBehaviour
{
    [SerializeField]  // Maak de uniqueId zichtbaar en bewerkbaar in de Unity Editor
    private string uniqueId;

    void Awake()
    {
        // Alleen een nieuwe ID genereren als er nog geen ID is
        if (string.IsNullOrEmpty(uniqueId))
        {
            uniqueId = System.Guid.NewGuid().ToString();
            Debug.LogWarning("No Unique ID provided. Generated a new one: " + uniqueId);
        }
        SaveUniqueId();
    }

    private void SaveUniqueId()
    {
        // Deze methode kan behouden blijven voor het opslaan van de ID in een bestand of database
        ES3.Save(gameObject.name + "_uniqueId", uniqueId, "playerData.es3");
    }

    public string GetUniqueId()
    {
        return uniqueId;
    }

#if UNITY_EDITOR
    void Reset()
    {
        // Zorg ervoor dat een nieuwe ID wordt gegenereerd wanneer het script in de editor wordt gereset
        uniqueId = System.Guid.NewGuid().ToString();
        Debug.Log("Reset UniqueIdentifier, new ID: " + uniqueId);
    }
#endif
}
