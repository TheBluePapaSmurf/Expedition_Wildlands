using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas parentCanvas;
    private Transform originalParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent; // Sla de originele parent op
        canvasGroup.blocksRaycasts = false; // Zorg dat het object niet de raycast blokkeert
        transform.SetParent(parentCanvas.transform); // Zet het item tijdelijk op de canvas root
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor; // Update positie
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; // Zet raycasts weer aan

        // Controleer of het item op een valide dropzone is gedropt
        if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag("InventorySlot"))
        {
            // Verplaats het item naar de nieuwe slot
            transform.SetParent(eventData.pointerEnter.transform);
            rectTransform.anchoredPosition = Vector2.zero; // Reset de positie binnen de nieuwe slot
        }
        else
        {
            // Als het niet op een slot is gedropt, zet het terug naar de originele parent
            transform.SetParent(originalParent);
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}
