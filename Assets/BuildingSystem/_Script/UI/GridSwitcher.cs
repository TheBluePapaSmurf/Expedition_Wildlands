using UnityEngine;

public class GridSwitcher : MonoBehaviour
{
    [SerializeField]
    private GridManager gridManager;

    public void SwitchToGrid(int gridIndex)
    {
        gridManager.SwitchGrid(gridIndex);
        Debug.Log($"Switched to grid {gridIndex}");
    }
}
