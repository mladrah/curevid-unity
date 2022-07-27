using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WPMF;

public class CountryHappiness : MonoBehaviour
{
    private Country country;

    [Header("Happiness")]
    [SerializeField] private Image happinessIcon;
    [SerializeField] private TextMeshProUGUI happinessNum;
    [SerializeField] private TextMeshProUGUI happinessDailyChange;
    [SerializeField] private Slider happinessSlider;

    [Header("Revolt")]
    [SerializeField] private Image revoltIcon;
    [SerializeField] private TextMeshProUGUI revoltNum;
    [SerializeField] private TextMeshProUGUI revoltDailyChange;
    [SerializeField] private Slider revoltSlider;

    public void UpdateUI(Country country) {
        this.country = country;

        UpdateHappinessUI();
        UpdateRevoltUI();
    }

    private void UpdateHappinessUI() {
        if (country.Happiness.IsBelow) {
            happinessIcon.sprite = Images.Instance.happiness_angry;
            happinessIcon.color = Colors.HexToColor(Colors.HEX_RED);
        } else if (country.Happiness.IsAbove) {
            happinessIcon.sprite = Images.Instance.happiness_happy;
            happinessIcon.color = Colors.HexToColor(Colors.HEX_GREEN);
        } else {
            happinessIcon.sprite = Images.Instance.happiness_neutral;
            happinessIcon.color = Colors.WHITE;
        }
        happinessNum.text = "" + Format.Stat(country.Happiness.TotalValue);
        happinessSlider.value = (float) country.Happiness.TotalValue;
        happinessDailyChange.text = Format.StatColoredString(Calculate.Stat(country.Happiness), "", " /day");
        happinessDailyChange.GetComponent<TooltipTrigger>().modifiers = Format.Modifiers(country.Happiness.Modifiers);
    }

    private void UpdateRevoltUI() {
        if (country.Revolt.IsAbove1) {
            revoltIcon.color = Colors.HexToColor(Colors.HEX_RED);
        } else {
            revoltIcon.color = Colors.WHITE;
        }

        revoltNum.text = "" + Format.Stat(country.Revolt.TotalValue);
        revoltSlider.value = (float) country.Revolt.TotalValue;
        revoltDailyChange.text = Format.StatColoredString(Calculate.Stat(country.Revolt), "", " /day", true);
        revoltDailyChange.GetComponent<TooltipTrigger>().modifiers = Format.Modifiers(country.Revolt.Modifiers);
    }
}
