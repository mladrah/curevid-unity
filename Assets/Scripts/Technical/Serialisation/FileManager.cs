using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class FileManager
{
    //public const string PATH = Application.persistentDataPath + "/saves/"; /*C:/Users/rahmi/Desktop/saves";*/


    
    public static bool WriteToFile(string a_FileName, string a_FileContents) {
        var fullPath = Path.Combine(SerialisationCanvas.PATH, a_FileName);
         
        try {
            File.WriteAllText(fullPath, a_FileContents);
            return true;
        } catch (Exception e) {
            Debug.LogError($"Failed to write to {fullPath} with exception {e}");
            return false;
        }
    }

    public static bool LoadFromFile(string a_FileName, out string result) {

        if (SceneManager.GetActiveScene().buildIndex == (int)Scenes.Train) {
            try {
                result = Resources.Load<TextAsset>("Neuroevolution/train_saves/" + a_FileName.Replace(".dat", "")).text;
                return true;
            }catch(Exception e) {
                Debug.LogError($"Failed to read from {a_FileName} with exception {e}");
                result = "";
                return false;
            }
        }

        var fullPath = Path.Combine(SerialisationCanvas.PATH, a_FileName);

        try {
            result = File.ReadAllText(fullPath);
            return true;
        } catch (Exception e) {
            Debug.LogError($"Failed to read from {fullPath} with exception {e}");
            result = "";
            return false;
        }
    }
}