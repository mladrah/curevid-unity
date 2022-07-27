using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;

public class LogManager : MonoBehaviour
{
    private static LogManager instance;

    [SerializeField] private TextMeshProUGUI logContent;

    private void Awake() {
        instance = this;
        logContent.text = "";
    }

    public static void Log(string text, string countryName = "", string hexColor = "") {
        if (instance == null)
            return;

        if (instance.logContent.text.Length != 0)
            instance.logContent.text += "\n";

        if (hexColor.Length != 0)
            instance.logContent.text += "<color=" + hexColor + ">";

        if (countryName.Length != 0)
            instance.logContent.text += "<b>" + countryName + ": </b>";

        instance.logContent.text += text;

        if (hexColor.Length != 0)
            instance.logContent.text += "</color>";
    }
}
