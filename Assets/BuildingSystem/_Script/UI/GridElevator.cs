using UnityEngine;
using UnityEngine.UI;

public class GridElevator : MonoBehaviour
{
    public GameObject targetObject; // Het object dat je wilt verplaatsen
    public float stepSize = 2f; // Hoeveelheid om te verhogen/verlagen

    public Button increaseButton; // Knop om het object te verhogen
    public Button decreaseButton; // Knop om het object te verlagen

    public StructurePlacer structurePlacer; // Referentie naar StructurePlacer

    public int currentIndex = 0; // Standaard index

    private int maxIndex = 2;

    private void Start()
    {
        if (structurePlacer != null)
        {
            maxIndex = structurePlacer.transform.childCount - 1; // Aantal child GameObjects
        }
        // Controleer of de knoppen en het doelobject zijn ingesteld
        if (increaseButton != null)
            increaseButton.onClick.AddListener(IncreasePosition);

        if (decreaseButton != null)
            decreaseButton.onClick.AddListener(DecreasePosition);
    }

    public void SetCurrentIndex(int index)
    {
        currentIndex = index; // Stel de index in vanuit een UI of ander script
    }

    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    public void IncreasePosition()
    {
        if (currentIndex < maxIndex)
        {
            Vector3 newPosition = targetObject.transform.position;
            newPosition.y += stepSize;
            targetObject.transform.position = newPosition;

            if (structurePlacer != null)
            {
                structurePlacer.IncreaseHeight(stepSize);
            }

            currentIndex++;
            Debug.Log($"Current Index Increased: {currentIndex}");
        }
    }

    public void DecreasePosition()
    {
        if (currentIndex > 0)
        {
            Vector3 newPosition = targetObject.transform.position;
            newPosition.y -= stepSize;
            targetObject.transform.position = newPosition;

            if (structurePlacer != null)
            {
                structurePlacer.DecreaseHeight(stepSize);
            }

            currentIndex--;
            Debug.Log($"Current Index Decreased: {currentIndex}");
        }
    }
}
