using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class FilterButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private ViewManager.View viewType;
    public ScriptableObject reference;
    public Image panel;
    public Image icon;
    [SerializeField] private KeyCode key;
    private static bool keyPressed;
    public GameObject content;
    [HideInInspector] public Color panelUnselectedCol;

    [Header("Animation")]
    private Tween punchTween;
    [SerializeField] private Color panelSelectedCol;
    private const float colorTweenDuration = 0.1f;
    private const Ease colorTweenEase = Ease.OutSine;
    [SerializeField] private RectTransform subFilterViewRect;
    private const float childsTweenDuration = 0.1f;
    private const Ease childsShowTween = Ease.OutSine;
    private const Ease childsHideTween = Ease.InSine;
    private Vector2 subFilterViewPos;
    private Vector2 subFilterViewHidePos;


    private void Awake() {
        panelUnselectedCol = panel.color;

        subFilterViewPos = subFilterViewRect.anchoredPosition;
        subFilterViewHidePos = new Vector2(subFilterViewRect.anchoredPosition.x, subFilterViewRect.anchoredPosition.y - 20);
        subFilterViewRect.anchoredPosition = subFilterViewHidePos;

        if (reference != null) {
            if(reference as MeasurementSO){
                icon.sprite = ((MeasurementSO)reference).Icon;
            }
        }
    }

    private void Start() {
        HideChildsTween(0);

        if (viewType.Equals(ViewManager.Instance.currentView) && ViewManager.Instance.currentReference == reference) {
            Colorize(panel, panelSelectedCol, 0);
        }
    }

    private void Update() {
        if (UIManager.Instance.inputDisable)
            return;

        if (Input.GetKeyDown(key) && !keyPressed) {
            keyPressed = true;
            ApplyFilter();
        }
        if (Input.GetKeyUp(key)) {
            keyPressed = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        ApplyFilter();
    }

    public Tween ApplyFilter() {
        punchTween.Kill();
        transform.localScale = new Vector2(1, 1);
        punchTween = UIManager.Instance.PunchScale(transform, new Vector2(0.1f, 0.1f), 0.2f);
        ResetAll();
        Tween tween = Colorize(panel, panelSelectedCol, colorTweenDuration);

        if (content.transform.childCount > 0) {
            FilterButton fb = content.transform.GetChild(0).GetComponent<FilterButton>();
            fb.ApplyFilter().
            OnComplete(() => ShowChildsTween(childsTweenDuration));
        } else {
            if (reference != null)
                ViewManager.Instance.FilterBy(viewType, reference);
            else
                ViewManager.Instance.FilterBy(viewType);
        }

        return tween;
    }

    private void ResetAll() {
        foreach (Transform child in transform.parent) {
            FilterButton fb = child.GetComponent<FilterButton>();
            fb.UnColorize(fb.panel, new Color(fb.panelUnselectedCol.r, fb.panelUnselectedCol.g, fb.panelUnselectedCol.b, fb.panel.color.a), 0);
            if (fb.content.activeInHierarchy)
                fb.HideChildsTween(childsTweenDuration);
        }
    }

    #region Animation Methods
    public Tween UnColorize(Image panel, Color col, float duration) {
        return panel.DOColor(col, duration).SetEase(colorTweenEase);
    }

    private Tween Colorize(Image panel, Color col, float duration) {
        return panel.DOColor(col, duration).SetEase(colorTweenEase);
    }

    private void HideChildsTween(float duration) {
        foreach (Transform child in content.transform) {
            FilterButton fb = child.gameObject.GetComponent<FilterButton>();
            Colorize(fb.panel, new Color(fb.panel.color.r, fb.panel.color.g, fb.panel.color.b, 0), duration).SetEase(childsHideTween);
            Colorize(fb.icon, new Color(fb.icon.color.r, fb.icon.color.g, fb.icon.color.b, 0), duration).SetEase(childsHideTween);
        }
        UIManager.Instance.MoveAnchorPosition(subFilterViewRect, subFilterViewHidePos, duration).SetEase(childsHideTween).
        OnComplete(() => content.SetActive(false));
    }

    private void ShowChildsTween(float duration) {
        content.SetActive(true);
        foreach (Transform child in content.transform) {
            FilterButton fb = child.gameObject.GetComponent<FilterButton>();

            if (fb.panel.color.a != 0)
                fb.panel.color = new Color(fb.panel.color.r, fb.panel.color.g, fb.panel.color.b, 0);

            Colorize(fb.panel, new Color(fb.panel.color.r, fb.panel.color.g, fb.panel.color.b, 1), duration).SetEase(childsShowTween);
            Colorize(fb.icon, new Color(fb.icon.color.r, fb.icon.color.g, fb.icon.color.b, 1), duration).SetEase(childsShowTween);
        }
        UIManager.Instance.MoveAnchorPosition(subFilterViewRect, subFilterViewPos, duration).SetEase(childsShowTween);
    }
    #endregion
}
