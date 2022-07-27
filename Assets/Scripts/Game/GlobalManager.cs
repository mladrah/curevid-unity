using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using WPMF;

public class GlobalManager : MonoBehaviour, ISaveable
{
    private WorldMap2D map;

    #region Properties
    private static GlobalManager _instance;
    public static GlobalManager Instance { get => _instance; }

    private Dictionary<string, Continent> _continents;
    public Dictionary<string, Continent> Continents { get => _continents; }

    private Dictionary<string, Subregion> _subregions;
    public Dictionary<string, Subregion> Subregions { get => _subregions; }

    private Dictionary<string, Country> _countries;
    public Dictionary<string, Country> Countries { get => _countries; }

    private List<CountryWorker> _countryWorkers;
    public List<CountryWorker> CountryWorkers { get => _countryWorkers; }
    #endregion

    #region Event Fields
    public delegate void countryManagerDelegate();
    public event countryManagerDelegate allCountriesInitializedEvent;
    [NonSerialized] public bool allCountriesInitialized;
    #endregion

    #region Debug
    [Header("Debug")]
    [SerializeField] private bool debugSingleCountries = false;
    public List<string> countryNames;
    #endregion

    private void Awake() {
        _instance = this;
        _countries = new Dictionary<string, Country>();
        _continents = new Dictionary<string, Continent>();
        _subregions = new Dictionary<string, Subregion>();
        _countryWorkers = new List<CountryWorker>();
    }

    private void Start() {
        map = WorldMap2D.instance;

        if (debugSingleCountries) {
            foreach (string countryName in countryNames) {
                Country country = map.countries[map.GetCountryIndex(countryName)];
                _countries.Add(countryName, country);

                if (country == null)
                    Debug.LogError("Could not find Country with the name: " + countryName);
            }
        } else
            _countries = map.countries.ToDictionary(c => c.name);

        InitWorld();

        allCountriesInitialized = true;
        allCountriesInitializedEvent();
    }

    #region Init World
    public void InitWorld() {
        foreach (KeyValuePair<string, Country> c in _countries) {
            JSONNode countryData = DataManager.GetCountryData(c.Key);
            if (countryData == null) {
                Debug.LogError("Couldn't find " + c.Key + "inside JSON File");
                return;
            }

            InitContinent(countryData);
            InitSubregion(countryData, c.Value);
            InitPopulation(countryData, c.Value);
            InitMeasurements(c.Value);
        }

        InitCountryWorkers();
    }

    public void InitContinent(JSONNode countryData) {
        string continentName = countryData["region"];

        if (!_continents.ContainsKey(continentName))
            _continents.Add(continentName, new Continent(continentName));
    }

    public void InitSubregion(JSONNode countryData, Country country) {
        string subregionName = countryData["subregion"];
        string subregionContinentName = countryData["region"];

        Continent continent;
        if (_continents.TryGetValue(subregionContinentName, out continent)) {
            Subregion subregion;
            if (!continent.Subregions.TryGetValue(subregionName, out subregion)) {
                subregion = new Subregion(subregionName, continent);
                continent.Subregions.Add(subregionName, subregion);
            }
            country.Subregion = subregion;
            subregion.Countries.Add(country.name, country);
            if (!Subregions.ContainsKey(subregionName))
                Subregions.Add(subregionName, subregion);
        }

        if (subregionName.Equals(""))
            Debug.LogError("Country: " + country.name + " has no Subregion| (CountryManager.cs : InitializeCountries()");
    }

    public void InitCountryWorkers() {
        foreach (KeyValuePair<string, Country> element in _countries) {
            CountryWorker cw;
            cw = new CountryWorker(element.Value, DataManager.GetCountryCovidDayOneData(element.Value.name));
            _countryWorkers.Add(cw);
        }
    }

    public void InitPopulation(JSONNode countryData, Country country) {
        country.Population = countryData["population"];

        if (country.Population == 0)
            Debug.LogError("Country: " + country.name + " has 0 Population | (CountryManager.cs : InitializeCountries()");
    }

    private void InitMeasurements(Country country) {
        foreach (MeasurementSO mso in MeasurementManager.Instance.generalMeasurements)
            country.AddMeasurement(new Measurement(mso, country));

        foreach (Measurement m in country.Measurements)
            m.FindSiblings();
    }
    #endregion

    #region Value Getter Setter
    public double GetTotalEconomyAbs() {
        double totalEconomy = 0;

        foreach (KeyValuePair<string, Country> element in _countries) {
            totalEconomy += Math.Abs(element.Value.Economy.TotalValue);
        }

        return totalEconomy;
    }

    public void DistributeVaccines() {
        foreach (KeyValuePair<string, Country> element in _countries) {
            element.Value.Vaccination.DistributeVaccines();
        }

        CountryCanvas.instance.UpdateStatistic();
    }

    public double GetGlobalValue(Value type) {
        double value = 0;

        foreach (KeyValuePair<string, Continent> element in _continents)
            value += GetContinentValue(element.Key, type);

        if (type.Equals(Value.Happiness) || type.Equals(Value.Revolt))
            value /= Continents.Count;

        return value;
    }

    public double GetContinentValue(string name, Value type) {
        double value = 0;
        Continent continent;
        if (_continents.TryGetValue(name, out continent)) {
            foreach (KeyValuePair<string, Subregion> element in continent.Subregions)
                value += GetSubregionValue(element.Key, type);
        } else
            Debug.LogError("Could not find Continent: " + name + " | (GlobalManager.cs : GetContinentValue(...))");

        if (type.Equals(Value.Happiness) || type.Equals(Value.Revolt))
            value /= continent.Subregions.Count;

        return value;
    }

    public double GetSubregionValue(string name, Value type) {
        double value = 0;
        Subregion subregion;
        if (_subregions.TryGetValue(name, out subregion)) {
            foreach (KeyValuePair<string, Country> element in subregion.Countries)
                value += GetCountryValue(element.Key, type);
        } else
            Debug.LogError("Could not find Subregion: " + name + " | (GlobalManager.cs : GetSubregionValue(...))");

        if (type.Equals(Value.Happiness) || type.Equals(Value.Revolt))
            value /= subregion.Countries.Count;

        return value;
    }

    public double GetCountryValue(string name, Value type) {
        double value = 0;
        Country country;
        if (_countries.TryGetValue(name, out country)) {
            switch (type) {
                case Value.Population:
                    value += country.Population;
                    break;
                case Value.Infection:
                    value += country.Infection.TotalValue;
                    break;
                case Value.Fatality:
                    value += country.Fatality.TotalValue;
                    break;
                case Value.Vaccination:
                    value += country.Vaccination.TotalValue;
                    break;
                case Value.Recovery:
                    value += country.Recovery.TotalValue;
                    break;
                case Value.Economy:
                    value += country.Economy.TotalValue;
                    break;
                case Value.Happiness:
                    value += country.Happiness.TotalValue;
                    break;
                case Value.Revolt:
                    value += country.Revolt.TotalValue;
                    break;
                default:
                    Debug.LogError("Could not match View type: " + type.ToString() + " | (GlobalManager.cs : GetCountryValue(...))");
                    break;
            }
        } else
            Debug.LogError("Could not find Country: " + name + " | (GlobalManager.cs : GetCountryValue(...))");

        return value;
    }
    #endregion

    #region Serialisation
    public void PopulateSaveData(SaveData saveData) {
        SaveData.GlobalManagerData gmd = new SaveData.GlobalManagerData(this);

        saveData.globalManagerData = gmd;
    }

    public void LoadFromSaveData(SaveData saveData) {
        SaveData.GlobalManagerData gmd = saveData.globalManagerData;

        foreach (SaveData.CountryData cd in gmd.countryDatas) {
            Country c = null;
            if (_countries.TryGetValue(cd.name, out c)) {
                c.LoadFromSaveData(cd);
            } else {
                Debug.LogError("Loading Failed: CountryData Name does not exist in Dicionary");
            }
        }

        GlobalCanvas.Instance.UpdateUI();
        ViewManager.Instance.FilterBy(ViewManager.View.Infection);
        ProgressCanvas.Instance.UpdateGameOverProgress();
    }
    #endregion
}
