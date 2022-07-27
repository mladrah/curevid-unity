using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class StatisticCell : MonoBehaviour
{
    public TextMeshProUGUI label;
    public TextMeshProUGUI num;

    public void Colorize(string hexCode) {
        //label.text = Format.ColorString(label.text, hexCode);
        num.text = Format.ColorString(num.text, hexCode);
    }

    public void ColorizeCondition(Func<bool> condition, string hexCodeTrue, string hexCodeFalse) {
        if (condition())
            Colorize(hexCodeTrue);
        else
            Colorize(hexCodeFalse);
    }
}
