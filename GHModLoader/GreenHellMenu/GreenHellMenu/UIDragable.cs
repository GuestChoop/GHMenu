using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragable : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public static Canvas canvas;
    public static RectTransform rect;
    bool isDrag;

    public void OnDrag(PointerEventData eventData)
    {
        if (isDrag)
        {
            rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDrag = true;
        GreenHellMenu.Instance.canvas.alpha = 0.6f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDrag = false;
        GreenHellMenu.Instance.canvas.alpha = 1;
    }
}
