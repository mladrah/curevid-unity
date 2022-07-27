using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SerialisationCanvas : DynamicCanvas
{
    public enum SerialisationMode
    {
        Loading,
        Saving
    }

    private static SerialisationCanvas _instance;
    public static SerialisationCanvas Instance { get => _instance; }

    [Header("Concrete Canvas")]
    public GameObject saveFileParent;
    public GameObject saveFilePrefab;
    public GameObject serialisationPanel;
    public Dictionary<string, string> saveFileList;
    public TextMeshProUGUI panelTitle;

    [Header("Serialisation Logic")]
    public SaveFilePrefab focusedSaveFile;
    public static string PATH;

    [Header("Save")]
    public GameObject inputFieldPanel;
    public TMP_InputField inputField;
    public Button saveBtn;

    [Header("Load")]
    public Button loadBtn;

    public override void Awake() {
        base.Awake();
        _instance = this;

        PATH = Application.persistentDataPath + "/saves/";

        saveFileList = new Dictionary<string, string>();
    }

    private void Start() {
        if(SceneManager.GetActiveScene().buildIndex != (int) Scenes.Train)
            LoadSaveFiles();

        ResetFocusedFile();
    }

    public override void HidePanel() {
        base.HidePanel();

        ResetFocusedFile();

        serialisationPanel.SetActive(false);
    }

    public void ShowLoadPanel() {
        base.ShowPanel();

        inputFieldPanel.SetActive(false);
        saveBtn.gameObject.SetActive(false);
        loadBtn.gameObject.SetActive(true);

        panelTitle.text = "Load Game";

        LoadSaveFiles();
        serialisationPanel.SetActive(true);
    }

    public void ShowSavePanel() {
        base.ShowPanel();

        ResetFocusedFile();

        loadBtn.gameObject.SetActive(false);
        inputFieldPanel.SetActive(true);
        saveBtn.gameObject.SetActive(true);

        if (SceneManager.GetActiveScene().buildIndex == (int)Scenes.Train)
            inputField.text = "Train_Save_State";
        else
            inputField.text = "Curevid_Save_" + UnityEngine.Random.Range(1000, 9999);

        panelTitle.text = "Save Game";

        LoadSaveFiles();
        serialisationPanel.SetActive(true);
    }

    public void Save() {
        string saveFileName = inputField.text;

        if (string.IsNullOrEmpty(saveFileName))
            saveFileName = "Curevid_Save_" + UnityEngine.Random.Range(1000, 9999);

        if (saveFileList.ContainsKey(saveFileName)) {
            Debug.Log("Override");
        }

        if (SceneManager.GetActiveScene().buildIndex == (int)Scenes.Train) {
            LoadSaveFiles();
            saveFileName = "Train_Save_State_" + saveFileList.Count;
        }

        SaveDataManager.SaveJsonData(saveFileName + ".dat");

        inputField.text = "Curevid_Save_" + UnityEngine.Random.Range(1000, 9999);

        ResetFocusedFile();

        LoadSaveFiles();
    }

    public void Load() {
        if (focusedSaveFile == null) {
            return;
        }

        //SaveDataManager.LoadJsonData(focusedSaveFile.saveName.text + ".dat");
        LoadingCanvas.Instance.LoadScene("scene_game", true, focusedSaveFile.saveName.text);

        ResetFocusedFile();
    }

    public void Delete(string saveFileName) {
        string path = "";

        if (saveFileList.TryGetValue(saveFileName, out path)) {
            File.Delete(path);
        } else {
            Debug.LogError("Could not find path to: " + saveFileName);
        }

        ResetFocusedFile();

        LoadSaveFiles();
    }



    public void LoadSaveFiles() {
        foreach (Transform child in saveFileParent.transform)
            Destroy(child.gameObject);

        if (!Directory.Exists(PATH))
            Directory.CreateDirectory(PATH);

        saveFileList = Directory.GetFiles(PATH).OrderByDescending(f => new FileInfo(f).LastWriteTime).ToDictionary(f => Path.GetFileNameWithoutExtension(f), f => f);

        foreach (KeyValuePair<string, string> sf in saveFileList) {
            GameObject go = Instantiate(saveFilePrefab);
            go.transform.SetParent(saveFileParent.transform, false);

            SaveFilePrefab prefab = go.GetComponent<SaveFilePrefab>();

            FileManager.LoadFromFile(sf.Key + ".dat", out var json);
            SaveData sd = new SaveData();
            sd.LoadFromJson(json);

            prefab.ingameTime.text = string.Format("{0:00}.{1:00}.{2}", sd.timeData.day, sd.timeData.month, sd.timeData.year);
            prefab.saveName.text = Path.GetFileName(sf.Key);
            prefab.OnClickEvent += SetFocusedFile;
        }
    }

    public void SetFocusedFile(SaveFilePrefab saveFile) {
        ResetFocusedFile();

        inputField.text = saveFile.saveName.text;
        focusedSaveFile = saveFile;
        saveFile.Focus();
    }

    public void ResetFocusedFile() {
        if (focusedSaveFile == null)
            return;

        focusedSaveFile.Unfocus();
        focusedSaveFile = null;
    }
}
