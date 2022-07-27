using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;

public class MenuCanvas : DynamicCanvas
{
    private static MenuCanvas _instance;
    public static MenuCanvas Instance { get => _instance; }

    [Header("Concrete UI")]
    public GameObject menuPanel;

    public override void Awake() {
        base.Awake();
        _instance = this;
    }

    public override void HidePanel() {
        base.HidePanel();

        CameraController.Instance.canMove = true;
        if(EventManager.Instance == null)
            TimeManager.Instance.CanBeModified = true;
        else if (!EventManager.Instance.isActive)
            TimeManager.Instance.CanBeModified = true;

        if (TimeManager.Instance.timeSpeed != TimeManager.TimeSpeed.PAUSE)
            TimeManager.Instance.Pause();

        menuPanel.SetActive(false);

        UIManager.Instance.inputDisable = false;
    }

    public override void ShowPanel() {
        base.ShowPanel();

        menuPanel.SetActive(true);

        CameraController.Instance.canMove = false;
        if (TimeManager.Instance.timeSpeed != TimeManager.TimeSpeed.PAUSE)
            TimeManager.Instance.Pause();
        TimeManager.Instance.CanBeModified = false;

        UIManager.Instance.inputDisable = true;
    }
}
