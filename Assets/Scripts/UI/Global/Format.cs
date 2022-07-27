using System.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Format : MonoBehaviour
{
    #region Modifiers Strings
    public static string Modifiers(List<Modifier> modifiers) {
        string formatted = "";

        List<Modifier> positiveModifiers = new List<Modifier>();
        List<Modifier> negativeModifiers = new List<Modifier>();

        for (int i = 0; i < modifiers.Count; i++) {
            Modifier modifier = modifiers[i];

            if (modifier.ValueType.Equals(Modifier.valueType.POSITIVE))
                positiveModifiers.Add(modifier);
            else
                negativeModifiers.Add(modifier);
        }

        formatted = ModifiersFormat(positiveModifiers);

        if (positiveModifiers.Count > 0)
            formatted += "\n";

        formatted += ModifiersFormat(negativeModifiers);

        return formatted;
    }

    private static string ModifiersFormat(List<Modifier> modifiers) {
        string formatted = "";

        for (int i = 0; i < modifiers.Count; i++) {
            Modifier modifier = modifiers[i];
            string formattedBegin = (modifier.ValueType.Equals(Modifier.valueType.POSITIVE) ? "<color=" + Colors.HEX_GREEN + ">" : "<color=" + Colors.HEX_RED + ">") +
                                    (modifier.Value >= 0 ? "+" : "");

            string multiplicativeFormatted = "";
            string additiveFormatted = "";

            if (modifier.EffectType.Equals(Modifier.effectType.MULTIPLICATIVE)) {
                multiplicativeFormatted = Percentage(modifier.Value);
            }

            if (modifier.EffectType.Equals(Modifier.effectType.ADDITIVE)) {
                additiveFormatted = Stat(modifier.Value);
            }

            string formattedEnd = " " + modifier.Name + "</color>"/* + "\n"*/;

            if (i < modifiers.Count - 1)
                formattedEnd += "\n";

            formatted += formattedBegin + multiplicativeFormatted + additiveFormatted + formattedEnd;
        }

        return formatted;
    }

    public static string SingleModifier(Modifier modifier) {
        string formatted = "";

        string formattedBegin = (modifier.ValueType.Equals(Modifier.valueType.POSITIVE) ? "<color=" + Colors.HEX_GREEN + ">" : "<color=" + Colors.HEX_RED + ">") +
                                   (modifier.Value >= 0 ? "+" : "");

        string multiplicativeFormatted = "";
        string additiveFormatted = "";

        if (modifier.EffectType.Equals(Modifier.effectType.MULTIPLICATIVE)) {
            multiplicativeFormatted = Percentage(modifier.Value);
        }

        if (modifier.EffectType.Equals(Modifier.effectType.ADDITIVE)) {
            additiveFormatted = (modifier.ModifierType.Equals(Modifier.modifierType.ECONOMY) ? Stat(modifier.Value) : "" + modifier.Value);
        }

        string formattedEnd = " " + modifier.Name + "</color>";

        formatted += formattedBegin + multiplicativeFormatted + additiveFormatted + formattedEnd;

        return formatted;
    }
    #endregion

    #region Stat Strings
    public static string Stat(float number) {
        return number.ToString("F", CultureInfo.InvariantCulture);
    }

    public static string Stat(double number) {
        return number.ToString("F", CultureInfo.InvariantCulture);
    }

    public static string StatColoredString(float number, string additionTextBehind = "", string additionalTextAfter = "", bool mirrored = false, bool multiplicativeOnly = false) {
        string formattedString = "";
        if (!multiplicativeOnly)
            formattedString = additionTextBehind + Stat(number) + additionalTextAfter;
        else
            formattedString = additionTextBehind + Percentage(number) + additionalTextAfter;

        string formattedStringColored = number == 0 ? ColorString(formattedString, Colors.HEX_GREEN) :
            number >= 0 ? ColorString("+" + formattedString, !mirrored ? Colors.HEX_GREEN : Colors.HEX_RED) :
            ColorString(formattedString, !mirrored ? Colors.HEX_RED : Colors.HEX_GREEN);

        return formattedStringColored;
    }

    public static string StatColoredString(double number, string additionTextBehind = "", string additionalTextAfter = "", bool mirrored = false, bool multiplicativeOnly = false) {
        string formattedString = "";
        if (!multiplicativeOnly)
            formattedString = additionTextBehind + Stat(number) + additionalTextAfter;
        else
            formattedString = additionTextBehind + Percentage(number) + additionalTextAfter;

        string formattedStringColored = number == 0 ? ColorString(formattedString, Colors.HEX_GREEN) :
            number >= 0 ? ColorString("+" + formattedString, !mirrored ? Colors.HEX_GREEN : Colors.HEX_RED) :
            ColorString(formattedString, !mirrored ? Colors.HEX_RED : Colors.HEX_GREEN);

        return formattedStringColored;
    }

    public static string SignString(float number) {
        return number >= 0 ? "+" : "-";
    }

    public static string SignString(double number) {
        return number >= 0 ? "+" : "-";
    }
    #endregion

    #region Color Strings
    public static string ColorString(string text, string hexCode) {
        return "<color=" + hexCode + ">" + text + "</color>";
    }
    #endregion

    public static string ColorStringCondition(Func<bool> condition, string text, string hexCodeTrue, string hexCodeFalse) {
        if (condition())
            return ColorString(text, hexCodeTrue);
        else
            return ColorString(text, hexCodeFalse);
    }

    #region Number Strings
    public static string Percentage(float number) {
        return number.ToString("P", new CultureInfo("en-US", false).NumberFormat);
    }

    public static string Percentage(double number) {
        return number.ToString("P", new CultureInfo("en-US", false).NumberFormat);
    }

    public static string LargeNumber(double number, bool toSign = false) {
        string formattedString = number.ToString("N1", CultureInfo.CreateSpecificCulture("sv-SE"));

        if (toSign) {
            formattedString = SignString(number) + formattedString;
        }

        for (int i = 0; i < formattedString.Length; i++) {
            if (formattedString[i] == ',') {
                formattedString = formattedString.Remove(i, 2);
            }
        }

        return formattedString;
    }
    #endregion
}
