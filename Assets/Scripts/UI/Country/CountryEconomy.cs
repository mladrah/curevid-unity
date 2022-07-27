using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using WPMF;

public class CountryEconomy : MonoBehaviour
{
    private Country country;

    [Header("General")]
    [SerializeField] private TextMeshProUGUI economyNum;
    [SerializeField] private TextMeshProUGUI economyModifiersText;

    public void UpdateUI(Country country) {
        this.country = country;

        economyNum.text = "";
        economyModifiersText.text = "";

        economyNum.text += Format.StatColoredString(country.Economy.TotalValue);

        List<Modifier> tempList = new List<Modifier>();
        foreach (Modifier modifier in country.Economy.Modifiers) {
            tempList.Add(modifier);
        }

        economyModifiersText.text += Format.Modifiers(tempList);
    }
}