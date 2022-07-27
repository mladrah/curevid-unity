using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NEAT;

public enum Phase
{
    Phase_1,
    Phase_1_2,
    Phase_2,
    Phase_2_2,
    Phase_2_3,
    Phase_3,
    Phase_3_2,
    Phase_3_3
}

public enum Model
{
    FS,
    Regular,
    Hidden
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get => _instance; }

    public Phase currentPhase;
    public Model currentModel;

    [Header("Train Data")]
    public int saveStateNumber;
    public bool logSaveStateFile;

    [Header("MM")]
    [SerializeField] private GameObject mmP1;
    [SerializeField] private GameObject mmP12;

    [SerializeField] private GameObject mmP2;
    [SerializeField] private GameObject mmP22;
    [SerializeField] private GameObject mmP23;

    [SerializeField] private GameObject mmP3;
    [SerializeField] private GameObject mmP32;

    [Header("RM")]
    [SerializeField] private GameObject rmP3;
    [SerializeField] private GameObject rmP32;

    [Header("Configs")]
    [SerializeField] private GameObject configFS;
    [SerializeField] private GameObject configRegular;
    [SerializeField] private GameObject configHidden;

    private void Awake() {
        _instance = this;

        mmP1.SetActive(false);
        mmP12.SetActive(false);

        mmP2.SetActive(false);
        mmP22.SetActive(false);
        mmP23.SetActive(false);

        mmP3.SetActive(false);
        mmP32.SetActive(false);

        rmP3.SetActive(false);
        rmP32.SetActive(false);

        configFS.SetActive(false);
        configRegular.SetActive(false);
        configHidden.SetActive(false);

        switch (currentPhase) {
            case Phase.Phase_1:
                mmP1.SetActive(true);
                break;
            case Phase.Phase_1_2:
                mmP12.SetActive(true);
                break;
            case Phase.Phase_2:
                mmP2.SetActive(true);
                break;
            case Phase.Phase_2_2:
                mmP22.SetActive(true);
                break;
            case Phase.Phase_2_3:
                mmP23.SetActive(true);
                break;
            case Phase.Phase_3:
                mmP3.SetActive(true);
                rmP3.SetActive(true);
                break;
            case Phase.Phase_3_2:
                mmP32.SetActive(true);
                rmP32.SetActive(true);
                break;
            case Phase.Phase_3_3:
                mmP32.SetActive(true);
                rmP32.SetActive(true);
                break;
        }

        switch (currentModel) {
            case Model.FS:
                configFS.SetActive(true);
                break;
            case Model.Regular:
                configRegular.SetActive(true);
                break;
            case Model.Hidden:
                configHidden.SetActive(true);
                break;
        }
    }

    public void LoadNewGame() {
        int randomSaveStateIndex = Random.Range(1, 61);

        string saveFileName;
        if (saveStateNumber == 0)
            saveFileName = "Train_Save_State_" + randomSaveStateIndex + ".dat";
        else
            saveFileName = "Train_Save_State_" + saveStateNumber + ".dat";

        SaveDataManager.LoadJsonData(saveFileName);

        if (logSaveStateFile)
            Debug.Log("Loaded: " + saveFileName);
    }
}
