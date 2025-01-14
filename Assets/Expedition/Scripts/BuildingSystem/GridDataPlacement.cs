using System.Collections.Generic;
using UnityEngine;

public class GridDataPlacement : MonoBehaviour
{
    private HashSet<Vector3> placedPositions = new HashSet<Vector3>();

    // Controleer of een positie al bezet is
    public bool IsPositionOccupied(Vector3 position)
    {
        return placedPositions.Contains(position);
    }

    // Voeg een nieuwe geplaatste positie toe
    public void AddPosition(Vector3 position)
    {
        placedPositions.Add(position);
    }

    // Verwijder een positie (optioneel, bijv. bij verwijderen van objecten)
    public void RemovePosition(Vector3 position)
    {
        placedPositions.Remove(position);
    }
}
