using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Allows to place Wall objects on the map
/// </summary>
public class WallPlacementStrategy : SelectionStrategy
{
    protected Vector3Int? startposition;
    protected PlacementGridData objectPlacementData, inWallPlacementData;
    public WallPlacementStrategy(PlacementGridData wallPlacementData, PlacementGridData inWallPlacementData, PlacementGridData objectPlacementData, GridManager gridManager) : base(wallPlacementData, gridManager)
    {
        this.objectPlacementData = objectPlacementData;
        this.inWallPlacementData = inWallPlacementData;
    }

    public override void StartSelection(Vector3 mousePosition, SelectionData selectionData)
    {
        Grid activeGrid = gridManager.GetGrid(0); // Haal de actieve grid op

        selectionData.Clear();
        startposition = gridManager.GetCellPosition(activeGrid, mousePosition, selectionData.PlacedItemData.objectPlacementType);
        selectionData.AddToWorldPositions(gridManager.GetWorldPosition(activeGrid, startposition.Value));

        selectionData.PlacementValidity = true;

        lastDetectedPosition.TryUpdatingPositon(startposition.Value);
        Debug.Log($"Selection {lastDetectedPosition.GetPosition()}");
        if (selectionData.PlacementValidity == false)
            startposition = null;
    }

    public override bool ModifySelection(Vector3 mousePosition, SelectionData selectionData)
    {
        Grid activeGrid = gridManager.GetGrid(0); // Haal de actieve grid op

        Vector3Int tempPos = gridManager.GetCellPosition(activeGrid, mousePosition, selectionData.PlacedItemData.objectPlacementType);
        if (lastDetectedPosition.TryUpdatingPositon(tempPos))
        {
            selectionData.Clear();
            if (startposition.HasValue && startposition.Value != lastDetectedPosition.lastPosition.Value)
            {
                List<Vector3Int> path = GridSelectionHelper.AStar(startposition.Value, tempPos, placementData);

                Vector3 worldPos;
                for (int i = 0; i < path.Count - 1; i++)
                {
                    worldPos = gridManager.GetWorldPosition(activeGrid, path[i]);
                    selectionData.AddToWorldPositions(worldPos);
                    selectionData.AddToPreviewPositions(worldPos);
                    selectionData.AddToGridPositions(path[i]);
                }
                worldPos = gridManager.GetWorldPosition(activeGrid, lastDetectedPosition.lastPosition.Value);
                selectionData.AddToPreviewPositions(worldPos);

                List<Quaternion> rotationValues = GridSelectionHelper.CalculateRotation(path);
                rotationValues.Add(rotationValues[^1]);

                selectionData.SetObjectRotation(rotationValues);
                selectionData.SetGridCheckRotation(rotationValues);
                selectionData.PlacementValidity = ValidatePlacement(selectionData);
            }
            else
            {
                selectionData.AddToWorldPositions(gridManager.GetWorldPosition(activeGrid, lastDetectedPosition.GetPosition()));
                selectionData.PlacementValidity = true;
            }
            return true;
        }
        return false;
    }


    protected override bool ValidatePlacement(SelectionData selectionData)
    {
        //checks if the placement position is valid
        bool validity = PlacementValidator.CheckIfPositionsAreValid(
            selectionData.GetSelectedGridPositions(),
            placementData,
            selectionData.PlacedItemData.size,
            selectionData.GetSelectedPositionsGridRotation(),
            selectionData.PlacedItemData.objectPlacementType.IsEdgePlacement());
        if (validity)
        {
            //Checks if no other wall are at those positions
            validity = PlacementValidator.CheckIfPositionsAreFree(
            selectionData.GetSelectedGridPositions(),
            placementData,
            selectionData.PlacedItemData.size,
            selectionData.GetSelectedPositionsGridRotation(),
            selectionData.PlacedItemData.objectPlacementType.IsEdgePlacement());
        }
        if(validity)
        {
            //Checks if no other IN Wall objects are at those positions
            validity = PlacementValidator.CheckIfPositionsAreFree(
            selectionData.GetSelectedGridPositions(),
            inWallPlacementData,
            selectionData.PlacedItemData.size,
            selectionData.GetSelectedPositionsGridRotation(),
            selectionData.PlacedItemData.objectPlacementType.IsEdgePlacement());
        }
        if(validity)
        {
            //Checks if we are not trying to place wall inside a furniture object
            validity = PlacementValidator.CheckIfNotCrossingMultiCellObject(
                selectionData.GetSelectedGridPositions(),
                objectPlacementData,
                selectionData.PlacedItemData.size,
                selectionData.GetSelectedPositionsGridRotation(),
                selectionData.PlacedItemData.objectPlacementType.IsEdgePlacement());
        }
        return validity;
    }

    public override void FinishSelection(SelectionData selectionData)
    {
        startposition = null;
        lastDetectedPosition.Reset();
    }
}
