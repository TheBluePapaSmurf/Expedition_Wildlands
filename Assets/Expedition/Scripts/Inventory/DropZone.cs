using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            // Controleer of het gedropte object een DraggableItem is
            DraggableItem draggable = eventData.pointerDrag.GetComponent<DraggableItem>();
            if (draggable != null)
            {
                // Zet het gedropte item in deze slot
                draggable.transform.SetParent(transform);
                draggable.transform.localPosition = Vector3.zero;
                Debug.Log($"Item dropped in slot {name}");
            }
        }
    }
}
