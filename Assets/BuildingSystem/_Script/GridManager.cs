using System;
using UnityEngine;

/// <summary>
/// Helper class that connects the Grid component and Grid shader, and allows other scripts to access the data from the grid.
/// </summary>
public class GridManager : MonoBehaviour
{
    [SerializeField]
    private Grid grid;
    [SerializeField]
    private Renderer gridRenderer;

    [SerializeField]
    private Vector3 gridCellSize;
    private Vector3 halfGridCellSize;

    [SerializeField]
    private Vector2Int defaultScale = new Vector2Int(10, 10);

    public Vector2Int GridSize =>
        Vector2Int.RoundToInt(
            defaultScale *
            new Vector2(
                gridRenderer.transform.localScale.x,
                gridRenderer.transform.localScale.z)
        );

    [SerializeField]
    private string cellSizeParameter = "_GridSize", defaultScaleParameter = "_DefaultScale";

    private void OnEnable()
    {
        Debug.Log("GridManager enabled");

        // Ensure grid settings are initialized
        if (grid == null)
        {
            Debug.LogError("Grid component is missing!");
            return;
        }

        grid.cellSize = gridCellSize;
        halfGridCellSize = gridCellSize / 2f;

        if (gridRenderer != null && gridRenderer.material != null)
        {
            gridRenderer.material.SetVector(cellSizeParameter, new Vector2(1 / gridCellSize.x, 1 / gridCellSize.z));
            gridRenderer.material.SetVector(defaultScaleParameter, new Vector2(defaultScale.x, defaultScale.y));
        }
    }

    private void OnDisable()
    {
        Debug.Log("GridManager disabled");
        // Optionally clean up gridRenderer or material properties if needed
    }

    public Vector3Int GetCellPosition(Vector3 worldPosition, PlacementType placementType)
    {
        if (placementType.IsEdgePlacement())
        {
            worldPosition += halfGridCellSize;
        }
        return grid.WorldToCell(worldPosition);
    }

    public Vector3 GetWorldPosition(Vector3Int cellPosition)
    {
        return grid.CellToWorld(cellPosition);
    }

    public Vector3 GetCenterPositionForCell(Vector3Int cellPosition)
    {
        return GetWorldPosition(cellPosition) + halfGridCellSize;
    }

    public void ToggleGrid(bool value)
    {
        if (gridRenderer != null)
        {
            gridRenderer.gameObject.SetActive(value);
        }
    }
}

/// <summary>
/// Placement types define where and how objects can be placed on the grid.
/// </summary>
public enum PlacementType
{
    None,
    Floor,
    Wall,
    InWalls,
    NearWallObject,
    FreePlacedObject
}

/// <summary>
/// Extension methods for the PlacementType enum to add behavior.
/// </summary>
public static class PlacementTypeExtensions
{
    public static bool IsEdgePlacement(this PlacementType placementType)
    {
        return placementType == PlacementType.Wall || placementType == PlacementType.InWalls;
    }

    public static bool IsObjectPlacement(this PlacementType placementType)
    {
        return placementType == PlacementType.FreePlacedObject || placementType == PlacementType.NearWallObject;
    }
}
