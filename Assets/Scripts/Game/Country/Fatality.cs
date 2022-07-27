using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPMF;

public class Fatality : Stat
{
    private readonly Country _country;

    #region Properties
    private float _caseFatalityRate;
    public float CaseFatalityRate { get => _caseFatalityRate; }

    private double _crudeMortalityRate;
    public double CrudeMortalityRate { get => _crudeMortalityRate; }

    private double _newFatalities;
    public double NewFatalities { get => _newFatalities; }

    private double _virusSource;
    public double VirusSource { get => _virusSource; }

    private double _cureSource;
    public double CureSource { get => _cureSource; }
    #endregion

    public Fatality(Country country) {
        _country = country;
        _caseFatalityRate = Random.Range(0.01f, 0.04f);
    }

    #region Fatality Methods
    public Cases CauseFatalitiesVirus(Cases cases) {
        double fatalities = cases.count * (_caseFatalityRate /*+ _country.Vaccination.CureCaseFatalityRate*/);
        cases.count -= fatalities;
        _virusSource += fatalities;
        
        return cases;
    }

    public void SumNewFatalities() {
        double oldValue = TotalValue;
        _cureSource += _country.Vaccination.SideEffectFatalities;
        NewTotalValue(_virusSource + _cureSource);
        _newFatalities = TotalValue - oldValue;
        //_cureSource += _newFatalities * _country.Vaccination.CureCaseFatalityRate;
    }

    public void UpdateCrudeMortalityRate() {
        _crudeMortalityRate = (TotalValue / _country.Population) * 100000;
    }
    #endregion

    #region Serialisation
    public void LoadFromSaveData(SaveData.CountryFatalityData fatalityData) {
        base.LoadFromSaveData(fatalityData.baseStat);

        _caseFatalityRate = fatalityData.caseFatalityRate;
        _crudeMortalityRate = fatalityData.crudeMortalityRate;
        _newFatalities = fatalityData.newFatalities;

        _virusSource = fatalityData.virusSource;
        _cureSource = fatalityData.cureSource;
    }
    #endregion
}
