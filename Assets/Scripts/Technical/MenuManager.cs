using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private static MenuManager _instance;
    public static MenuManager Instance { get => _instance; }

    private void Awake() {
        _instance = this;
    }

    public void OnTrain() {
        LoadingCanvas.Instance.LoadScene("scene_train");
    }

    public void OnContinue() {
        SerialisationCanvas.Instance.LoadSaveFiles();
        LoadingCanvas.Instance.LoadScene("scene_game", true, SerialisationCanvas.Instance.saveFileList.First().Key);
    }
    
    public void OnNewGame() {
        LoadingCanvas.Instance.LoadScene("scene_game");
    }
    
    public void OnExit() {
        Application.Quit();
    }

    public void OnMainMenu() {
        LoadingCanvas.Instance.LoadScene("scene_menu");

    }
}
