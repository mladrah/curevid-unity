using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine;
using NEAT;
using WPMF;

public class AIDataManager : MonoBehaviour
{
    public static AIDataManager Instance { get; private set; }

    [Header("Serialisation")]
    public string FILE_NAME_RUN_STATISTIC;
    //public string FILE_NAME_PANDEMIC_STATISTIC;
    //public string FILE_NAME_MEASUREMENT_STATISTIC;
    public string FILE_NAME_GAME_STATISTIC;
    public string FILE_NAME_GENERAL_STATISTIC;
    private string PATH;

    [Header("Training Data")]
    public bool saveFileLoop;
    public bool saveFileLoopReset;
    public int maxSaveFileCount;
    private int saveFileCount;

    [Header("Run Statistic")]
    public int runCap = 100;
    public int currentRun;
    public bool resetCurrentRun;
    public bool onWinLoop;

    [Header("Game Statistic")]
    public bool saveGameStatistic;
    private GameStatistic gameStatistic;
    private bool gameStatisticInitialized;
    [NonSerialized] public Dictionary<string, Dictionary<string, int>> measurementDict;

    [Header("Neural Net Statistic")]
    public bool saveBestScorePerGeneration;
    public int maxGenerationCount;

    private void Awake() {
        DontDestroyOnLoad(this.gameObject);

        Instance = this;

        if (onWinLoop) {
            if (resetCurrentRun)
                PlayerPrefs.SetInt("CurrentRun", 1);
            currentRun = PlayerPrefs.GetInt("CurrentRun");

            Debug.Log("<b>Start Loggin Run Statistic</b>");
        }

        if (saveFileLoopReset)
            PlayerPrefs.SetInt("SaveFileCount", 0);
        if (saveFileLoop)
            saveFileCount = PlayerPrefs.GetInt("SaveFileCount");
    }

    private void Start() {
        PATH = Application.dataPath + "/Resources/Neuroevolution/models/" + UIManagerNEAT.Instance.directoryName + "data/";

        if (saveBestScorePerGeneration)
            FILE_NAME_GENERAL_STATISTIC = "best_genome_score_per_gen_" + GameManager.Instance.currentModel.ToString() + "_" + GameManager.Instance.currentPhase.ToString();

        if (saveGameStatistic) {
            if (string.IsNullOrEmpty(FILE_NAME_GAME_STATISTIC))
                FILE_NAME_GAME_STATISTIC = "game_statistic_" + GameManager.Instance.currentPhase.ToString() + ".json";
            else
                FILE_NAME_GAME_STATISTIC = FILE_NAME_GAME_STATISTIC + ".json";

            measurementDict = new Dictionary<string, Dictionary<string, int>>();

            foreach (MeasurementSO mso in MeasurementManager.Instance.generalMeasurements) {
                if (!measurementDict.ContainsKey(mso.FamilyName))
                    measurementDict.Add(mso.FamilyName, new Dictionary<string, int>());
            }

            foreach (MeasurementSO mso in MeasurementManager.Instance.generalMeasurements) {
                if (!measurementDict[mso.FamilyName].ContainsKey(mso.Name))
                    measurementDict[mso.FamilyName].Add(mso.Name, 0);
            }

            Debug.Log("<b>Start Logging Pandemic Statistic</b>");
            Debug.Log("<b>Start Logging Measurement Statistic</b>");
        }

        GlobalManager.Instance.allCountriesInitializedEvent += () => {
            if (saveFileLoop) {
                if (saveFileCount < maxSaveFileCount) {
                    SerialisationCanvas.Instance.Save();
                    saveFileCount++;
                    PlayerPrefs.SetInt("SaveFileCount", saveFileCount);
                    Application.LoadLevel(Application.loadedLevel);
                }
            }
        };
    }

    public void SaveRunStatistic(int generation) {
        string filename;
        if (string.IsNullOrEmpty(FILE_NAME_RUN_STATISTIC))
            filename = "run_statistic.json";
        else
            filename = FILE_NAME_RUN_STATISTIC + ".json";

        RunStatistic runStatistic;
        try {
            runStatistic = GetRunStatistic(filename);
        } catch (FileNotFoundException e) {
            runStatistic = new RunStatistic();
            runStatistic.runDatas = new List<RunData>();
            Debug.Log("Initialized RunManager");
        }

        RunData runData = new RunData(currentRun, ++generation);
        runStatistic.runDatas.Add(runData);
        //Reading the Network model into a string.
        string jsonRep = JsonUtility.ToJson(runStatistic);


        if (!Directory.Exists(PATH)) {
            Directory.CreateDirectory(PATH);
        }

        //Saving the file.
        File.WriteAllText(PATH + filename, jsonRep);

        PlayerPrefs.SetInt("CurrentRun", ++currentRun);

        if (currentRun <= runCap)
            Application.LoadLevel(Application.loadedLevel);
    }

    public void SaveGlobalRValue(DateTime date, double value) {
        GameStatistic rValueStatistic;

        if (!gameStatisticInitialized)
            rValueStatistic = InitializeGameStatistic(FILE_NAME_GAME_STATISTIC);
        else
            rValueStatistic = gameStatistic;

        double globalRValue = GlobalManager.Instance.Countries.Sum(e => e.Value.Infection.RValue) / GlobalManager.Instance.Countries.Count;
        double rValueRecovered = GlobalManager.Instance.Countries.Sum(e => e.Value.Infection.RecoveryRInfluence) / GlobalManager.Instance.Countries.Count;
        double rValueVaccinated = GlobalManager.Instance.Countries.Sum(e => e.Value.Infection.VaccinatedRInfluence) / GlobalManager.Instance.Countries.Count;

        Tuple<string, double>[] rValueSociety = new Tuple<string, double>[7];
        rValueSociety[0] = new Tuple<string, double>("Socialising", 0);
        rValueSociety[1] = new Tuple<string, double>("Education", 0);
        rValueSociety[2] = new Tuple<string, double>("Work", 0);
        rValueSociety[3] = new Tuple<string, double>("Activities", 0);
        rValueSociety[4] = new Tuple<string, double>("Gatherings", 0);
        rValueSociety[5] = new Tuple<string, double>("Revolt", 0);
        rValueSociety[6] = new Tuple<string, double>("Neighboring Countries", 0);


        for (int i = 0; i < rValueSociety.Length; i++) {
            Stat stat;
            double sum = 0;
            foreach (KeyValuePair<string, Country> element in GlobalManager.Instance.Countries) {
                Country country = element.Value;
                if(element.Value.Infection.SocietyDictionary.TryGetValue(rValueSociety[i].Item1, out stat)) {
                    if (country.Infection.IsInfected)
                        sum += Calculate.Stat(stat);
                    else
                        sum += 0;
                } else {
                    Debug.LogError("Error: " + rValueSociety[i].Item1);
                }
            }
            double average = sum / GlobalManager.Instance.Countries.Count;
            rValueSociety[i] = new Tuple<string, double>(rValueSociety[i].Item1, average);
        }

        rValueStatistic.rHistoryGlobal.Add(new DayData(date, globalRValue));

        rValueStatistic.rHistoryRecovered.Add(new DayData(date, rValueRecovered));
        rValueStatistic.rHistoryVaccinated.Add(new DayData(date, rValueVaccinated));

        rValueStatistic.rHistorySocial.Add(new DayData(date, rValueSociety[0].Item2));
        rValueStatistic.rHistoryEducation.Add(new DayData(date, rValueSociety[1].Item2));
        rValueStatistic.rHistoryWork.Add(new DayData(date, rValueSociety[2].Item2));
        rValueStatistic.rHistoryActivities.Add(new DayData(date, rValueSociety[3].Item2));
        rValueStatistic.rHistoryGatherings.Add(new DayData(date, rValueSociety[4].Item2));
        rValueStatistic.rHistoryRevolt.Add(new DayData(date, rValueSociety[5].Item2));
        rValueStatistic.rHistoryNeighboring.Add(new DayData(date, rValueSociety[6].Item2));

        gameStatistic.rHistoryGlobal = rValueStatistic.rHistoryGlobal;

        gameStatistic.rHistorySocial = rValueStatistic.rHistorySocial;
        gameStatistic.rHistoryEducation = rValueStatistic.rHistoryEducation;
        gameStatistic.rHistoryWork = rValueStatistic.rHistoryWork;
        gameStatistic.rHistoryActivities = rValueStatistic.rHistoryActivities;
        gameStatistic.rHistoryGatherings = rValueStatistic.rHistoryGatherings;
        gameStatistic.rHistoryRevolt = rValueStatistic.rHistoryRevolt;
        gameStatistic.rHistoryNeighboring = rValueStatistic.rHistoryNeighboring;

        gameStatistic.rHistoryRecovered = rValueStatistic.rHistoryRecovered;
        gameStatistic.rHistoryVaccinated = rValueStatistic.rHistoryVaccinated;
    }

    public void SavePandemicDayStatistic(Value valueType, DateTime date, double value) {
        GameStatistic pandemicStatistic;

        if (!gameStatisticInitialized)
            pandemicStatistic = InitializeGameStatistic(FILE_NAME_GAME_STATISTIC);
        else
            pandemicStatistic = gameStatistic;

        switch (valueType) {
            case Value.Infection:
                pandemicStatistic.infectionHistory.Add(new DayData(date, value));
                break;
            case Value.Fatality:
                pandemicStatistic.fatalityHistory.Add(new DayData(date, value));
                break;
            case Value.Vaccination:
                pandemicStatistic.vaccinationHistory.Add(new DayData(date, value));
                break;
            case Value.Recovery:
                pandemicStatistic.recoveryHistory.Add(new DayData(date, value));
                break;
            case Value.Happiness:
                pandemicStatistic.globalHappinessHistory.Add(new DayData(date, value));
                break;
            case Value.Revolt:
                pandemicStatistic.globalRevoltHistory.Add(new DayData(date, value));
                break;
            case Value.Economy:
                pandemicStatistic.economyHistory.Add(new DayData(date, value));
                break;
            default:
                Debug.LogError("Value does not match");
                break;
        }

        gameStatistic.infectionHistory = pandemicStatistic.infectionHistory;
        gameStatistic.fatalityHistory = pandemicStatistic.fatalityHistory;
        gameStatistic.vaccinationHistory = pandemicStatistic.vaccinationHistory;
        gameStatistic.recoveryHistory = pandemicStatistic.recoveryHistory;

        gameStatistic.globalHappinessHistory = pandemicStatistic.globalHappinessHistory;
        gameStatistic.globalRevoltHistory = pandemicStatistic.globalRevoltHistory;

        gameStatistic.economyHistory = pandemicStatistic.economyHistory;
    }

    public void SaveEconomyChangeStatistic(DateTime date, float value) {
        GameStatistic economyStatistic;

        if (!gameStatisticInitialized)
            economyStatistic = InitializeGameStatistic(FILE_NAME_GAME_STATISTIC);
        else
            economyStatistic = gameStatistic;

        economyStatistic.economyChangeHistory.Add(new DayData(date, value));

        gameStatistic.economyChangeHistory = economyStatistic.economyChangeHistory;
    }

    public void SaveResearchStatistic(DateTime date, int researchCount) {
        GameStatistic researchStatistic;

        if (!gameStatisticInitialized)
            researchStatistic = InitializeGameStatistic(FILE_NAME_GAME_STATISTIC);
        else
            researchStatistic = gameStatistic;

        researchStatistic.researchHistory.Add(new DayData(date, researchCount));

        gameStatistic.researchHistory = researchStatistic.researchHistory;
    }

    public void SaveMeasurementStatistic(DateTime date) {
        GameStatistic measurementStatistic;

        if (!gameStatisticInitialized)
            measurementStatistic = InitializeGameStatistic(FILE_NAME_GAME_STATISTIC);
        else
            measurementStatistic = gameStatistic;

        foreach (KeyValuePair<string, Dictionary<string, int>> element in measurementDict) {

            switch (element.Key) {
                case "Mask Mandate":
                    measurementStatistic.maskMandateHistory.Add(new MeasurementDayData(date));
                    foreach (KeyValuePair<string, int> subElement in element.Value) {
                        measurementStatistic.maskMandateHistory.Last().measurementValues.Add(new MeasurementData(subElement.Key, subElement.Value));
                    }
                    break;
                case "Relief Fund":
                    measurementStatistic.reliefFundHistory.Add(new MeasurementDayData(date));
                    foreach (KeyValuePair<string, int> subElement in element.Value)
                        measurementStatistic.reliefFundHistory.Last().measurementValues.Add(new MeasurementData(subElement.Key, subElement.Value));
                    break;
                case "Socialising":
                    measurementStatistic.socialisingHistory.Add(new MeasurementDayData(date));
                    foreach (KeyValuePair<string, int> subElement in element.Value) {
                        measurementStatistic.socialisingHistory.Last().measurementValues.Add(new MeasurementData(subElement.Key, subElement.Value));
                    }
                    break;
                case "Education":
                    measurementStatistic.educationHistory.Add(new MeasurementDayData(date));
                    foreach (KeyValuePair<string, int> subElement in element.Value)
                        measurementStatistic.educationHistory.Last().measurementValues.Add(new MeasurementData(subElement.Key, subElement.Value));
                    break;
                case "Work":
                    measurementStatistic.workHistory.Add(new MeasurementDayData(date));
                    foreach (KeyValuePair<string, int> subElement in element.Value)
                        measurementStatistic.workHistory.Last().measurementValues.Add(new MeasurementData(subElement.Key, subElement.Value));
                    break;
                case "Activities":
                    measurementStatistic.activitiesHistory.Add(new MeasurementDayData(date));
                    foreach (KeyValuePair<string, int> subElement in element.Value)
                        measurementStatistic.activitiesHistory.Last().measurementValues.Add(new MeasurementData(subElement.Key, subElement.Value));
                    break;
                case "Gatherings":
                    measurementStatistic.gatheringsHistory.Add(new MeasurementDayData(date));
                    foreach (KeyValuePair<string, int> subElement in element.Value)
                        measurementStatistic.gatheringsHistory.Last().measurementValues.Add(new MeasurementData(subElement.Key, subElement.Value));
                    break;
                case "Revolt":
                    measurementStatistic.revoltHistory.Add(new MeasurementDayData(date));
                    foreach (KeyValuePair<string, int> subElement in element.Value)
                        measurementStatistic.revoltHistory.Last().measurementValues.Add(new MeasurementData(subElement.Key, subElement.Value));
                    break;
                case "Neighboring Countries":
                    measurementStatistic.neighboringCountriesHistory.Add(new MeasurementDayData(date));
                    foreach (KeyValuePair<string, int> subElement in element.Value)
                        measurementStatistic.neighboringCountriesHistory.Last().measurementValues.Add(new MeasurementData(subElement.Key, subElement.Value));
                    break;
                default:
                    Debug.LogError("Measurement Family missing: " + element.Key);
                    break;
            }
        }

        gameStatistic.maskMandateHistory = measurementStatistic.maskMandateHistory;
        gameStatistic.reliefFundHistory = measurementStatistic.reliefFundHistory;
        gameStatistic.socialisingHistory = measurementStatistic.socialisingHistory;
        gameStatistic.educationHistory = measurementStatistic.educationHistory;
        gameStatistic.workHistory = measurementStatistic.workHistory;
        gameStatistic.activitiesHistory = measurementStatistic.activitiesHistory;
        gameStatistic.gatheringsHistory = measurementStatistic.gatheringsHistory;
        gameStatistic.revoltHistory = measurementStatistic.revoltHistory;
        gameStatistic.neighboringCountriesHistory = measurementStatistic.neighboringCountriesHistory;

        measurementDict.Keys.ToList().ForEach(x => measurementDict[x].Keys.ToList().ForEach(x2 => measurementDict[x][x2] = 0));
    }

    public void SaveGameStatistic() {
        ////Reading the Network model into a string.
        string jsonRep = JsonUtility.ToJson(gameStatistic);

        if (!Directory.Exists(PATH)) {
            Directory.CreateDirectory(PATH);
        }

        //Saving the file.
        File.WriteAllText(PATH + FILE_NAME_GAME_STATISTIC, jsonRep);
    }

    public void SaveGraphPoint(double x, double y) {
        string filename;
        if (string.IsNullOrEmpty(FILE_NAME_GENERAL_STATISTIC))
            filename = "general_graph.json";
        else
            filename = FILE_NAME_GENERAL_STATISTIC + ".json";

        Graph generalStatistic;
        try {
            generalStatistic = GetGeneralStatistic(filename);
        } catch (FileNotFoundException e) {
            generalStatistic = new Graph();
            generalStatistic.points = new List<Point>();

            Debug.Log("Initialized GeneralStatistic");
        }

        generalStatistic.points.Add(new Point(x, y));

        //Reading the Network model into a string.
        string jsonRep = JsonUtility.ToJson(generalStatistic);

        if (!Directory.Exists(PATH)) {
            Directory.CreateDirectory(PATH);
        }

        //Saving the file.
        File.WriteAllText(PATH + filename, jsonRep);
    }

    #region Getter
    public GameStatistic InitializeGameStatistic(string filename) {
        gameStatistic = new GameStatistic();

        gameStatistic.rHistoryGlobal = new List<DayData>();
        gameStatistic.rHistorySocial = new List<DayData>();
        gameStatistic.rHistoryEducation = new List<DayData>();
        gameStatistic.rHistoryWork = new List<DayData>();
        gameStatistic.rHistoryActivities = new List<DayData>();
        gameStatistic.rHistoryGatherings = new List<DayData>();
        gameStatistic.rHistoryRevolt = new List<DayData>();
        gameStatistic.rHistoryNeighboring = new List<DayData>();
        gameStatistic.rHistoryRecovered = new List<DayData>();
        gameStatistic.rHistoryVaccinated = new List<DayData>();

        gameStatistic.infectionHistory = new List<DayData>();
        gameStatistic.fatalityHistory = new List<DayData>();
        gameStatistic.vaccinationHistory = new List<DayData>();
        gameStatistic.recoveryHistory = new List<DayData>();

        gameStatistic.globalHappinessHistory = new List<DayData>();
        gameStatistic.globalRevoltHistory = new List<DayData>();

        gameStatistic.economyHistory = new List<DayData>();
        gameStatistic.economyChangeHistory = new List<DayData>();
        gameStatistic.researchHistory = new List<DayData>();

        gameStatistic.maskMandateHistory = new List<MeasurementDayData>();
        gameStatistic.reliefFundHistory = new List<MeasurementDayData>();
        gameStatistic.socialisingHistory = new List<MeasurementDayData>();
        gameStatistic.educationHistory = new List<MeasurementDayData>();
        gameStatistic.workHistory = new List<MeasurementDayData>();
        gameStatistic.activitiesHistory = new List<MeasurementDayData>();
        gameStatistic.gatheringsHistory = new List<MeasurementDayData>();
        gameStatistic.revoltHistory = new List<MeasurementDayData>();
        gameStatistic.neighboringCountriesHistory = new List<MeasurementDayData>();


        gameStatisticInitialized = true;

        return gameStatistic;
    }

    public RunStatistic GetRunStatistic(string filename) {
        if (!Directory.Exists(PATH)) {
            Directory.CreateDirectory(PATH);
        }

        //Reading the json file from the input file path.
        string jsonRep = File.ReadAllText(PATH + filename);
        RunStatistic runStatistic = JsonUtility.FromJson<RunStatistic>(jsonRep);

        return runStatistic;
    }

    public Graph GetGeneralStatistic(string filename) {
        if (!Directory.Exists(PATH)) {
            Directory.CreateDirectory(PATH);
        }

        //Reading the json file from the input file path.
        string jsonRep = File.ReadAllText(PATH + filename);
        Graph generalStatistic = JsonUtility.FromJson<Graph>(jsonRep);

        return generalStatistic;
    }
    #endregion
}

#region Structs
[System.Serializable]
public struct RunStatistic
{
    public List<RunData> runDatas;
}

[System.Serializable]
public struct RunData
{
    public int runId;
    public int generation;

    public RunData(int runId, int generation) {
        this.runId = runId;
        this.generation = generation;
    }
}

public struct GameStatistic
{
    public List<DayData> rHistoryGlobal;
    public List<DayData> rHistorySocial;
    public List<DayData> rHistoryEducation;
    public List<DayData> rHistoryWork;
    public List<DayData> rHistoryActivities;
    public List<DayData> rHistoryGatherings;
    public List<DayData> rHistoryRevolt;
    public List<DayData> rHistoryNeighboring;
    public List<DayData> rHistoryRecovered;
    public List<DayData> rHistoryVaccinated;

    public List<DayData> infectionHistory;
    public List<DayData> fatalityHistory;
    public List<DayData> vaccinationHistory;
    public List<DayData> recoveryHistory;

    public List<DayData> globalHappinessHistory;
    public List<DayData> globalRevoltHistory;

    public List<DayData> economyHistory;
    public List<DayData> economyChangeHistory;
    public List<DayData> researchHistory;

    public List<MeasurementDayData> maskMandateHistory;
    public List<MeasurementDayData> reliefFundHistory;
    public List<MeasurementDayData> socialisingHistory;
    public List<MeasurementDayData> educationHistory;
    public List<MeasurementDayData> workHistory;
    public List<MeasurementDayData> activitiesHistory;
    public List<MeasurementDayData> gatheringsHistory;
    public List<MeasurementDayData> revoltHistory;
    public List<MeasurementDayData> neighboringCountriesHistory;

}

[System.Serializable]
public struct MeasurementDayData
{
    public DateData date;
    public List<MeasurementData> measurementValues;

    public MeasurementDayData(DateTime date) {
        this.date = new DateData(date);
        this.measurementValues = new List<MeasurementData>();
    }
}

[System.Serializable]
public struct MeasurementData
{
    public string name;
    public int value;

    public MeasurementData(string name, int value) {
        this.name = name;
        this.value = value;
    }
}

[System.Serializable]
public struct Graph
{
    public List<Point> points;
}

[System.Serializable]
public struct Point
{
    public double x;
    public double y;

    public Point(double x, double y) {
        this.x = x;
        this.y = y;
    }
}

[System.Serializable]
public struct DayData
{
    public DateData date;
    public double value;

    public DayData(DateTime date, double value) {
        this.date = new DateData(date);
        this.value = value;
    }
}

[System.Serializable]
public struct DateData
{
    public int day;
    public int month;
    public int year;

    public DateData(DateTime dateTime) {
        this.day = dateTime.Day;
        this.month = dateTime.Month;
        this.year = dateTime.Year;
    }
}
#endregion
