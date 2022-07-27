using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WPMF;

[System.Serializable]
public struct Cases
{
    public double count;
    public System.DateTime date;
    public int day;
    public int month;
    public int year;

    public Cases(double count, System.DateTime date) {
        this.count = count;

        this.date = date;
        this.day = date.Day;
        this.month = date.Month;
        this.year = date.Year;
    }
}

public class Infection : Stat
{
    private readonly Country _country;

    #region Properties
    private double _rValue;
    public double RValue { get => _rValue; }

    private Dictionary<string, Stat> _societyDictionary;
    public Dictionary<string, Stat> SocietyDictionary { get => _societyDictionary; }

    private double _recoveryRInfluence;
    public double RecoveryRInfluence { get => _recoveryRInfluence; }

    private double _vaccinatedRInfluence;
    public double VaccinatedRInfluence { get => _vaccinatedRInfluence; }

    private double _newCases;
    public double NewCases { get => _newCases; set => _newCases = value; }

    private Queue<Cases> _infectionHistory;
    public Queue<Cases> InfectionHistory { get => _infectionHistory; }

    private double _incidenceRate;
    public double IncidenceRate { get => _incidenceRate; }

    private Queue<Cases> _incidenceHistory;
    public Queue<Cases> IncidenceHistory { get => _incidenceHistory; }

    private const int INCIDENCE_HISTORY_DAY_COUNT = 7;

    private bool _isStopped;
    public bool IsStopped { get => _isStopped; }

    private bool _isInfected;
    public bool IsInfected { get => _isInfected; }

    #endregion

    public Infection(Country country) {
        //Random.seed = 135468;

        _country = country;
        _societyDictionary = new Dictionary<string, Stat>();
        _infectionHistory = new Queue<Cases>();
        _incidenceHistory = new Queue<Cases>();

        _societyDictionary.Add("Socialising", new Stat("Socialising", new Modifier("Base Value", Modifier.effectType.ADDITIVE, Modifier.modifierType.INFECTION, Modifier.valueType.NEGATIVE, Random.Range(0.5f, 0.7f), Modifier.subModifierType.Infection_Socialising)));
        _societyDictionary.Add("Education", new Stat("Education", new Modifier("Base Value", Modifier.effectType.ADDITIVE, Modifier.modifierType.INFECTION, Modifier.valueType.NEGATIVE, Random.Range(0.2f, 0.3f), Modifier.subModifierType.Infection_Education)));
        _societyDictionary.Add("Work", new Stat("Work", new Modifier("Base Value", Modifier.effectType.ADDITIVE, Modifier.modifierType.INFECTION, Modifier.valueType.NEGATIVE, Random.Range(0.2f, 0.3f), Modifier.subModifierType.Infection_Work)));
        _societyDictionary.Add("Activities", new Stat("Activities", new Modifier("Base Value", Modifier.effectType.ADDITIVE, Modifier.modifierType.INFECTION, Modifier.valueType.NEGATIVE, Random.Range(0.4f, 0.6f), Modifier.subModifierType.Infection_Activities)));
        _societyDictionary.Add("Gatherings", new Stat("Gatherings", new Modifier("Base Value", Modifier.effectType.ADDITIVE, Modifier.modifierType.INFECTION, Modifier.valueType.NEGATIVE, Random.Range(0.4f, 0.6f), Modifier.subModifierType.Infection_Gatherings)));
        _societyDictionary.Add("Revolt", new Stat("Revolt", new Modifier("Base Value", Modifier.effectType.ADDITIVE, Modifier.modifierType.INFECTION, Modifier.valueType.NEGATIVE, 0, Modifier.subModifierType.Infection_Revolt)));

        List<Modifier> modifiers = new List<Modifier>();
        foreach (Country c in _country.NeighboringCountries)
            modifiers.Add(new Modifier(c.name, Modifier.effectType.ADDITIVE, Modifier.modifierType.INFECTION, Modifier.valueType.NEGATIVE, 0, Modifier.subModifierType.Infection_Neighboring_Countries));

        _societyDictionary.Add("Neighboring Countries", new Stat("Neighboring Countries", modifiers));

        UpdateRValue();


    }

    #region R Value Methods
    public override void AddModifier(Modifier modifier) {
        Modifier newModifier = new Modifier(modifier.Name, modifier.EffectType, modifier.ModifierType, modifier.ValueType, modifier.Value, modifier.SubModifierType);
        Stat stat = null;

        switch (newModifier.SubModifierType) {
            case Modifier.subModifierType.Infection_Socialising:
                if (_societyDictionary.TryGetValue("Socialising", out stat))
                    stat.AddModifier(newModifier);
                else
                    Debug.LogError("Could not find 'Socialising' in SocietyDictionary");
                break;
            case Modifier.subModifierType.Infection_Education:
                if (_societyDictionary.TryGetValue("Education", out stat))
                    stat.AddModifier(newModifier);
                else
                    Debug.LogError("Could not find 'Education' in SocietyDictionary");
                break;
            case Modifier.subModifierType.Infection_Work:
                if (_societyDictionary.TryGetValue("Work", out stat))
                    stat.AddModifier(newModifier);
                else
                    Debug.LogError("Could not find 'Work' in SocietyDictionary");
                break;
            case Modifier.subModifierType.Infection_Activities:
                if (_societyDictionary.TryGetValue("Activities", out stat))
                    stat.AddModifier(newModifier);
                else
                    Debug.LogError("Could not find 'Activities' in SocietyDictionary");
                break;
            case Modifier.subModifierType.Infection_Gatherings:
                if (_societyDictionary.TryGetValue("Gatherings", out stat))
                    stat.AddModifier(newModifier);
                else
                    Debug.LogError("Could not find 'Gathering' in SocietyDictionary");
                break;
            case Modifier.subModifierType.Infection_Revolt:
                if (_societyDictionary.TryGetValue("Revolt", out stat))
                    stat.AddModifier(newModifier);
                else
                    Debug.LogError("Could not find 'Revolt' in SocietyDictionary");
                break;
            case Modifier.subModifierType.Infection_Neighboring_Countries:
                if (_societyDictionary.TryGetValue("Neighboring Countries", out stat))
                    stat.AddModifier(newModifier);
                else
                    Debug.LogError("Could not find 'Neighboring Countries' in SocietyDictionary");
                break;
            default:
                Debug.LogError("Could not match SubModifierType: " + newModifier.SubModifierType + " | (Infections.cs : AddModifier(...))");
                break;
        }

        UpdateRValue();
    }

    public override void RemoveModifier(Modifier modifier) {
        Modifier toRemoveModifier = new Modifier(modifier.Name, modifier.EffectType, modifier.ModifierType, modifier.ValueType, modifier.Value, modifier.SubModifierType);
        Stat stat = null;
        switch (toRemoveModifier.SubModifierType) {
            case Modifier.subModifierType.Infection_Socialising:
                if (_societyDictionary.TryGetValue("Socialising", out stat))
                    stat.RemoveModifier(toRemoveModifier);
                else
                    Debug.LogError("Could not find 'Socialising' in SocietyDictionary");
                break;
            case Modifier.subModifierType.Infection_Education:
                if (_societyDictionary.TryGetValue("Education", out stat))
                    stat.RemoveModifier(toRemoveModifier);
                else
                    Debug.LogError("Could not find 'Education' in SocietyDictionary");
                break;
            case Modifier.subModifierType.Infection_Work:
                if (_societyDictionary.TryGetValue("Work", out stat))
                    stat.RemoveModifier(toRemoveModifier);
                else
                    Debug.LogError("Could not find 'Work' in SocietyDictionary");
                break;
            case Modifier.subModifierType.Infection_Activities:
                if (_societyDictionary.TryGetValue("Activities", out stat))
                    stat.RemoveModifier(toRemoveModifier);
                else
                    Debug.LogError("Could not find 'Activities' in SocietyDictionary");
                break;
            case Modifier.subModifierType.Infection_Gatherings:
                if (_societyDictionary.TryGetValue("Gatherings", out stat))
                    stat.RemoveModifier(toRemoveModifier);
                else
                    Debug.LogError("Could not find 'Gathering' in SocietyDictionary");
                break;
            case Modifier.subModifierType.Infection_Revolt:
                if (_societyDictionary.TryGetValue("Revolt", out stat))
                    stat.RemoveModifier(toRemoveModifier);
                else
                    Debug.LogError("Could not find 'Revolt' in SocietyDictionary");
                break;
            case Modifier.subModifierType.Infection_Neighboring_Countries:
                if (_societyDictionary.TryGetValue("Neighboring Countries", out stat))
                    stat.RemoveModifier(toRemoveModifier);
                else
                    Debug.LogError("Could not find 'Neighboring Countries' in SocietyDictionary");
                break;
            default:
                Debug.LogError("Could not match SubModifierType: " + toRemoveModifier.SubModifierType + " | (Infections.cs : AddModifier(...))");
                break;
        }

        UpdateRValue();
    }

    public void UpdateRValue() {
        _rValue = 0;

        if (!_isInfected)
            return;


        foreach (KeyValuePair<string, Stat> element in _societyDictionary)
            _rValue += Calculate.Stat(element.Value);

        double rValueNegative = _rValue;

        if (_country.Recovery != null)
            _recoveryRInfluence = (_country.Recovery.TotalValue / _country.Population) * rValueNegative;

        if (_country.Vaccination != null) {
            _vaccinatedRInfluence = _country.Vaccination.TotalValue / _country.Population;
            _vaccinatedRInfluence *= (Calculate.StatMultiplicative(Cure.Instance.Effectivity));
            _vaccinatedRInfluence *= 1.4178;
        }

        _rValue -= _recoveryRInfluence - _vaccinatedRInfluence;

        if (_rValue < 0)
            _rValue = 0;
    }

    public void UpdateNeighborInfluence() {
        foreach (Country c in _country.NeighboringCountries) {
            if (c.Infection == null) {
                Debug.LogWarning("Neighbor not initialized");
                return;
            }

            float infectionRatio = ((float)(c.Infection.TotalValue / c.Population)) / _country.NeighboringCountries.Count;
            Stat stat = null;
            if (_societyDictionary.TryGetValue("Neighboring Countries", out stat))
                stat.UpdateModifier(c.name, infectionRatio);
            else {
                Debug.LogError("Could not find 'Neighboring Countries' Stat");
                return;
            }
        }
    }

    public double CalculateCauseShare(Stat cause) {
        if (RValue == 0)
            return 0;

        return (Calculate.Stat(cause) / RValue) * _newCases;
    }
    #endregion

    #region Infection Methods
    public void Infect(Cases cases) {
        if (Mathf.RoundToInt((float) cases.count) == 0 && _rValue > 1) {
            cases = new Cases(1, cases.date);
        }

        if (cases.count > 0) {
            cases = _country.Fatality.CauseFatalitiesVirus(cases);
            _infectionHistory.Enqueue(new Cases(cases.count, cases.date));
            _newCases = cases.count;
            ChangeTotalValueBy(cases.count);
        }
        CalculateIncidenceRate(cases);

        _isInfected = true;
    }

    private void CalculateIncidenceRate(Cases cases) {
        _incidenceHistory.Enqueue(cases);

        if (_incidenceHistory.Count > INCIDENCE_HISTORY_DAY_COUNT) {
            _incidenceHistory.Dequeue();
        }

        double sum = 0;
        foreach (Cases c in _incidenceHistory)
            sum += c.count;

        _incidenceRate = (sum / _country.Population) * 100000;
        _incidenceRate = _incidenceRate < 0 ? 0 : _incidenceRate;
    }

    public void StopInfection() {
        _newCases = 0;
        _isStopped = true;
    }

    public void OutputInfectionHistory() {
        Debug.Log("---------------------------");

        IEnumerator<Cases> enumerator =
                    _infectionHistory.GetEnumerator();
        while (enumerator.MoveNext()) {

            Debug.Log(Format.LargeNumber(enumerator.Current.count) + " " + enumerator.Current.date);
        }

        Debug.Log("---------------------------");
    }
    #endregion

    #region Serialisation
    public void LoadFromSaveData(SaveData.CountryInfectionData infectionData) {
        LoadFromSaveData(infectionData.baseStat);

        _rValue = infectionData.rValue;
        _societyDictionary = infectionData.societyList.ToDictionary(k => k.name, k => new Stat(k.name, k.modifiers));
        _recoveryRInfluence = infectionData.recoveryRInfluence;
        _vaccinatedRInfluence = infectionData.vaccinatedRInfluence;

        _newCases = infectionData.newCases;
        _infectionHistory = new Queue<Cases>(infectionData.infectionHistory.Select(c => new Cases(c.count, new System.DateTime(c.year, c.month, c.day))).ToList());

        _incidenceRate = infectionData.incidenceRate;
        _incidenceHistory = new Queue<Cases>(infectionData.incidenceHistory.Select(c => new Cases(c.count, new System.DateTime(c.year, c.month, c.day))).ToList());

        _isStopped = infectionData.isStopped;
        _isInfected = infectionData.isInfected;
    }
    #endregion
}
