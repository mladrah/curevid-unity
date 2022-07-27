using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using UnityEngine;

public class TechnicalManager : MonoBehaviour
{
    private static TechnicalManager _instance;
    public static TechnicalManager Instance { get => _instance; }

    public const float CANVAS_SCALER_X = 1920;
    public const float CANVAS_SCALER_Y = 1080;

    public bool targetFrameRate;

    [Header("Debug")]
    public bool debugWinner;
    public bool debugEndless;
    public bool debugScreenshot;
    public bool debugReset;
    public bool debugResearch;

    private void Awake() {
        _instance = this;

        if (targetFrameRate)
            Application.targetFrameRate = 144;
        else
            Application.targetFrameRate = 1000;
    }

    public void RecordTime<T>(Action<T> proc, T param) {
        Stopwatch st = new Stopwatch();
        st.Start();
        proc(param);
        st.Stop();
        UnityEngine.Debug.Log(string.Format("MyMethod took {0} ms to complete", st.ElapsedMilliseconds));
    }

    private static Stopwatch stopwatch;
    private static string info;
    public static void StartTime(string info = "") {
        UnityEngine.Debug.LogFormat("Starting Time <b>{0}</b>", info);
        TechnicalManager.info = info;
        stopwatch = new Stopwatch();
        stopwatch.Start();
    }

    public static void StopTime() {
        stopwatch.Stop();
        UnityEngine.Debug.Log(string.Format("MyMethod took {0} ms to complete", stopwatch.ElapsedMilliseconds));
        UnityEngine.Debug.LogFormat("Stopping Time <b>{0}</b>", TechnicalManager.info);
    }
}
