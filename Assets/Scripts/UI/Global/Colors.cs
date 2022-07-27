using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Colors
{
    #region Color Object
    public static Color WHITE_DARK = new Color(1, 0.9058824f, 0.7960784f);
    public static Color BLUE_DARK = new Color(0.1019608f, 0.1019608f, 0.145098f);
    public static Color BLUE_MEDIUM = new Color(0.1372549f, 0.1372549f, 0.2078431f);
    public static Color BLUE_LIGHT = new Color(0.1843137f, 0.1843137f, 0.2627451f);
    public static Color BLUE_VERY_LIGHT = new Color(0.241901f, 0.241901f, 0.3396226f);
    #endregion

    #region String Hex Color Code
    public static string HEX_WHITE = "#FFF8F0";
    public static Color WHITE { get => HexToColor(HEX_WHITE); }

    public static string HEX_GREEN = "#ABFFCD";
    public static Color GREEN { get => HexToColor(HEX_GREEN); }

    public static string HEX_YELLOW = "#FFE2AB";
    public static Color YELLOW { get => HexToColor(HEX_YELLOW); }

    public static string HEX_RED = "#D62841";
    public static Color RED { get => HexToColor(HEX_RED); }

    public static string HEX_BLUE = "#50C0FF";
    public static Color BLUE { get => HexToColor(HEX_BLUE); }

    public static string HEX_PURPLE = "#AB50FF";
    public static Color PURPLE { get => HexToColor(HEX_PURPLE); }

    //public static string HEX_DEFAULT_RED = "#DA4167";
    //public static Color DEFAULT_RED { get => HexToColor(HEX_DEFAULT_RED); }

    public static string HEX_DEFAULT_GREEN = "#40D999";
    public static Color DEFAULT_GREEN { get => HexToColor(HEX_DEFAULT_GREEN); }

    public static string HEX_BLUE_1 = "#3E3E57";
    public static Color BLUE_1 { get => HexToColor(HEX_BLUE_1); }

    public static string HEX_BLUE_2 = "#2F2F43";
    public static Color BLUE_2 { get => HexToColor(HEX_BLUE_2); }

    public static string HEX_BLUE_3 = "#232335";
    public static Color BLUE_3 { get => HexToColor(HEX_BLUE_3); }

    public static string HEX_BLUE_4 = "#1A1A25";
    public static Color BLUE_4 { get => HexToColor(HEX_BLUE_4); }

    #endregion

    #region Converters
    public static Color HexToColor(string hex) {
        if(hex[0] != '#') {
            Debug.LogError("Hex String -> " + hex + " is missing a '#' | (Colors.cs : HexToColor)");
            return Color.magenta;
        }

        Color color;
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }
    #endregion
}
