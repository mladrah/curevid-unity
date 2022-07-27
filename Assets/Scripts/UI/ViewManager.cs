using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using WPMF;

public class ViewManager : MonoBehaviour
{
    public enum View
    {
        Infection,
        Economy,
        Happiness,
        Revolt,
        Vaccination,
        Measurement,
        Subregion,
        Continent
    }

    private static ViewManager _instance;
    public static ViewManager Instance { get => _instance; }

    public View currentView;
    private List<CountryWorker> workers;

    public GameObject subfilterButtonPrefab;

    public ScriptableObject currentReference;

    private void Awake() {
        _instance = this;
    }

    private void Start() {
        GlobalManager.Instance.allCountriesInitializedEvent += OnWorkersInitialized;
    }

    public void FilterBy(View view, ScriptableObject reference = null) {
        currentReference = reference;
        workers = GlobalManager.Instance.CountryWorkers;
        currentView = view;
        //VisualizeStandard();

        switch (currentView) {
            case View.Infection:
                FilterAll(VisualizeInfection);
                break;
            case View.Economy:
                FilterAll(VisualizeEconomy);
                break;
            case View.Happiness:
                FilterAll(VisualizeHappiness);
                break;
            case View.Revolt:
                FilterAll(VisualizeRevolt);
                break;
            case View.Vaccination:
                FilterAll(VisualizeVaccination);
                break;
            case View.Measurement:
                FilterAll(VisualizeMeasurement, reference);
                break;
            case View.Subregion:
                FilterAll(VisualizeSubregion);
                break;
            case View.Continent:
                FilterAll(VisualizeContinent);
                break;
            default:
                Debug.LogError("Could not match View | (ViewManager.cs : FilterBy(...))");
                break;
        }
    }

    private void FilterAll(Action<CountryWorker, bool> visualization) {
        foreach (CountryWorker cw in workers) {
            visualization(cw, false);
        }
    }

    private void FilterAll(Action<CountryWorker, ScriptableObject> visualization, ScriptableObject reference) {
        foreach (CountryWorker cw in workers) {
            visualization(cw, reference);
        }
    }

    private void OnWorkersInitialized() {
        foreach (CountryWorker cw in GlobalManager.Instance.CountryWorkers) {
            cw.countryDecorator.isColorized = true;
            cw.countryDecorator.fillColor = Colors.WHITE;
        }

        FilterBy(currentView);
    }

    public void VisualizeInfection(CountryWorker cw, bool tweening = true) {

        float infectionRate = (float)(cw.Country.Infection.TotalValue / cw.Country.Population);
        //if (infectionRate > 0.05f) { // Fixes: Country Color gets White
        float r = Colors.WHITE.r;
        float g = Colors.WHITE.g - infectionRate;
        float b = Colors.WHITE.b - infectionRate;

        if (g <= 0.45f) {
            g = 0.45f;
            b = 0.45f;
        }
        SetColor(cw, r, g, b);
        //}
    }

    public void VisualizeEconomy(CountryWorker cw, bool tweening = true) {
        //Debug.Log("vis eco");
        float economyRate = (float)(cw.Country.Economy.TotalValue / GlobalManager.Instance.GetTotalEconomyAbs());
        economyRate *= 10;

        float r = 1;
        float g = 1;
        float b = 1;

        if (economyRate > 0) {
            r = 1 - economyRate;
            b = 1 - economyRate;
        } else {
            g = 1 + economyRate;
            b = 1 + economyRate;
        }

        SetColor(cw, r, g, b);
    }

    public void VisualizeHappiness(CountryWorker cw, bool tweening = true) {
        //Debug.Log("vis hap");
        float hapRate = (float)(cw.Country.Happiness.TotalValue);
        hapRate = hapRate / 100;
        hapRate -= 0.5f;

        float r = 1;
        float g = 1;
        float b = 0;
        if (hapRate > 0) {
            r -= hapRate;
        } else {
            g += hapRate;
        }

        SetColor(cw, r, g, b);
    }

    public void VisualizeRevolt(CountryWorker cw, bool tweening = true) {
        //Debug.Log("vis rev");
        float revRate = (float)(cw.Country.Revolt.TotalValue);
        revRate = revRate / 2.5f;
        revRate = revRate / 100f;
        float r = 1.1f - revRate;
        float g = 1 - revRate;
        float b = 1 - revRate;
        SetColor(cw, r, g, b);
    }

    public void VisualizeVaccination(CountryWorker cw, bool tweening = true) {
        //Debug.Log("vis vac");
        float vacRate = (float)(cw.Country.Vaccination.TotalValue / cw.Country.Population);
        float r = 0.95f;
        float g = 1;
        float b = 1;
        if (vacRate > 0.05f) { // Fixes: Country Color gets White
            r -= vacRate;
        }
        SetColor(cw, r, g, b);
    }

    public void VisualizeMeasurement(CountryWorker cw, ScriptableObject reference = null) {
        if (reference == null) {
            reference = currentReference;
            if (currentReference == null)
                Debug.Log("Current ist null");
        }
        float r = 1f;
        float g = 1f;
        float b = 1f;

        MeasurementSO mso = (MeasurementSO)reference;
        Measurement foundM = null;

        foreach (Measurement m in cw.Country.Measurements) {
            if (m.Name.Equals(mso.Name)) {
                foundM = m;
                break;
            }
        }

        if (foundM.IsActive) {
            r = 0.9f;
            g = 0.75f;
        } else if (foundM.Family.Count > 0) {
            for (int i = 0; i < foundM.Family.Count; i++) {
                if (foundM.Family[i].IsActive) {
                    r = 0.9f - ((1 + i) * 0.1f);
                    g = 0.75f - ((1 + i) * 0.175f);
                }
            }
        }

        SetColor(cw, r, g, b);
    }

    public void VisualizeSubregion(CountryWorker cw, bool tweening = false) {
        Color subregionColor = RandomColorByString(cw.Country.Subregion.Name, 100);
        SetColor(cw, subregionColor.r, subregionColor.g, subregionColor.b);
    }

    public void VisualizeContinent(CountryWorker cw, bool tweening = false) {
        Color color = Colors.WHITE;

        switch (cw.Country.Subregion.Continent.Name) {
            case "Americas": color = Colors.YELLOW;
                break;
            case "Europe": color = Colors.RED;
                break;
            case "Africa": color = Colors.BLUE;
                break;
            case "Asia": color = Colors.GREEN;
                break;
            case "Oceania": color = Colors.PURPLE;
                break;
            default: Debug.LogError("Could not match: " + cw.Country.Subregion.Continent.Name + " to Continent | (ViewManger.cs : VisualizeContinent(...))");
                break;
        }

        SetColor(cw, color.r, color.g, color.b);
    }

    private Color RandomColorByString(string seed, int min) {
        System.Random random = new System.Random(seed.GetHashCode());
        Color background = new Color(
            (float)random.Next(min, 255),
            (float)random.Next(min, 255),
            (float)random.Next(min, 255)
        );
        return new Color(background.r / 255, background.g / 255, background.b / 255);
    }

    public void SetColor(CountryWorker cw, float r, float g, float b, float a = 1) {
        cw.countryDecorator.fillColor.r = r;
        cw.countryDecorator.fillColor.g = g;
        cw.countryDecorator.fillColor.b = b;
        cw.countryDecorator.fillColor.a = a;
    }
}
