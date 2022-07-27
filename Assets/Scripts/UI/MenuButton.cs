using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Components")]
    private Button btn;
    [SerializeField] private TextMeshProUGUI label;
    private RectTransform labelRect;
    [SerializeField] private Image background;
    private RectTransform backgroundRect;
    private LayoutElement layoutElement;

    [Header("Animation")]
    public bool animateLabelOnly;
    private Sequence sequence;
    [SerializeField] private Ease ease;
    [SerializeField] private float duration;
    [SerializeField] private float moveDuration;
    [SerializeField] private float moveOffsetX;
    private float backgroundInitWidth;
    private Vector2 labelInitPos;

    private void Awake() {
        btn = GetComponent<Button>();

        layoutElement = GetComponent<LayoutElement>();
        labelRect = label.GetComponent<RectTransform>();
        labelInitPos = labelRect.anchoredPosition;

        backgroundRect = background.GetComponent<RectTransform>();
        backgroundInitWidth = backgroundRect.rect.width;
    }

    private void Start() {
        OnInteractableChange();
    }

    private void OnEnable() {

        if (animateLabelOnly)
            label.color = Colors.BLUE_2;
    }

    public void OnInteractableChange() {
        if (!btn.interactable) {
            label.color = new Color(label.color.r, label.color.g, label.color.b, 0.5f);
            background.color = new Color(background.color.r, background.color.g, background.color.b, 0.5f);
        } else {
            label.color = new Color(label.color.r, label.color.g, label.color.b, 1f);
            background.color = new Color(background.color.r, background.color.g, background.color.b, 1f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (!btn.interactable)
            return;

        if (animateLabelOnly) {
            label.DOColor(Colors.WHITE, duration);
            return;
        }

        sequence.Kill();
        sequence = DOTween.Sequence();

        sequence.
        Append(backgroundRect.DOScaleX(layoutElement.preferredWidth / 10, duration)).

        Join(background.DOColor(Colors.DEFAULT_GREEN, duration)).

        Join(labelRect.DOAnchorPosX(labelInitPos.x + moveOffsetX, moveDuration)).

        SetEase(ease);
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (!btn.interactable)
            return;

        if (animateLabelOnly) {
            label.DOColor(Colors.BLUE_1, duration);
            return;
        }

        sequence.Kill();
        sequence = DOTween.Sequence();

        sequence.
        Append(backgroundRect.DOScaleX(backgroundInitWidth / 10, duration)).
        Join(background.DOColor(Colors.WHITE, duration)).

        Join(label.DOColor(Colors.WHITE, duration)).

        Join(labelRect.DOAnchorPosX(labelInitPos.x, moveDuration)).

        SetEase(ease);
    }
}
