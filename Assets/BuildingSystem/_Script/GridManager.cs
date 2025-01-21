using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

/// <summary>
/// Helper class that connects the Grid component and Grid shader and allows other scripts to access the data from the grid
/// </summary>
public class GridManager : MonoBehaviour
{
    [System.Serializable]
    public class GridData
    {
        public Grid grid;
        public Renderer gridRenderer;
        public Vector3 cellSize;
    }

    [SerializeField]
    private Vector3 gridCellSize;
    private Vector3 halfGridCellSize;

    [SerializeField]
    private Vector2Int defaultScale = new Vector2Int(10, 10);

    public Vector2Int GetGridSize(int gridIndex)
    {
        if (gridIndex < 0 || gridIndex >= grids.Count)
            throw new System.IndexOutOfRangeException("Invalid grid index.");

        var gridData = grids[gridIndex];
        return Vector2Int.RoundToInt(
            defaultScale *
            new Vector2(
                gridData.gridRenderer.transform.localScale.x,
                gridData.gridRenderer.transform.localScale.z)
            );
    }



    [SerializeField]
    private string cellSizeParameter = "_GridSize", defaultScaleParameter = "_DefaultScale";

    [SerializeField]
    private List<GridData> grids = new List<GridData>();

    private Dictionary<Grid, Vector3> halfGridCellSizeMap = new();

    private int activeGridIndex = 0;

    [SerializeField]
    private InputManager inputManager;

    private void Start()
    {
        InitializeGrids();

        inputManager.OnSwitchGrid += HandleSwitchGrid;
    }

    /// <summary>
    /// Initializes the grids and calculates half cell sizes for each grid.
    /// </summary>
    private void InitializeGrids()
    {
        foreach (var gridData in grids)
        {
            gridData.grid.cellSize = gridData.cellSize;
            halfGridCellSizeMap[gridData.grid] = gridData.cellSize / 2f;

            gridData.gridRenderer.material.SetVector("_GridSize", new Vector2(1 / gridData.cellSize.x, 1 / gridData.cellSize.z));
        }
    }

    public void SwitchGrid(int newGridIndex)
    {
        // Controleer of de nieuwe index binnen de grenzen ligt
        if (newGridIndex < 0)
        {
            Debug.LogWarning("Je hebt het laagste grid bereikt.");
            return;
        }

        if (newGridIndex >= grids.Count)
        {
            Debug.LogWarning("Je hebt het hoogste grid bereikt.");
            return;
        }

        // Schakel de huidige grid uit
        ToggleGrid(activeGridIndex, false);

        // Schakel de nieuwe grid in
        activeGridIndex = newGridIndex;
        ToggleGrid(activeGridIndex, true);

        Debug.Log($"Switched to grid {activeGridIndex}");
    }


    private void HandleSwitchGrid(int gridIndex)
    {
        SwitchGrid(gridIndex);
        Debug.Log($"Switched to grid {gridIndex}");
    }

    /// <summary>
    /// Adds a new grid dynamically to the manager.
    /// </summary>
    public void AddGrid(Grid grid, Renderer gridRenderer, Vector3 cellSize)
    {
        grids.Add(new GridData
        {
            grid = grid,
            gridRenderer = gridRenderer,
            cellSize = cellSize
        });
        grid.cellSize = cellSize;
        halfGridCellSizeMap[grid] = cellSize / 2f;

        gridRenderer.material.SetVector("_GridSize", new Vector2(1 / cellSize.x, 1 / cellSize.z));
    }

    /// <summary>
    /// Toggles visibility of all grids off.
    /// </summary>
    public void HideAllGrids()
    {
        foreach (var gridData in grids)
        {
            gridData.gridRenderer.gameObject.SetActive(false);
        }
    }

    public int GetGridCount()
    {
        return grids.Count;
    }


    /// <summary>
    /// Toggles visibility of a specific grid.
    /// </summary>
    public void ToggleGrid(int index, bool value)
    {
        if (index < 0 || index >= grids.Count)
            return;

        grids[index].gridRenderer.gameObject.SetActive(value);
    }

    /// <summary>
    /// Gets the grid at the specified index.
    /// </summary>
    public Grid GetGrid(int index)
    {
        if (index < 0 || index >= grids.Count)
            throw new System.IndexOutOfRangeException("Invalid grid index.");

        return grids[index].grid;
    }

    /// <summary>
    /// Gets the cell position on a specific grid based on world position.
    /// </summary>
    public Vector3Int GetCellPosition(Grid grid, Vector3 worldPosition, PlacementType placementType)
    {
        if (!halfGridCellSizeMap.ContainsKey(grid))
            throw new System.ArgumentException("Grid is not managed by this GridManager.");

        if (placementType.IsEdgePlacement())
            worldPosition += halfGridCellSizeMap[grid];

        return grid.WorldToCell(worldPosition);
    }

    /// <summary>
    /// Gets the world position of a cell on a specific grid.
    /// </summary>
    public Vector3 GetWorldPosition(Grid grid, Vector3Int cellPosition)
    {
        return grid.CellToWorld(cellPosition);
    }

    /// <summary>
    /// Gets the center position of a cell on a specific grid.
    /// </summary>
    public Vector3 GetCenterPositionForCell(Grid grid, Vector3Int cellPosition)
    {
        return GetWorldPosition(grid, cellPosition) + halfGridCellSizeMap[grid];
    }
}

/// <summary>
/// Placement types. A better idea would be to try to create objects from it ex using ScriptableObjects.
/// Still enum works well for a prototype.
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
/// Because of the limitation of using enum the end result is that you need extensions methods
/// since you can't easily add more data to an enum. This way I can reliably access the additional data
/// without having to check each if / switch statement where I have used the enum.
/// </summary>
public static class PlacementTypeExtensions
{
    public static bool IsEdgePlacement(this PlacementType placementType)
    => placementType switch
        {
            PlacementType.Wall => true,
            PlacementType.InWalls => true,
            _ => false
        };
    public static bool IsObjectPlacement(this PlacementType placementType)
    => placementType switch
    {
        PlacementType.FreePlacedObject => true,
        PlacementType.NearWallObject => true,
        _ => false
    };
}