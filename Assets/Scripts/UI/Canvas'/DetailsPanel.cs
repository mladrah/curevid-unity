using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DetailsPanel : MonoBehaviour
{
    [SerializeField] private Button closeBtn;

    private bool _isVisible;
    public bool IsVisible { get => _isVisible; }

    private RectTransform rectTransform;
    private Tween detailsPanelTween;
    private Ease detailsPanelTweenEase;
    private Vector2 detailsPanelPos;
    private Vector2 detailsPanelHidePos;

    public Action onHideAction;

    private void Awake() {
        InitAnimation();
    }

    private void Start() {
        closeBtn.onClick.AddListener(HideTween);

        ResetPanel();
    }

    private void OnDisable() {
        ResetPanel();
    }

    private void InitAnimation() {
        rectTransform = GetComponent<RectTransform>();

        detailsPanelTweenEase = Ease.OutExpo;

        detailsPanelPos = rectTransform.anchoredPosition;
        detailsPanelHidePos = new Vector2((-1) * detailsPanelPos.x, detailsPanelPos.y);
    }

    public void ShowTween() {
        if (IsVisible)
            return;

        ShowPanel();
        detailsPanelTween.Kill();
        detailsPanelTween = UIManager.Instance.MoveAnchorPosition(rectTransform, detailsPanelPos, 0.5f).SetEase(detailsPanelTweenEase);
        _isVisible = true;
    }

    public void HideTween() {
        detailsPanelTween.Kill();
        detailsPanelTween = UIManager.Instance.MoveAnchorPosition(rectTransform, detailsPanelHidePos, 0.5f).SetEase(detailsPanelTweenEase);
        _isVisible = false;
        
        OnHide();
    }

    private void ShowPanel() {
        this.gameObject.SetActive(true);
    }

    private void OnHide() {
        if (onHideAction == null)
            return;

        onHideAction();
    }

    public void ResetPanel() {
        detailsPanelTween.Kill();
        _isVisible = false;
        rectTransform.anchoredPosition = detailsPanelHidePos;
    }

}
