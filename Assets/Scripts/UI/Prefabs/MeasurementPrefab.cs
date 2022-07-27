using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MeasurementPrefab : MonoBehaviour, IPointerClickHandler
{
    private Measurement _measurement;

    #region UI Fields
    [SerializeField] private Image panel;
    [SerializeField] private Image icon;
    [SerializeField] private new TextMeshProUGUI name;
    public GameObject familyLine;
    [SerializeField] private Color activeColor;
    #endregion

    #region Delegate
    public delegate void MeasurementPrefabDelegate(Measurement measurement);
    public event MeasurementPrefabDelegate OnClickEvent;
    #endregion

    #region Animation
    [Header("Animation")]
    private Tween punchTween;
    private const float punchScale = 0.15f;
    private Tween colorTween;
    private const float colorTweenDuration = 0.4f;
    private const Ease colorTweenEase = Ease.OutExpo;
    #endregion

    #region Clone
    public void Clone(Measurement measurement) {
        _measurement = measurement;

        SetFields();
        SetVisual();
    }
    #endregion

    #region Set UI Fields
    private void SetFields() {
        name.text = _measurement.Name;
        icon.sprite = _measurement.Icon;
    }

    public void SetVisual() {
        panel.color = _measurement.IsActive ? activeColor : Colors.HexToColor(Colors.HEX_WHITE);
        icon.color = _measurement.IsActive ? Colors.HexToColor(Colors.HEX_WHITE) : Colors.HexToColor(Colors.HEX_BLUE_2);
        name.color = _measurement.IsActive ? Colors.HexToColor(Colors.HEX_WHITE) : Colors.HexToColor(Colors.HEX_BLUE_2);
    }
    #endregion

    #region OnClick
    public void OnPointerClick(PointerEventData eventData) {
        punchTween.Kill();
        transform.localScale = new Vector2(1, 1);
        punchTween = UIManager.Instance.PunchScale(transform, new Vector2(punchScale, punchScale));
        OnClickEvent(_measurement);
    }
    #endregion
}
