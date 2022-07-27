using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TooltipManager : MonoBehaviour
{
    #region Game Object Fields
    private static TooltipManager instance;
    [Header("Content Box")]
    public Tooltip tooltip;
    #endregion

    #region Animation Fields
    [Header("Animation")]
    [SerializeField] private float fadeDuration;
    private Sequence fadeSequence;
    private float imageAlphaValue;
    #endregion 

    private void Awake() {
        instance = this;
        fadeSequence = DOTween.Sequence();
        imageAlphaValue = tooltip.GetComponent<Image>().color.a;
    }

    private void Start() {
        tooltip.gameObject.SetActive(false);
    }

    #region DynamicContentBox Behaviour
    public static void Show(string description = "", string header = "", string modifier = "", string duration = "") {
        instance.tooltip.SetText(description, header, modifier, duration);

        DoTooltipInvisible();

        instance.tooltip.gameObject.SetActive(true);
        instance.tooltip.isVisible = true;

        DoTooltipVisible();
    }

    public static void Hide() {
        instance.tooltip.gameObject.SetActive(false);
        instance.fadeSequence.Kill();
        instance.tooltip.isVisible = false;
    }

    public static void Reposition() {
        instance.tooltip.TrackMouse();
    }
    #endregion

    #region Animation
    private static void DoTooltipInvisible() {
        instance.tooltip.GetComponent<Image>().color = ChangeAlpha(instance.tooltip.GetComponent<Image>().color, 0);
        instance.tooltip.headerText.color = ChangeAlpha(instance.tooltip.headerText.color, 0);
        instance.tooltip.descriptionText.color = ChangeAlpha(instance.tooltip.descriptionText.color, 0);
        instance.tooltip.modifierText.color = ChangeAlpha(instance.tooltip.modifierText.color, 0);
        instance.tooltip.durationText.color = ChangeAlpha(instance.tooltip.durationText.color, 0);
    }

    private static void DoTooltipVisible() {
        instance.fadeSequence.
            Join(instance.tooltip.GetComponent<Image>().DOFade(instance.imageAlphaValue, instance.fadeDuration)).
            Join(instance.tooltip.headerText.DOFade(1, instance.fadeDuration)).
            Join(instance.tooltip.descriptionText.DOFade(1, instance.fadeDuration)).
            Join(instance.tooltip.modifierText.DOFade(1, instance.fadeDuration)).
            Join(instance.tooltip.durationText.DOFade(1, instance.fadeDuration));
    }

    private static Color ChangeAlpha(Color col, float value) {
        var tempColor = col;
        tempColor.a = value;
        return tempColor;
    }
    #endregion
}
