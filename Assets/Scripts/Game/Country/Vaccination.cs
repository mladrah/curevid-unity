using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPMF;

public class Vaccination : Stat
{
    private readonly Country _country;

    #region Properties
    private double _newVaccinated;
    public double NewVaccinated { get => _newVaccinated; }

    private double _cureCaseFatalityRate;
    public double CureCaseFatalityRate { get => _cureCaseFatalityRate; }

    private double _sideEffectFatalities;
    public double SideEffectFatalities { get => _sideEffectFatalities; }

    private bool _isDistributed;
    public bool IsDistributed { get => _isDistributed; }
    #endregion

    public Vaccination(Country country) : base() {
        _country = country;
    }

    #region Vaccination Methods
    public void Vaccinate() {
        double oldValue = TotalValue;

        ChangeTotalValueBy(Calculate.Stat(this));

        if (TotalValue > _country.Population - _country.Fatality.TotalValue)
            NewTotalValue(_country.Population - _country.Fatality.TotalValue);

        _newVaccinated = TotalValue - oldValue;

        UpdateModifier("Base Value", Modifiers[0].Value * Random.Range(0.95f, 1.05f));

        _isDistributed = true;

        _sideEffectFatalities = _newVaccinated * _cureCaseFatalityRate;
    }

    public void DistributeVaccines() {
        float value = (float)(0.00125f * _country.Population);
        switch (_country.Subregion.Continent.Name) {
            case "Americas":
                value *= Random.Range(1, 1.6f);
                break;
            case "Europe":
                value *= Random.Range(1, 1.6f);
                break;
            case "Asia":
                value *= Random.Range(0.5f, 0.9f);
                break;
            case "Oceania":
                value *= Random.Range(0.8f, 1);
                break;
            case "Africa":
                value *= Random.Range(0.2f, 0.5f);
                break;
            default:
                Debug.LogError("Continent Name did not match");
                break;
        }

        Modifiers.Add(new Modifier("Base Value", Modifier.effectType.ADDITIVE, Modifier.modifierType.VACCINE, Modifier.valueType.POSITIVE, value));

        _cureCaseFatalityRate = Cure.Instance.SafetyCasualities();

        _isDistributed = true;
    }
    #endregion

    #region Serialisation
    public void LoadFromSaveData(SaveData.CountryVaccinationData vaccinationData) {
        base.LoadFromSaveData(vaccinationData.baseStat);

        _newVaccinated = vaccinationData.newVaccinated;
        _cureCaseFatalityRate = vaccinationData.cureCaseFatalityRate;
        _sideEffectFatalities = vaccinationData.sideEffectFatalities;

        _isDistributed = vaccinationData.isDistributed;
    }
    #endregion
}
