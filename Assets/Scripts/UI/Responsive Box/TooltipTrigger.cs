using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Content Fields
    [Header("Content")]
    public string header;
    [TextArea(3, 10)]
    public string modifiers;
    [TextArea(3, 10)]
    public string description;
    public string duration;
    #endregion

    #region Events
    public void OnPointerEnter(PointerEventData eventData) {
        TooltipManager.Reposition();
        TooltipManager.Show(description, header, modifiers, duration);
    }

    public void OnPointerExit(PointerEventData eventData) {
        TooltipManager.Hide();
    }
    #endregion
}
