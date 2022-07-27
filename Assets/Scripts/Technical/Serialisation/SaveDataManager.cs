using System.Collections.Generic;
using UnityEngine;
using Game;
using UnityEngine.SceneManagement;

public static class SaveDataManager
{
    public static SaveData SaveJsonData(string saveFileName) {
        SaveData sd = new SaveData();
        IEnumerable<ISaveable> a_Saveables = GetToSerializeData();

        foreach (var saveable in a_Saveables) {
            //Debug.Log("Save: " + saveable.ToString());
            saveable.PopulateSaveData(sd);
        }

        if (FileManager.WriteToFile(saveFileName, sd.ToJson())) {
            //Debug.Log("Save successful");
            //Debug.Log("---------");
        }
        return sd;
    }

    public static void LoadJsonData(string saveFileName) {
        SaveData sd = new SaveData();
        IEnumerable<ISaveable> a_Saveables = GetToSerializeData();

        if (FileManager.LoadFromFile(saveFileName, out var json)) {
            sd.LoadFromJson(json);

            foreach (var saveable in a_Saveables) {
                //Debug.Log("Load: " + saveable.ToString());
                saveable.LoadFromSaveData(sd);
            }

            //Debug.Log("Load complete");
            //Debug.Log("---------");
        }
    }

    public static List<ISaveable> GetToSerializeData() {
        List<ISaveable> serialiseData = new List<ISaveable>();

        serialiseData.Add(GlobalManager.Instance);
        serialiseData.Add(TimeManager.Instance);
        serialiseData.Add(ResourceManager.Instance);
        serialiseData.Add(ResearchManager.Instance);
        //serialiseData.Add(EventManager.Instance);
        serialiseData.Add(Cure.Instance);

        return serialiseData;
    }
}