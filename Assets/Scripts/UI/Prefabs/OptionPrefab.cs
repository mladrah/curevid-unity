using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class OptionPrefab : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Game.Event _event;
    private Option _reference;
    [SerializeField] private TextMeshProUGUI label;
    private TooltipTrigger ttTrigger;

    private void Awake() {
        ttTrigger = GetComponent<TooltipTrigger>();
    }

    public void Clone(Option reference, Game.Event e) {
        _reference = reference;
        _event = e;

        label.text = _reference.description;
        ttTrigger.modifiers = "";
        ttTrigger.modifiers += "Modifier(s) will be applied:\n";
        ttTrigger.modifiers += Format.Modifiers(_reference.modifiers);
        ttTrigger.duration = "Duration: " + _event.Duration + " days";
    }

    public void OnPointerClick(PointerEventData eventData) {
        _event.ApplyEvent(_reference.modifiers);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        label.DOColor(Colors.HexToColor(Colors.HEX_WHITE), 0.1f);
    }

    public void OnPointerExit(PointerEventData eventData) {
        label.DOColor(Colors.HexToColor(Colors.HEX_BLUE_2), 0.1f);
    }
}
