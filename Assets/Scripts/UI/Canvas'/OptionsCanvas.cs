using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class OptionsCanvas : DynamicCanvas
{
    private static OptionsCanvas _instance;
    public static OptionsCanvas Instance { get => _instance; }

    [Header("Concrete Canvas")]
    public GameObject optionsPanel;

    [Header("UI Fields")]
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown fpsLimitDropdown;
    public Toggle fullscreenToggle;

    public Resolution[] resolutions;
    [HideInInspector] public int[] fpsLimit = { 30, 60, 120, 144, 240, -1 };

    public override void Awake() {
        base.Awake();
        _instance = this;
        resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
    }

    private void Start() {
        UpdateScreenUI();
        UpdateQualityUI();
        UpdateFPSLimitUI();
    }

    public override void HidePanel() {
        base.HidePanel();

        optionsPanel.SetActive(false);
    }

    public override void ShowPanel() {
        base.ShowPanel();

        optionsPanel.SetActive(true);
    }

    private void UpdateScreenUI() {
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolution = -1;
        int i = 0;
        foreach (Resolution r in resolutions) {
            string toAdd = r.width + " x " + r.height;
            options.Add(toAdd);

            if (r.width == Screen.width && r.height == Screen.height)
                currentResolution = i;

            i++;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolution;
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = Screen.fullScreen;
    }

    private void UpdateQualityUI() {
        qualityDropdown.value = QualitySettings.GetQualityLevel();
    }

    private void UpdateFPSLimitUI() {
        int currentFPSLimitIndex = fpsLimit[3];
        for (int i = 0; i < fpsLimit.Length; i++) {
            if (fpsLimit[i] == Application.targetFrameRate)
                currentFPSLimitIndex = i;
        }

        fpsLimitDropdown.value = currentFPSLimitIndex;
        fpsLimitDropdown.RefreshShownValue();
    }
}
