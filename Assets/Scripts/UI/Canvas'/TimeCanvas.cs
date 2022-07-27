using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TimeCanvas : MonoBehaviour
{
    private static TimeCanvas _instance;
    public static TimeCanvas Instance { get => _instance; }

    #region Time UI Fields
    [SerializeField] private TextMeshProUGUI clockText;
    [SerializeField] private TextMeshProUGUI dateText;
    #endregion

    #region Time Modification UI Fields
    [Header("Time Modification UI")]
    [SerializeField] private List<Button> speedButtons;
    private Color initButtonColor;
    #endregion

    #region Animation
    [Header("Animation")]
    [SerializeField] private const float fadeDuration = 0.2f;
    private const Ease fadeInEase = Ease.OutSine;
    private const Ease fadeOutEase = Ease.InSine;
    #endregion

    private void Awake() {
        _instance = this;
        initButtonColor = speedButtons[0].GetComponent<Image>().color;
    }

    #region Update UI Fields
    public void UpdateTimeUI() {
        clockText.text = string.Format("{0:00}:{1:00}", TimeManager.Instance.hour, TimeManager.Instance.minute);
        dateText.text = string.Format("{0:00}.{1:00}.{2}", TimeManager.Instance.day, TimeManager.Instance.month, TimeManager.Instance.year);

    }
    #endregion

    #region Button Animations
    public void SetButtonUI(int index) {
        ResetButtons();
        speedButtons[index].GetComponent<Image>().DOColor(Colors.DEFAULT_GREEN, fadeDuration).SetEase(fadeInEase);
        speedButtons[index].GetComponent<Outline>().enabled = true;
    }

    private void ResetButtons() {
        foreach (Button button in speedButtons) {
            button.GetComponent<Image>().DOColor(initButtonColor, fadeDuration).SetEase(fadeOutEase);
            button.GetComponent<Outline>().enabled = false;
        }
    }
    #endregion
}
