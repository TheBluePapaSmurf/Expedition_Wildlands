using UnityEngine;
using UnityEngine.UI;

public class GridElevator : MonoBehaviour
{
    public GameObject targetObject; // Het object dat je wilt verplaatsen
    public float stepSize = 2f; // Hoeveelheid om te verhogen/verlagen

    public Button increaseButton; // Knop om het object te verhogen
    public Button decreaseButton; // Knop om het object te verlagen

    public StructurePlacer structurePlacer; // Referentie naar StructurePlacer

    private void Start()
    {
        // Controleer of de knoppen en het doelobject zijn ingesteld
        if (increaseButton != null)
            increaseButton.onClick.AddListener(IncreasePosition);

        if (decreaseButton != null)
            decreaseButton.onClick.AddListener(DecreasePosition);
    }

    public void IncreasePosition()
    {
        if (targetObject != null)
        {
            Vector3 newPosition = targetObject.transform.position;
            newPosition.y += stepSize;
            targetObject.transform.position = newPosition;

            // Update de hoogte in StructurePlacer
            if (structurePlacer != null)
            {
                structurePlacer.IncreaseHeight(stepSize);
            }
        }
    }

    public void DecreasePosition()
    {
        if (targetObject != null)
        {
            Vector3 newPosition = targetObject.transform.position;
            newPosition.y -= stepSize;
            targetObject.transform.position = newPosition;

            // Update de hoogte in StructurePlacer
            if (structurePlacer != null)
            {
                structurePlacer.DecreaseHeight(stepSize);
            }
        }
    }

}
