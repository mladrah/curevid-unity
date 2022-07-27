using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LoadingCanvas : MonoBehaviour
{
    private static LoadingCanvas _instance;
    public static LoadingCanvas Instance { get => _instance; }

    [Header("UI Fields")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private Slider progressSlider;

    public bool isDone;

    private void Awake() {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this);
    }

    public void LoadScene(string sceneName, bool loadingFile = false, string fileName = "") {
        progressSlider.value = 0;
        loadingPanel.SetActive(true);
        StartCoroutine(LoadProgress(sceneName, loadingFile, fileName));
    }

    private IEnumerator LoadProgress(string sceneName, bool loadingFile = false, string fileName = "") {

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOperation.isDone) {

            progressSlider.value = asyncOperation.progress;

            yield return null;
        }


        if (loadingFile) {
            SaveDataManager.LoadJsonData(fileName + ".dat");
        }

        StartCoroutine(Wait(0.25f));
    }

    private IEnumerator Wait(float seconds) {
        progressSlider.value = 1;

        yield return new WaitForSeconds(seconds);

        loadingPanel.SetActive(false);
    }
}
