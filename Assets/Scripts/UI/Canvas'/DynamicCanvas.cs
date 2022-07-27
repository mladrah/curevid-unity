using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class DynamicCanvas : MonoBehaviour, IDragHandler
{
    [Header("Dynamic Canvas")]
    public bool isDraggable;
    public RectTransform dragRect;
    private const float borderOffset = 10;
    private Canvas canvas;

    public virtual void Awake() {
        canvas = GetComponent<Canvas>();
    }

    public virtual void HidePanel() {
        UIManager.Instance.RemoveInterfaceFromList(this);
    }

    public virtual void ShowPanel() {
        UIManager.Instance.AddInterface(this);
    }

    public void OnDrag(PointerEventData eventData) {
        if (isDraggable) {
            Vector2 newPos = eventData.delta / canvas.scaleFactor;
            dragRect.anchoredPosition += newPos;
            dragRect.anchoredPosition = new Vector2(Mathf.Clamp(dragRect.anchoredPosition.x, 
                                                    -TechnicalManager.CANVAS_SCALER_X / 2 + dragRect.rect.width / 2, 
                                                    TechnicalManager.CANVAS_SCALER_X/2 - dragRect.rect.width/2),
                                                    Mathf.Clamp(dragRect.anchoredPosition.y,
                                                    -TechnicalManager.CANVAS_SCALER_Y / 2 + dragRect.rect.height / 2,
                                                    TechnicalManager.CANVAS_SCALER_Y / 2 - dragRect.rect.height / 2));
        }
    }
}
