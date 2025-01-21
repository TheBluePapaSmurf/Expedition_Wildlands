using CommandSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Command that allows to remove objet and undo the removal if needed
/// </summary>
public class StructureRemoveCommand : ICommand
{
    PlacementManager placementManager;
    PlacementGridData placementData;
    ItemData itemData;
    GridManager gridManager;
    SelectionResult selectionResult, selectionResultToRestore;

    public StructureRemoveCommand(GridManager gridManager, PlacementManager placementManager, PlacementGridData placementData, ItemData itemData, SelectionResult selectionResult)
    {
        this.placementManager = placementManager;
        this.selectionResult = selectionResult;
        this.placementData = placementData;
        this.itemData = itemData;
        this.gridManager = gridManager;
    }

    public bool CanExecute()
    {
        GenerateUndoData();
        //If there is nothing to remove (remove selection selects empty spaces)
        //we don't want to perform this command and add it to a stack of commands to undo
        return selectionResultToRestore.selectedGridPositions.Count > 0;
    }

    public void Execute()
    {
        if(selectionResultToRestore.selectedGridPositions == null)
            GenerateUndoData();

        placementManager.RemoveStructureAt(selectionResult, placementData);

    }

    private void GenerateUndoData()
    {
        Grid activeGrid = gridManager.GetGrid(0); // Haal de actieve grid op

        List<Vector3Int> occupiedCellsGridPositions = new();
        List<Vector3> occupiedCellsPosition = new();
        List<Quaternion> occupiedCellsRotation = new();
        List<Quaternion> occupiedCellsGridCheckRotation = new();

        if (itemData.objectPlacementType.IsEdgePlacement())
        {
            HashSet<Edge> checkedEdges = new HashSet<Edge>();
            foreach (var pos in selectionResult.selectedGridPositions)
            {
                int rotation = Mathf.RoundToInt(selectionResult.selectedPositionGridCheckRotation[0].eulerAngles.y);
                List<Edge> edgesToCheck = this.placementData.GetEdgePositions(pos, itemData.size, rotation);

                foreach (var edge in edgesToCheck)
                {
                    if (checkedEdges.Contains(edge))
                        continue;

                    if (this.placementData.IsEdgeObjectAt(edge))
                    {
                        int savedRotation = (rotation == 0 || rotation == 180) ? 0 : 270;

                        List<Edge> edges = this.placementData.GetEdgesOccupiedForEdgeObject(edge);
                        checkedEdges.AddRange(edges);

                        Vector3Int newOrigin = edges.OrderBy(e => e.smallerPoint.x)
                                                    .ThenBy(e => e.smallerPoint.z)
                                                    .First()
                                                    .smallerPoint;

                        occupiedCellsRotation.Add(Quaternion.Euler(0, savedRotation, 0));
                        occupiedCellsGridCheckRotation.Add(Quaternion.Euler(0, savedRotation, 0));
                        occupiedCellsGridPositions.Add(newOrigin);
                        occupiedCellsPosition.Add(this.gridManager.GetWorldPosition(activeGrid, newOrigin));
                    }
                }
            }
        }
        else
        {
            foreach (var pos in selectionResult.selectedGridPositions)
            {
                List<Vector3Int> cellsToCheck = this.placementData.GetCellPositions(pos, itemData.size, Mathf.RoundToInt(selectionResult.selectedPositionGridCheckRotation[0].eulerAngles.y));

                foreach (var cell in cellsToCheck)
                {
                    if (this.placementData.IsCellObjectAt(cell) && this.selectionResult.selectedGridPositions.Contains(cell))
                    {
                        Vector3Int placementOriginPosition = this.placementData.GetOriginForCellObject(cell).Value;
                        occupiedCellsGridPositions.Add(placementOriginPosition);
                        int index = selectionResult.selectedGridPositions.IndexOf(cell);
                        occupiedCellsPosition.Add(this.gridManager.GetWorldPosition(activeGrid, placementOriginPosition));
                        occupiedCellsRotation.Add(selectionResult.selectedPositionsObjectRotation[index]);
                        occupiedCellsGridCheckRotation.Add(selectionResult.selectedPositionGridCheckRotation[index]);
                    }
                }
            }
        }

        selectionResultToRestore = new SelectionResult
        {
            isEdgeStructure = itemData.objectPlacementType.IsEdgePlacement(),
            placementValidity = true,
            selectedGridPositions = occupiedCellsGridPositions,
            selectedPositions = occupiedCellsPosition,
            selectedPositionGridCheckRotation = occupiedCellsGridCheckRotation,
            selectedPositionsObjectRotation = occupiedCellsRotation
        };
    }


    public void Undo()
    {
        placementManager.PlaceStructureAt(selectionResultToRestore, placementData, itemData);
    }
}
