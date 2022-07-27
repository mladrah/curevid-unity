using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine;
using SimpleJSON;
using DG.Tweening;
using WPMF;

public class CountryWorker
{
    private readonly Country country;
    public Country Country { get => country; }

    private JSONNode countryData;

    #region Custom Simulation Fields
    private DateTime dayOneDate;
    #endregion

    #region Real Simulation Fields
    private bool realSimulation;
    private int i = 0;
    #endregion

    #region Visual Fields
    public CountryDecorator countryDecorator;
    #endregion

    public CountryWorker(Country country, JSONNode countryData, bool realSimulation = false) {
        this.country = country;
        this.country.Initialize();
        this.country.Worker = this;
        this.countryData = countryData;
        this.realSimulation = realSimulation;
        DateTime.TryParse(countryData[0]["Date"], out this.dayOneDate);

        countryDecorator = new CountryDecorator(country.name);
        WorldMap2D.instance.decorator.SetCountryDecorator(0, Country.name, countryDecorator);

        TimeManager.Instance.dailyUpdateDelegate += DailyCountryUpdate;
        Country.economyUpdateEvent += UpdateEconomy;

        UpdateEconomy();
    }

    #region Visual Methods
    public void Visualize(ViewManager.View type) {
        if (type != ViewManager.Instance.currentView)
            return;
        switch (ViewManager.Instance.currentView) {
            case ViewManager.View.Infection:
                ViewManager.Instance.VisualizeInfection(this);
                break;
            case ViewManager.View.Economy:
                ViewManager.Instance.VisualizeEconomy(this);
                break;
            case ViewManager.View.Happiness:
                ViewManager.Instance.VisualizeHappiness(this);
                break;
            case ViewManager.View.Revolt:
                ViewManager.Instance.VisualizeRevolt(this);
                break;
            case ViewManager.View.Vaccination:
                ViewManager.Instance.VisualizeVaccination(this);
                break;
            case ViewManager.View.Measurement:
                ViewManager.Instance.VisualizeMeasurement(this);
                break;
            default:
                Debug.LogError("Could not match Visualization ModifierType | (CountryWorker.cs : Visualize(...))");
                break;
        }
    }
    #endregion

    public void DailyCountryUpdate() {
        if (realSimulation)
            ExactSimulation();
        else
            CustomSimulation();
    }

    #region Real Infection
    public void ExactSimulation() {
        float infected = countryData[i]["Cases"];
        country.Infection.NewTotalValue(infected);
        float infectionRate = (infected * 50) / (float)country.Population;
        if (infectionRate > 0.05f) { // Fixes: Country Color gets White
            DOTween.To(() => countryDecorator.fillColor.g, x => countryDecorator.fillColor.g = x, 1 - infectionRate, 1);
            DOTween.To(() => countryDecorator.fillColor.b, x => countryDecorator.fillColor.b = x, 1 - infectionRate, 1);
        }
        i++;
    }
    #endregion

    #region Custom Infection
    public void CustomSimulation() {

        if (!country.Infection.IsInfected)
            isToday();
        else {
            UpdateInfection();
        }

        UpdateRecovery();
        UpdateVaccination();
        UpdateFatality();

        UpdateEconomy();

        UpdateTraits();

        UpdateHappiness();
        UpdateRevolt();

        UpdateOpenPopulation();

        country.InvokeGeneralUpdateEvent();

        if (country.Infection.TotalValue > country.Population)
            Debug.Log(country.name);
        //country.Infection.OutputInfectionHistory();
        //if (country.name == "China")
        //    Debug.Log(Format.LargeNumber(country.OpenPopulation));
    }

    public void UpdateInfection() {
        country.Infection.UpdateNeighborInfluence();
        country.Infection.UpdateRValue();

        double newCases = country.Infection.NewCases * (double)country.Infection.RValue;
        if (newCases > country.OpenPopulation) {
            country.Infection.Infect(new Cases(country.OpenPopulation, TimeManager.Instance.GetCurrentDate()));
            country.Infection.StopInfection();
        } else {
            country.Infection.Infect(new Cases(newCases, TimeManager.Instance.GetCurrentDate()));
        }

        Visualize(ViewManager.View.Infection);
    }

    public void UpdateRecovery() {
        if (country.Infection.InfectionHistory.Count > 0) {
            Cases oldCases = country.Infection.InfectionHistory.Peek();

            //Debug.Log(oldCases.count + " " + oldCases.date + " | " + country.Infection.InfectionHistory.Count);

            if ((TimeManager.Instance.GetCurrentDate() - oldCases.date).TotalDays >= Recovery.RECOVERY_START) {
                country.Infection.InfectionHistory.Dequeue();
                country.Infection.ChangeTotalValueBy(-oldCases.count);
                country.Recovery.ChangeTotalValueBy(oldCases.count);
            }
        } else {
            if (country.OpenPopulation < 0 && country.Vaccination.IsDistributed)
                country.Recovery.ChangeTotalValueBy(country.OpenPopulation);
        }
    }

    public void UpdateVaccination() {
        if (!country.Vaccination.IsDistributed)
            return;

        double oldValue = country.Vaccination.TotalValue;
        country.Vaccination.Vaccinate();
        if (oldValue != country.Vaccination.TotalValue)
            Visualize(ViewManager.View.Vaccination);
    }

    public void UpdateFatality() {
        country.Fatality.SumNewFatalities();
        country.Fatality.UpdateCrudeMortalityRate();
    }

    public void UpdateMeasurement(Measurement measurement) {
        if (measurement.IsActive)
            country.AddTrait(new Trait(measurement.Name, measurement.Description, measurement.ActiveDuration, measurement.Icon, measurement.Modifiers));
        else
            country.RemoveTrait(measurement.Name);

        country.InvokeMeasurementEvent();
        Visualize(ViewManager.View.Measurement);
    }

    public void UpdateEconomy() {
        float newEconomy = Calculate.Stat(country.Economy);
        if (country.Economy.TotalValue != newEconomy) {
            country.Economy.NewTotalValue(newEconomy);
            country.InvokeEconomyEvent();
            Visualize(ViewManager.View.Economy);
        }
    }

    public void UpdateTraits() {
        List<String> toDelete = new List<String>();

        for (int i = 0; i < country.GetTraitCount(); i++) {
            Trait trait = country.Traits[i];
            if (trait.ExpirationDate != DateTime.MinValue && DateTime.Compare(TimeManager.Instance.GetCurrentDate(), trait.ExpirationDate) >= 0) {
                toDelete.Add(trait.Name);
            }
        }

        for (int i = 0; i < toDelete.Count; i++) {
            country.RemoveTrait(toDelete[i]);
        }

        country.InvokeTraitEvent();
    }

    public void UpdateHappiness() {
        float newHappiness = Calculate.Stat(country.Happiness);
        if (newHappiness != 0) {
            double oldValue = country.Happiness.TotalValue;
            country.Happiness.ChangeTotalValueBy(newHappiness);
            if (country.Happiness.TotalValue < 0)
                country.Happiness.NewTotalValue(0);
            else if (country.Happiness.TotalValue > 100)
                country.Happiness.NewTotalValue(100);
            country.InvokeHappinessEvent();

            if (oldValue != country.Happiness.TotalValue)
                Visualize(ViewManager.View.Happiness);
        }

        if (country.Happiness.IsAbove && !country.Happiness.IsHappy) {
            LogManager.Log("Population is Happy!", country.name, Colors.HEX_GREEN);
            country.AddTrait(country.Happiness.HappyTrait);
        } else if (country.Happiness.IsBelow && !country.Happiness.IsAngry) {
            LogManager.Log("Population is Unhappy!", country.name, Colors.HEX_RED);
            country.AddTrait(country.Happiness.AngryTrait);
        } else {
            if (!country.Happiness.IsAbove && country.Happiness.IsHappy)
                country.RemoveTrait(country.Happiness.HappyTrait.Name);
            else if (!country.Happiness.IsBelow && country.Happiness.IsAngry)
                country.RemoveTrait(country.Happiness.AngryTrait.Name);
        }
    }

    public void UpdateRevolt() {
        float newRevolt = Calculate.Stat(country.Revolt);
        if (newRevolt != 0) {
            double oldTv = country.Revolt.TotalValue;
            country.Revolt.ChangeTotalValueBy(newRevolt);
            if (country.Revolt.TotalValue < 0)
                country.Revolt.NewTotalValue(0);
            else if (country.Revolt.TotalValue > 100)
                country.Revolt.NewTotalValue(100);
            country.InvokeHappinessEvent();

            if (oldTv != country.Revolt.TotalValue)
                Visualize(ViewManager.View.Revolt);
        }

        if (country.Revolt.IsAbove1 && !country.Revolt.Is1Active) {
            LogManager.Log("Revolt Phase 1 Active!", country.name, Colors.HEX_RED);
            country.AddTrait(country.Revolt.TraitPhase1);
        } else if (!country.Revolt.IsAbove1 && country.Revolt.Is1Active)
            country.RemoveTrait(country.Revolt.TraitPhase1.Name);

        if (country.Revolt.IsAbove2 && !country.Revolt.Is2Active) {
            LogManager.Log("Revolt Phase 2 Active!", country.name, Colors.HEX_RED);
            country.AddTrait(country.Revolt.TraitPhase2);
        } else if (!country.Revolt.IsAbove2 && country.Revolt.Is2Active)
            country.RemoveTrait(country.Revolt.TraitPhase2.Name);

        if (country.Revolt.IsAbove3 && !country.Revolt.Is3Active) {
            LogManager.Log("Revolt Phase 3 Active!", country.name, Colors.HEX_RED);
            country.AddTrait(country.Revolt.TraitPhase3);
        } else if (!country.Revolt.IsAbove3 && country.Revolt.Is3Active)
            country.RemoveTrait(country.Revolt.TraitPhase3.Name);
    }

    public void UpdateOpenPopulation() {
        country.OpenPopulation = country.Population - country.Infection.TotalValue - country.Recovery.TotalValue - country.Fatality.TotalValue;
        //Debug.Log(Format.LargeNumber(country.OpenPopulation) + " = " +
        //    Format.LargeNumber(country.Population) + " - " +
        //    Format.LargeNumber(country.Infection.TotalValue) + "(I) - " +
        //    Format.LargeNumber(country.Recovery.TotalValue) + "(R) - " +
        //    Format.LargeNumber(country.Fatality.TotalValue) + "(F)");
    }

    public void isToday() {
        if (TimeManager.Instance.day >= dayOneDate.Day && TimeManager.Instance.month >= dayOneDate.Month && TimeManager.Instance.year >= dayOneDate.Year) {
            double newCases = ++country.Infection.NewCases * country.Infection.RValue;
            country.Infection.Infect(new Cases(newCases, TimeManager.Instance.GetCurrentDate()));
        }
    }
    #endregion
}

