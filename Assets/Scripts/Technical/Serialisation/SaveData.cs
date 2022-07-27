using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using WPMF;
using Game;

[System.Serializable]
public class SaveData
{
    [System.Serializable]
    public struct GlobalManagerData
    {
        public List<CountryData> countryDatas;

        public GlobalManagerData(GlobalManager globalManager) {
            countryDatas = globalManager.Countries.Select(c => new CountryData(c.Value)).ToList();
        }

    }
    [System.Serializable]
    public struct CountryData
    {
        public string name;
        public double openPopulation;

        public CountryInfectionData infection;
        public CountryFatalityData fatality;
        public StatData recovery;
        public CountryVaccinationData vaccination;
        public StatData economy;
        public CountryHappinessData happiness;
        public CountryRevoltData revolt;

        public List<CountryTraitData> traits;
        public List<CountryMeasurementData> measurements;

        public CountryData(Country country) {

            this.name = country.name;
            this.openPopulation = country.OpenPopulation;

            this.infection = new CountryInfectionData(country.Infection);
            this.fatality = new CountryFatalityData(country.Fatality);
            this.recovery = new StatData(country.Recovery);
            this.vaccination = new CountryVaccinationData(country.Vaccination);
            this.economy = new StatData(country.Economy);
            this.happiness = new CountryHappinessData(country.Happiness);
            this.revolt = new CountryRevoltData(country.Revolt);

            this.traits = country.Traits.Select(t => new CountryTraitData(t)).ToList();
            this.measurements = country.Measurements.Select(m => new CountryMeasurementData(m.Name, m.IsActive)).ToList();
        }
    }

    [System.Serializable]
    public struct CountryTraitData
    {
        public string name;
        public string description;
        public Sprite icon;
        public List<Modifier> modifiers;
        public TimeData expirationDate;

        public CountryTraitData(Trait trait) {
            this.name = trait.Name;
            this.description = trait.Description;
            this.icon = trait.Icon;
            this.modifiers = trait.Modifiers;
            this.expirationDate = new TimeData(0, 0, trait.ExpirationDate.Day, trait.ExpirationDate.Month, trait.ExpirationDate.Year);
        }
    }

    [System.Serializable]
    public struct CountryMeasurementData
    {
        public string name;
        public bool isActive;

        public CountryMeasurementData(string name, bool isActive) {
            this.name = name;
            this.isActive = isActive;
        }
    }

    [System.Serializable]
    public struct CountryInfectionData
    {
        public StatData baseStat;

        public double rValue;
        public List<StatData> societyList;
        public double recoveryRInfluence;
        public double vaccinatedRInfluence;

        public double newCases;
        public List<Cases> infectionHistory;
        public double incidenceRate;
        public List<Cases> incidenceHistory;

        public bool isStopped;
        public bool isInfected;

        public CountryInfectionData(Infection infection) {
            this.baseStat = new StatData(infection);

            this.rValue = infection.RValue;
            this.societyList = infection.SocietyDictionary.Select(sd => new StatData(sd.Value)).ToList();
            this.recoveryRInfluence = infection.RecoveryRInfluence;
            this.vaccinatedRInfluence = infection.VaccinatedRInfluence;

            this.newCases = infection.NewCases;
            this.infectionHistory = infection.InfectionHistory.ToList();
            this.incidenceRate = infection.IncidenceRate;
            this.incidenceHistory = infection.IncidenceHistory.ToList();

            this.isStopped = infection.IsStopped;
            this.isInfected = infection.IsInfected;
        }
    }

    [System.Serializable]
    public struct CountryFatalityData
    {
        public StatData baseStat;

        public float caseFatalityRate;
        public double crudeMortalityRate;
        public double newFatalities;

        public double virusSource;
        public double cureSource;

        public CountryFatalityData(Fatality fatality) {
            this.baseStat = new StatData(fatality);

            this.caseFatalityRate = fatality.CaseFatalityRate;
            this.crudeMortalityRate = fatality.CrudeMortalityRate;
            this.newFatalities = fatality.NewFatalities;

            this.virusSource = fatality.VirusSource;
            this.cureSource = fatality.CureSource;
        }
    }

    [System.Serializable]
    public struct CountryVaccinationData
    {
        public StatData baseStat;

        public double newVaccinated;
        public double cureCaseFatalityRate;
        public double sideEffectFatalities;

        public bool isDistributed;

        public CountryVaccinationData(Vaccination vaccination) {
            this.baseStat = new StatData(vaccination);

            this.newVaccinated = vaccination.NewVaccinated;
            this.cureCaseFatalityRate = vaccination.CureCaseFatalityRate;
            this.sideEffectFatalities = vaccination.SideEffectFatalities;

            this.isDistributed = vaccination.IsDistributed;
        }
    }

    [System.Serializable]
    public struct CountryHappinessData
    {
        public StatData baseStat;

        public bool isAbove;
        public bool isBelow;

        public bool isHappy;
        public bool isAngry;

        public CountryHappinessData(Happiness happiness) {
            this.baseStat = new StatData(happiness);

            this.isAbove = happiness.IsAbove;
            this.isBelow = happiness.IsBelow;

            this.isHappy = happiness.IsHappy;
            this.isAngry = happiness.IsAngry;
        }
    }

    [System.Serializable]
    public struct CountryRevoltData
    {
        public StatData baseStat;

        public bool isAbove1;
        public bool isAbove2;
        public bool isAbove3;

        public bool is1Active;
        public bool is2Active;
        public bool is3Active;

        public CountryRevoltData(Revolt revolt) {
            this.baseStat = new StatData(revolt);

            this.isAbove1 = revolt.IsAbove1;
            this.isAbove2 = revolt.IsAbove2;
            this.isAbove3 = revolt.IsAbove3;

            this.is1Active = revolt.Is1Active;
            this.is2Active = revolt.Is2Active;
            this.is3Active = revolt.Is3Active;
        }
    }

    [System.Serializable]
    public struct StatData
    {
        public string name;
        public double totalValue;
        public List<Modifier> modifiers;

        public StatData(Stat stat) {
            this.name = stat.Name;
            this.totalValue = stat.TotalValue;
            this.modifiers = stat.Modifiers;
        }
    }

    [System.Serializable]
    public struct TimeData
    {
        public double minute, hour;

        public int day, month, year;

        public TimeData(TimeManager timeManager) {
            this.minute = timeManager.minute;
            this.hour = timeManager.hour;

            this.day = timeManager.day;
            this.month = timeManager.month;
            this.year = timeManager.year;
        }

        public TimeData(double minute, double hour, int day, int month, int year) {
            this.minute = minute;
            this.hour = hour;

            this.day = day;
            this.month = month;
            this.year = year;
        }
    }

    [System.Serializable]
    public struct ResearchManagerData
    {
        public List<ResearchData> researchDatas;
        public int daysPassed;

        public ResearchManagerData(ResearchManager researchManager) {
            this.researchDatas = researchManager.Researches.Select(r => new ResearchData(r)).ToList();
            this.daysPassed = researchManager.DaysPassed;
        }
    }

    [System.Serializable]
    public struct ResearchData
    {
        public string name;
        public bool canBeResearched;
        public bool isResearching;
        public bool isResearched;

        public ResearchData(Research research) {
            this.name = research.Name;
            this.canBeResearched = research.CanBeResearched;
            this.isResearching = research.IsResearching;
            this.isResearched = research.IsResearched;
        }
    }

    [System.Serializable]
    public struct CureData
    {
        public StatData effectivity;
        public StatData safety;

        public CureData(Cure cure) {
            effectivity = new StatData(cure.Effectivity);
            safety = new StatData(cure.Safety);
        }
    }

    [System.Serializable]
    public struct ResourceManagerData
    {
        public StatData globalEconomy;

        public ResourceManagerData(ResourceManager resourceManager) {
            this.globalEconomy = new StatData(resourceManager.GlobalEconomy);
        }
    }

    [System.Serializable]
    public struct EventManagerData
    {
        public List<EventData> eventDatas;

        public EventManagerData(EventManager eventManager) {
            this.eventDatas = eventManager.EventList.Select(e => new EventData(e)).ToList();
        }
    }

    [System.Serializable]
    public struct EventData
    {
        public string title;
        public bool isOccured;

        public EventData(Game.Event gameEvent) {
            this.title = gameEvent.Title;
            this.isOccured = gameEvent.IsOccured;
        }
    }

    public GlobalManagerData globalManagerData;
    public TimeData timeData;
    public ResourceManagerData resourceManagerData;
    public ResearchManagerData researchManagerData;
    public EventManagerData eventManagerData;
    public CureData cureData;

    public string ToJson() {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string a_Json) {
        JsonUtility.FromJsonOverwrite(a_Json, this);
    }
}

public interface ISaveable
{
    void PopulateSaveData(SaveData saveData);
    void LoadFromSaveData(SaveData saveData);
}