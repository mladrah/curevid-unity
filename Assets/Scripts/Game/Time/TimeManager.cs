using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;

public class TimeManager : MonoBehaviour, ISaveable
{
    private static TimeManager _instance;
    public static TimeManager Instance { get => _instance; }

    #region Time Fields
    private const int START_DAY = 1, START_MONTH = 1, START_YEAR = 2020;
    [HideInInspector] public double minute, hour;
    [HideInInspector] public int day, month, year;
    #endregion

    #region Time Modifiaction Fields
    public enum TimeSpeed
    {
        PAUSE = 0,
        NORMAL = 10,
        FAST = 100,
        DEBUG_FAST = 10000,
        AI
    }
    public TimeSpeed timeSpeed;
    private bool isPaused;
    private int lastSelectedSpeedModifier;
    public bool CanBeModified { get; set; } = true;
    #endregion

    #region Event Delegates
    public delegate void DailyUpdate();
    public DailyUpdate dailyUpdateDelegate;
    #endregion

    #region AI
    public float simulationSpeed = 1;
    #endregion

    private void Awake() {
        _instance = this;
        lastSelectedSpeedModifier = Array.IndexOf(Enum.GetValues(timeSpeed.GetType()), timeSpeed);

        minute = 0;
        hour = 12;
        day = START_DAY;
        month = START_MONTH;
        year = START_YEAR;
    }

    private void Start() {
        TimeCanvas.Instance.SetButtonUI(timeSpeed == TimeSpeed.PAUSE ? 0 : timeSpeed == TimeSpeed.NORMAL ? 1 : timeSpeed == TimeSpeed.FAST ? 2 : 3);
        if (timeSpeed == TimeSpeed.PAUSE)
            PauseAnimation();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
            Pause();
        if (Input.GetKeyDown(KeyCode.Alpha1))
            NormalSpeed();
        if (Input.GetKeyDown(KeyCode.Alpha2))
            FastSpeed();
        if (Input.GetKeyDown(KeyCode.Alpha3))
            DebugFastSpeed();

        if (!timeSpeed.Equals(TimeSpeed.AI))
            CalculateMinute();
        else
            CalculateDay();

        TimeCanvas.Instance.UpdateTimeUI();
    }

    private void FixedUpdate() {

    }

    #region Time Calculation
    public void CalculateMinute() {
        minute += Time.fixedDeltaTime * (int)timeSpeed;

        if (minute >= 60) {
            CalculateHour();
        }
    }

    public void CalculateHour() {
        hour++;
        minute = 0;

        if (hour >= 24) {
            CalculateDay();
        }
    }

    public void CalculateDay() {
        day++;
        hour = 0;

        CalculateMonth();
        CalculateYear();

        dailyUpdateDelegate();
    }

    public void CalculateMonth() {
        int monthTmp = month % 8;
        if (month >= 8)
            monthTmp++;

        if (monthTmp % 2 == 1) {
            if (day > 31) {
                MonthEnd();
            }
        } else {
            if (month == 2) {
                if (year == 2020 && day > 29)
                    MonthEnd();
                else if (year > 2020 && day > 28) {
                    MonthEnd();
                }
            } else if (day > 30) {
                MonthEnd();
            }
        }
    }

    private void MonthEnd() {
        month++;
        day = 1;

        //SaveDataManager.SaveJsonData("auto_save_" + string.Format("{0:00}_{1:00}_{2}", TimeManager.Instance.day, TimeManager.Instance.month, TimeManager.Instance.year) + ".dat");
    }

    public void CalculateYear() {
        if (month > 12) {
            year++;
            month = 1;
        }
    }
    #endregion

    #region Time Modification
    public void Pause() {
        if (!CanBeModified)
            return;

        if (isPaused) {
            switch (lastSelectedSpeedModifier) {
                case 1:
                    NormalSpeed();
                    break;
                case 2:
                    FastSpeed();
                    break;
                case 3:
                    DebugFastSpeed();
                    break;
                default:
                    NormalSpeed();
                    break;
            }
            isPaused = false;
        } else {
            timeSpeed = TimeSpeed.PAUSE;
            isPaused = true;
            TimeCanvas.Instance.SetButtonUI(0);
            PauseAnimation();
        }
    }

    public void NormalSpeed() {
        if (!CanBeModified)
            return;

        timeSpeed = TimeSpeed.NORMAL;
        lastSelectedSpeedModifier = 1;
        isPaused = false;
        TimeCanvas.Instance.SetButtonUI(lastSelectedSpeedModifier);
        UnPauseAnimation();
    }

    public void FastSpeed() {
        if (!CanBeModified)
            return;

        timeSpeed = TimeSpeed.FAST;
        lastSelectedSpeedModifier = 2;
        isPaused = false;
        TimeCanvas.Instance.SetButtonUI(lastSelectedSpeedModifier);
        UnPauseAnimation();
    }

    //Debug
    public void DebugFastSpeed() {
        if (!CanBeModified)
            return;

        timeSpeed = TimeSpeed.DEBUG_FAST;
        lastSelectedSpeedModifier = 3;
        isPaused = false;
        TimeCanvas.Instance.SetButtonUI(lastSelectedSpeedModifier);
        UnPauseAnimation();
    }

    public void AIFastSpeed() {
        if (!CanBeModified)
            return;

        timeSpeed = TimeSpeed.AI;
        lastSelectedSpeedModifier = 4;
        isPaused = false;
        TimeCanvas.Instance.SetButtonUI(lastSelectedSpeedModifier);
        UnPauseAnimation();
    }
    #endregion

    #region Getter
    public DateTime GetCurrentDate() {
        try {
            new DateTime(year, month, day);
        }catch(ArgumentOutOfRangeException e) {
            Debug.Log(year + " " + month + " " + day);
        }
        return new DateTime(year, month, day);
    }

    public DateTime GetStartDate() {
        return new DateTime(START_YEAR, START_MONTH, START_DAY);
    }

    #endregion

    #region Animation
    public void PauseAnimation() {
        DOTween.To(() => PostProcessing.instance.vignette.intensity.value, x => PostProcessing.instance.vignette.intensity.Override(x), 0.25f, 0.3f);
    }
    public void UnPauseAnimation() {
        DOTween.To(() => PostProcessing.instance.vignette.intensity.value, x => PostProcessing.instance.vignette.intensity.Override(x), 0.1f, 0.3f);
    }
    #endregion

    #region Serialisation
    public void PopulateSaveData(SaveData saveData) {
        saveData.timeData = new SaveData.TimeData(this);
    }

    public void LoadFromSaveData(SaveData saveData) {
        SaveData.TimeData timeData = saveData.timeData;

        this.minute = timeData.minute;
        this.hour = timeData.hour;

        this.day = timeData.day;
        this.month = timeData.month;
        this.year = timeData.year;

        TimeCanvas.Instance.UpdateTimeUI();
    }
    #endregion
}
