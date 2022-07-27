using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using WPMF;
using NEAT;

using Debug = UnityEngine.Debug;

public class CurevidAgent : Agent
{
    [Header("Country Agent")]
    private Country currentCountry;

    [Header("Debug")]
    public bool debugInputLogging;
    public bool debugOutputLogging;
    public bool stopTime;
    private bool timeStarted;
    private Stopwatch st = new Stopwatch();

    private void Start() {

        TimeManager.Instance.dailyUpdateDelegate += ThinkForEveryCountry;
        ProgressCanvas.Instance.OnGameOver += Next;

        #region Statistics
        if (AIDataManager.Instance.saveBestScorePerGeneration)
            NEAT.NEAT.Instance.OnNextGeneration += () => {
                //if (NEAT.NEAT.Instance.GenerationCounter >= AIDataManager.Instance.maxGenerationCount)
                //    AIDataManager.Instance.saveBestScorePerGeneration = false;
                AIDataManager.Instance.SaveGraphPoint(NEAT.NEAT.Instance.GenerationCounter, NEAT.NEAT.Instance.BestGenomeLastGen.Fitness);
            };

        if (AIDataManager.Instance.onWinLoop)
            ProgressCanvas.Instance.OnGameWon += () => AIDataManager.Instance.SaveRunStatistic(NEAT.NEAT.Instance.GenerationCounter);
        if (AIDataManager.Instance.saveGameStatistic) {
            ProgressCanvas.Instance.OnGameOver += () => AIDataManager.Instance.SaveGameStatistic();
            ProgressCanvas.Instance.OnGameWon += () => AIDataManager.Instance.SaveGameStatistic();
        }
        else
            ProgressCanvas.Instance.OnGameWon += SaveSolution;
        #endregion

        #region Debug
        int familyCount = 0;
        HashSet<string> families = new HashSet<string>();
        foreach (MeasurementSO mso in MeasurementManager.Instance.generalMeasurements) {
            if (!families.Contains(mso.FamilyName)) {
                families.Add(mso.FamilyName);
                familyCount++;
            }
        }

        if (ConfigNEAT.OUTPUT_NODE_COUNT != familyCount)
            Debug.LogError("Measurement Family Count: <b>(" + familyCount + ")</b> does not match Output Node Count: <b>(" + ConfigNEAT.OUTPUT_NODE_COUNT + ")</b>");

        if (GameManager.Instance.currentPhase >= Phase.Phase_3) {
            Debug.Log("<b>Phase 3</b>");
        }
        #endregion
    }

    public void SaveSolution() {
        if (AIDataManager.Instance.saveBestScorePerGeneration)
            AIDataManager.Instance.SaveGraphPoint(NEAT.NEAT.Instance.GenerationCounter + 1, GetFitness());
        NEAT.NEAT.Instance.SaveModel(UIManagerNEAT.Instance.directoryName);
    }

    #region Agent Methods
    public override void ExecuteAction() {
        List<(string, string, float)> countryMeasurements = GetCountryMeasurementsFam();
        HashSet<string> measurementFamilies = new HashSet<string>();

        //int measurementOutputCount;
        //if (GameManager.Instance.currentPhase >= Phase.Phase_3)
        //    measurementOutputCount = outputs.Count - 1;
        //else
        //    measurementOutputCount = outputs.Count;

        for (int i = 0; i < output.Count; i++) {
            string familyName = countryMeasurements[i].Item1;

            if (!measurementFamilies.Contains(familyName)) {
                currentCountry.ToggleMeasurement(countryMeasurements[i].Item2, output[i]);
                measurementFamilies.Add(familyName);
            }
        }

        if (debugOutputLogging)
            DebugOutputLog(countryMeasurements);
    }

    public override List<float> UpdateInput() {
        List<(string, float)> countryInputs = new List<(string, float)>();

        countryInputs.AddRange(GetCountryStatistics());
        countryInputs.AddRange(GetCountryMeasurements());
        countryInputs.AddRange(GetGameStates());

        if (debugInputLogging) {
            DebugInputLog(countryInputs);
        }

        return countryInputs.Select(i => i.Item2).ToList();
    }

    public override float CalculateFitness() {
        float value;

        double globalFatalities = GlobalManager.Instance.GetGlobalValue(Value.Fatality);
        globalFatalities = globalFatalities > 0 ? globalFatalities : 1;
        int days = (TimeManager.Instance.GetCurrentDate() - TimeManager.Instance.GetStartDate()).Days;

        if (GameManager.Instance.currentPhase >= Phase.Phase_3)
            value = (float)((1f / (Mathf.Pow(10, -7) * globalFatalities)) * days * (1+ResearchManager.Instance.totalResearched));
        else
            value = (float)((1f / (Mathf.Pow(10, -7) * globalFatalities)) * days);

        return value;
    }

    public override void OnResetAgent() {
        #region Debug
        if (stopTime && timeStarted) {
            st.Stop();
            Debug.LogFormat("Run took {0} ms to complete", st.ElapsedMilliseconds);
        }
        #endregion

        GameManager.Instance.LoadNewGame();

        #region Debug
        if (debugInputLogging || debugOutputLogging)
            Debug.LogFormat("<color=magenta><b>------------------ Genome ID {0} ------------------</b></color>\n", this.brainNEAT.Id);
        #endregion
    }
    #endregion

    public void ThinkForEveryCountry() {
        if (!GlobalManager.Instance.allCountriesInitialized)
            return;

        if (TimeManager.Instance.GetCurrentDate().CompareTo(TimeManager.Instance.GetStartDate()) == 0) {
            return;
        }

        #region Debug
        if (!timeStarted) {
            st.Start();
            timeStarted = true;
        }
        #endregion

        UpdateFitness();
        UIManagerNEAT.Instance.UpdateFitnessUI(GetFitness());

        foreach (KeyValuePair<string, Country> element in GlobalManager.Instance.Countries) {
            currentCountry = element.Value;
            Think();
        }

        #region Statistics
        if (AIDataManager.Instance.saveGameStatistic) {
            //double globalRValue = GlobalManager.Instance.Countries.Sum(e => e.Value.Infection.RValue) / GlobalManager.Instance.Countries.Count(e => e.Value.Infection.RValue > 0);
            AIDataManager.Instance.SaveGlobalRValue(TimeManager.Instance.GetCurrentDate(), 0);

            AIDataManager.Instance.SavePandemicDayStatistic(Value.Infection, TimeManager.Instance.GetCurrentDate(), GlobalManager.Instance.GetGlobalValue(Value.Infection));
            AIDataManager.Instance.SavePandemicDayStatistic(Value.Fatality, TimeManager.Instance.GetCurrentDate(), GlobalManager.Instance.GetGlobalValue(Value.Fatality));
            AIDataManager.Instance.SavePandemicDayStatistic(Value.Vaccination, TimeManager.Instance.GetCurrentDate(), GlobalManager.Instance.GetGlobalValue(Value.Vaccination));
            AIDataManager.Instance.SavePandemicDayStatistic(Value.Recovery, TimeManager.Instance.GetCurrentDate(), GlobalManager.Instance.GetGlobalValue(Value.Recovery));

            AIDataManager.Instance.SavePandemicDayStatistic(Value.Happiness, TimeManager.Instance.GetCurrentDate(), GlobalManager.Instance.GetGlobalValue(Value.Happiness));
            AIDataManager.Instance.SavePandemicDayStatistic(Value.Revolt, TimeManager.Instance.GetCurrentDate(), GlobalManager.Instance.GetGlobalValue(Value.Revolt));

            AIDataManager.Instance.SavePandemicDayStatistic(Value.Economy, TimeManager.Instance.GetCurrentDate(), ResourceManager.Instance.GlobalEconomy.TotalValue);
            AIDataManager.Instance.SaveEconomyChangeStatistic(TimeManager.Instance.GetCurrentDate(), Calculate.Stat(ResourceManager.Instance.GlobalEconomy));
            AIDataManager.Instance.SaveResearchStatistic(TimeManager.Instance.GetCurrentDate(), ResearchManager.Instance.totalResearched);

            AIDataManager.Instance.SaveMeasurementStatistic(TimeManager.Instance.GetCurrentDate());
        }
        #endregion
    }
    #region Inputs

    public List<(string, float)> GetCountryStatistics() {
        List<(string, float)> countryStatistics = new List<(string, float)>();

        countryStatistics.Add(("RValue", (float)currentCountry.Infection.RValue));
        countryStatistics.Add(("Infection", (float)(currentCountry.Infection.TotalValue / currentCountry.Population)));
        countryStatistics.Add(("Fatalities", (float)(currentCountry.Fatality.TotalValue / currentCountry.Population)));
        countryStatistics.Add(("Recoveries", (float)(currentCountry.Recovery.TotalValue / currentCountry.Population)));

        if (GameManager.Instance.currentPhase >= Phase.Phase_2) {
            countryStatistics.Add(("Happiness", (float)(currentCountry.Happiness.TotalValue / 100)));
            countryStatistics.Add(("Happiness Rate", (float)(Calculate.Stat(currentCountry.Happiness) / 100)));

            countryStatistics.Add(("Revolt", (float)(currentCountry.Revolt.TotalValue / 100)));
            countryStatistics.Add(("Revolt Rate", (float)(Calculate.Stat(currentCountry.Revolt) / 100)));
        }
        return countryStatistics;
    }

    public List<(string, float)> GetCountryMeasurements() {
        List<(string, float)> countryMeasurements = new List<(string, float)>();
        HashSet<string> measurementFamilies = new HashSet<string>();

        for (int i = 0; i < currentCountry.Measurements.Count; i++) {
            Measurement measurement = currentCountry.Measurements[i];
            string familyName = measurement.FamilyName;

            if (!measurementFamilies.Contains(familyName)) {
                countryMeasurements.Add((familyName, currentCountry.GetMeasurementFamilyState(measurement.Name)));
                measurementFamilies.Add(familyName);
            }
        }

        return countryMeasurements;
    }

    public List<(string, string, float)> GetCountryMeasurementsFam() {
        List<(string, string, float)> countryMeasurements = new List<(string, string, float)>();
        HashSet<string> measurementFamilies = new HashSet<string>();

        for (int i = 0; i < currentCountry.Measurements.Count; i++) {
            Measurement measurement = currentCountry.Measurements[i];
            string familyName = measurement.FamilyName;

            if (!measurementFamilies.Contains(familyName)) {
                countryMeasurements.Add((measurement.FamilyName, measurement.Name, currentCountry.GetMeasurementFamilyState(measurement.Name)));
                measurementFamilies.Add(familyName);
            }
        }

        return countryMeasurements;
    }

    public List<(string, float)> GetGameStates() {
        List<(string, float)> gameStates = new List<(string, float)>();

        gameStates.Add(("GO Progress", ProgressCanvas.Instance.GetGameOverProgress()));

        return gameStates;
    }
    #endregion

    #region Outputs
    //public void Decide() {
    //    List<(string, string, float)> countryMeasurements = GetCountryMeasurementsFam();
    //    HashSet<string> measurementFamilies = new HashSet<string>();

    //    //int measurementOutputCount;
    //    //if (GameManager.Instance.currentPhase >= Phase.Phase_3)
    //    //    measurementOutputCount = outputs.Count - 1;
    //    //else
    //    //    measurementOutputCount = outputs.Count;

    //    for (int i = 0; i < output.Count; i++) {
    //        string familyName = countryMeasurements[i].Item1;

    //        if (!measurementFamilies.Contains(familyName)) {
    //            currentCountry.ToggleMeasurement(countryMeasurements[i].Item2, output[i]);
    //            measurementFamilies.Add(familyName);
    //        }
    //    }

    //    if (debugOutputLogging)
    //        DebugOutputLog(countryMeasurements);
    //}
    #endregion

    #region Debug
    int colorShuffle = 0;
    public void DebugInputLog(List<(string, float)> inputs) {
        colorShuffle++;
        string formattedString = "";
        string date = string.Format("{0:00}.{1:00}.{2}", TimeManager.Instance.day, TimeManager.Instance.month, TimeManager.Instance.year);

        foreach ((string, float) i in inputs) {
            formattedString += string.Format("<b>{0,-30}\t\t{1}</b>\n", i.Item1, i.Item2);
            formattedString += "--------------------------------------------------\n";
        }

        Debug.LogFormat("Logging : <color=magenta>{0}</color> <color={1}><b>{2} Input Values</b></color>\n\n{3}", currentCountry.name, colorShuffle % 2 == 1 ? "red" : "orange", date, formattedString);
    }

    public void DebugOutputLog(List<(string, string, float)> measurements) {
        string formattedString = "";
        string date = string.Format("{0:00}.{1:00}.{2}", TimeManager.Instance.day, TimeManager.Instance.month, TimeManager.Instance.year);

        for (int i = 0; i < this.output.Count; i++) {
            formattedString += string.Format("<b>{0,-30}\t\t{1}</b>\n", measurements[i].Item1, this.output[i]);
            formattedString += "--------------------------------------------------\n";
        }

        Debug.LogFormat("Logging : <color=magenta>{0}</color> <color={1}><b>{2} Output Values</b></color>\n\n{3}", currentCountry.name, colorShuffle % 2 == 1 ? "red" : "orange", date, formattedString);
    }
    #endregion
} 