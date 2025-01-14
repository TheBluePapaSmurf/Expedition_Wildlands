using UnityEngine;
using System.Collections.Generic;
using static StructureData;

public class GridPlacementWithValidation : MonoBehaviour
{
    public StructureData structureData; // Verwijzing naar het StructureData ScriptableObject
    public LayerMask gridLayer; // Laag voor het grid
    public float gridSize = 1f; // Grootte van de gridcellen
    public Transform placedObjectsParent; // Parent object voor geplaatste objecten

    private GameObject previewObject; // Het preview-object
    private bool isValidPlacement = true; // Controle of de plek beschikbaar is
    private int selectedStructureIndex = 0; // Huidig geselecteerde structuur

    void Start()
    {
        // Instantieer het eerste preview-object
        SetPreviewObject();
    }

    void Update()
    {
        UpdatePreviewPosition();

        // Plaats object als de speler klikt en de positie geldig is
        if (Input.GetMouseButtonDown(0) && isValidPlacement)
        {
            PlaceObject();
        }

        // Wissel tussen structuren met toetsen 1, 2, etc. (optioneel)
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectStructure(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectStructure(1);
    }

    void SetPreviewObject()
    {
        if (previewObject != null)
        {
            Destroy(previewObject); // Verwijder het oude preview-object
        }

        // Instantieer het nieuwe preview-object
        previewObject = Instantiate(structureData.structures[selectedStructureIndex].previewPrefab);
        MakeObjectPreview(previewObject);
    }

    void SelectStructure(int index)
    {
        if (index >= 0 && index < structureData.structures.Count)
        {
            selectedStructureIndex = index;
            SetPreviewObject(); // Update het preview-object
        }
    }

    void UpdatePreviewPosition()
    {
        // Raycast om de gridpositie te bepalen
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, gridLayer))
        {
            // Snap de positie naar het grid
            Vector3 snappedPosition = SnapToGrid(hit.point);
            previewObject.transform.position = snappedPosition;

            // Controleer of de positie al is bezet via GridData
            isValidPlacement = !FindObjectOfType<GridDataPlacement>().IsPositionOccupied(snappedPosition);

            // Pas de kleur van het preview-object aan op basis van validiteit
            SetPreviewColor(isValidPlacement ? Color.green : Color.red);
        }
    }

    void PlaceObject()
    {
        Vector3 position = previewObject.transform.position;

        // Voeg de positie toe aan GridData
        FindObjectOfType<GridDataPlacement>().AddPosition(position);

        // Instantieer het geselecteerde object op de gesnapte positie
        var selectedStructure = structureData.structures[selectedStructureIndex];
        GameObject placedObject = Instantiate(selectedStructure.prefab, position, Quaternion.identity);

        // Stel het geplaatste object in als child van het parent-object
        if (placedObjectsParent != null)
        {
            placedObject.transform.SetParent(placedObjectsParent);
        }

        // Controleer het type structuur (optionele logica)
        if (selectedStructure.structureType == StructureType.Floor)
        {
            Debug.Log("Een vloer is geplaatst!");
        }
        else if (selectedStructure.structureType == StructureType.Wall)
        {
            Debug.Log("Een muur is geplaatst!");
        }
    }


    Vector3 SnapToGrid(Vector3 position)
    {
        float x = Mathf.Round(position.x / gridSize) * gridSize;
        float y = Mathf.Round(position.y / gridSize) * gridSize; // Handhaaf de hoogte
        float z = Mathf.Round(position.z / gridSize) * gridSize;
        return new Vector3(x, y, z);
    }

    void MakeObjectPreview(GameObject obj)
    {
        // Schakel physics uit zodat het niet interfereert met de gameplay
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
    }

    void SetPreviewColor(Color color)
    {
        MeshRenderer[] renderers = previewObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.material.color = color;
        }
    }
}
