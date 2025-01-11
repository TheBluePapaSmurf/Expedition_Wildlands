using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Input manager. Manages user inputs and raises events for interaction.
/// </summary>
public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;

    public event Action OnMousePressed;
    public event Action OnMouseReleased;
    public event Action OnCancel;
    public event Action OnUndo;
    public event Action<int> OnRotate;
    public event Action<bool> OnToggleDelete;

    /// <summary>
    /// Gets the position on the map that the mouse is pointing at.
    /// </summary>
    /// <returns>The selected map position as a Vector3.</returns>
    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 100, placementLayermask))
        {
            lastPosition = hit.point;
        }
        return lastPosition;
    }

    /// <summary>
    /// Checks if the mouse is interacting with a UI element.
    /// </summary>
    /// <returns>True if interacting with UI, otherwise false.</returns>
    public bool IsInteractingWithUI()
        => EventSystem.current.IsPointerOverGameObject();

    private void OnEnable()
    {
        // Optional debugging
        Debug.Log("InputManager enabled");
    }

    private void OnDisable()
    {
        // Clean up if needed (e.g., disconnect custom input systems)
        Debug.Log("InputManager disabled");
    }

    private void Update()
    {
        HandleKeyboardInputs();
        HandleMouseInputs();
    }

    /// <summary>
    /// Handles keyboard inputs.
    /// </summary>
    private void HandleKeyboardInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape key pressed: triggering cancel event.");
            OnCancel?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("R key pressed: triggering undo event.");
            OnUndo?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Comma))
        {
            Debug.Log("Rotate left triggered.");
            OnRotate?.Invoke(-1);
        }

        if (Input.GetKeyDown(KeyCode.Period))
        {
            Debug.Log("Rotate right triggered.");
            OnRotate?.Invoke(1);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Debug.Log("Delete mode enabled.");
            OnToggleDelete?.Invoke(true);
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            Debug.Log("Delete mode disabled.");
            OnToggleDelete?.Invoke(false);
        }
    }

    /// <summary>
    /// Handles mouse inputs.
    /// </summary>
    private void HandleMouseInputs()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse button pressed.");
            OnMousePressed?.Invoke();
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Mouse button released.");
            OnMouseReleased?.Invoke();
        }
    }
}
