using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ProgressCanvas : MonoBehaviour
{
    [Header("Game Over Progress")]
    private static ProgressCanvas _instance;
    public static ProgressCanvas Instance { get => _instance; }
    [SerializeField] private TooltipTrigger gameOverProgressTt;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI percentageNum;
    private const double _gameOverRate = 0.01f;
    private int _gameOverValue;
    public static int GAME_OVER_VALUE { get => _instance._gameOverValue; }
    private float ratio;
    private Tween sliderTween;

    public delegate void ProgressDelegate();
    public event ProgressDelegate OnGameOver;
    public event ProgressDelegate OnGameWon;

    [Header("Game Over Canvas")]
    public GameObject gameOverCanvas;
    public TextMeshProUGUI gameOverDescription;
    public StatisticCell goInfections;
    public StatisticCell goFatalities;
    public StatisticCell goVaccinations;
    public StatisticCell goRecoveries;

    [Header("Win Canvas")]
    public GameObject wonCanvas;
    public StatisticCell wonInfection;
    public StatisticCell wonFatalities;
    public StatisticCell wonVaccination;
    public StatisticCell wonRecoveries;

    private void Awake() {
        _instance = this;

        slider.value = 0;
        percentageNum.text = Format.Percentage(0);

        GlobalManager.Instance.allCountriesInitializedEvent += () => _gameOverValue = Mathf.CeilToInt((float)_gameOverRate * (float)GlobalManager.Instance.GetGlobalValue(Value.Population));
    }

    private void Start() {
        TimeManager.Instance.dailyUpdateDelegate += UpdateGameOverProgress;
        TimeManager.Instance.dailyUpdateDelegate += CheckEndDate;

        gameOverProgressTt.description = "If approximately <b>" + Format.Percentage(_gameOverRate) + " (" + Format.LargeNumber(_gameOverValue) + ")</b> of the global Population dies, the game is lost!";
        gameOverDescription.text = "More than <b>" + Format.Percentage(_gameOverRate) + " (" + Format.LargeNumber(_gameOverValue) + ")</b> of the global population <b>died</b>";

        //Debug.LogFormat("Min Fitness for Solution: {0}", (1 / (Mathf.Pow(10,-7) * (_gameOverValue - 1))) * 517);
    }

    public void UpdateGameOverProgress() {
        double totalFatalities = GlobalManager.Instance.GetGlobalValue(Value.Fatality);
        ratio = (float)(totalFatalities / _gameOverValue);

        if (ratio > 1)
            ratio = 1;

        if (ratio > 1)
            Debug.Log(ratio);
        sliderTween.Kill();
        sliderTween = slider.DOValue(ratio, 0.5f);
        percentageNum.text = Format.Percentage(ratio);

        if (ratio == 1) {
            if (OnGameOver != null)
                OnGameOver();
            if (!TechnicalManager.Instance.debugWinner) {
                ShowGameOverCanvas();
            } else
                LogManager.Log("Game Over!", "", Colors.HEX_RED);
        }
    }

    public float GetGameOverProgress() {
        double totalFatalities = GlobalManager.Instance.GetGlobalValue(Value.Fatality);
        return (float)(totalFatalities / _gameOverValue);
    }

    public void ShowGameOverCanvas() {
        TimeManager.Instance.Pause();
        TimeManager.Instance.CanBeModified = false;

        goInfections.num.text = Format.LargeNumber(GlobalManager.Instance.GetGlobalValue(Value.Infection));
        goFatalities.num.text = Format.LargeNumber(GlobalManager.Instance.GetGlobalValue(Value.Fatality));
        goVaccinations.num.text = Format.LargeNumber(GlobalManager.Instance.GetGlobalValue(Value.Vaccination));
        goRecoveries.num.text = Format.LargeNumber(GlobalManager.Instance.GetGlobalValue(Value.Recovery));

        gameOverCanvas.SetActive(true);
    }

    private void CheckEndDate() {
        if (TimeManager.Instance.GetCurrentDate().Equals(new System.DateTime(2021, 6, 1))) {
            //if (!AIDataManager.Instance.saveBestScorePerGeneration) {
                OnGameWon();
                ShowWinCanvas();
            //}
        }
    }

    public void ShowWinCanvas() {
        TimeManager.Instance.Pause();
        TimeManager.Instance.CanBeModified = false;

        wonInfection.num.text = Format.LargeNumber(GlobalManager.Instance.GetGlobalValue(Value.Infection));
        wonFatalities.num.text = Format.LargeNumber(GlobalManager.Instance.GetGlobalValue(Value.Fatality));
        wonVaccination.num.text = Format.LargeNumber(GlobalManager.Instance.GetGlobalValue(Value.Vaccination));
        wonRecoveries.num.text = Format.LargeNumber(GlobalManager.Instance.GetGlobalValue(Value.Recovery));

        wonCanvas.SetActive(true);
    }
}
