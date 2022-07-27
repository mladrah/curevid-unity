using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    private static OptionsManager _instance;
    public static OptionsManager Instance { get => _instance; }

    private void Awake() {
        _instance = this;
    }

    public void SetResolution(int resolutionIndex) {
        Resolution res = OptionsCanvas.Instance.resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool isFullscreen) {
        Screen.fullScreen = isFullscreen;
    }

    public void SetQuality(int qualityIndex) {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFPS(int fpsIndex) {
        int fps = OptionsCanvas.Instance.fpsLimit[fpsIndex];
        Application.targetFrameRate = fps;
    }


}
