using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResearchManager : MonoBehaviour, ISaveable
{
    public static ResearchManager _instance;
    public static ResearchManager Instance { get => _instance; }

    private const string RESEARCHING_ECONOMY_MODIFIER = "Research";

    [Header("Research List")]
    [SerializeField]
    private List<Research> _researches;
    public List<Research> Researches { get => _researches; }

    [Header("Current Research")]
    private Research _research;
    public Research Research { get => _research; }

    private int _daysPassed;
    public int DaysPassed { get => _daysPassed; }

    [Header("AI")]
    private int researchIndex;
    public int totalResearched;

    private void Awake() {
        _instance = this;
    }

    private void Start() {
        TimeManager.Instance.dailyUpdateDelegate += UpdateResearchProgress;

        if (SceneManager.GetActiveScene().buildIndex.Equals((int)Scenes.Train)) {
            if (GameManager.Instance.currentPhase >= Phase.Phase_3) {
                for (int i = 0; i < _researches.Count; i++) {
                    if (_researches[i].Name != AIResearchManager.Instance.ResearchSOList[i].Name) {
                        Debug.LogErrorFormat("Research <b>{0}</b> does not match with ResearchSO Item <b>{1}</b>", _researches[i].Name, AIResearchManager.Instance.ResearchSOList[i].Name);
                    } else {
                        _researches[i].SetResearchSO(AIResearchManager.Instance.ResearchSOList[i]);
                    }
                }
            }
        }
    }

    private void FixedUpdate() {
        if (GameManager.Instance != null) {
            if (GameManager.Instance.currentPhase >= Phase.Phase_3) {
                if (researchIndex < _researches.Count) {
                    if (!_researches[researchIndex].IsResearched && !_researches[researchIndex].IsResearching) {
                        if (ResourceManager.Instance.GlobalEconomy.TotalValue >= _researches[researchIndex].Cost) {
                            //Debug.Log("Researching -> " + _researches[researchIndex].name);
                            ResearchCanvas.Instance.SetFocusedResearch(_researches[researchIndex]);
                            ResearchCanvas.Instance.StartResearch();
                        }
                    }
                }
            }
        }
    }

    public void StartResearch(Research research) {
        if (research == null) {
            Debug.LogError("Can not start Research: Object is null");
            return;
        }

        _research = research;
        _research.IsResearching = true;

        float dailyCost = TechnicalManager.Instance.debugResearch ? 0.5f : research.DailyCost;
        float researchCost = TechnicalManager.Instance.debugResearch ? 1f : research.Cost;

        ResourceManager.Instance.GlobalEconomy.AddModifier(new Modifier(RESEARCHING_ECONOMY_MODIFIER, Modifier.effectType.ADDITIVE, Modifier.modifierType.ECONOMY, Modifier.valueType.NEGATIVE, (-1) * dailyCost));
        ResourceManager.Instance.GlobalEconomy.ChangeTotalValueBy((-1) * researchCost);

        ResourceCanvas.Instance.UpdateResourcesUI();
    }

    public Research CancelResearch() {
        if (_research == null) {
            Debug.LogError("Can not cancel Research: It is already null");
            return null;
        }

        _research.IsResearching = false;
        Research researchTemp = _research;
        _research = null;
        _daysPassed = 0;

        ResourceManager.Instance.GlobalEconomy.RemoveModifier(RESEARCHING_ECONOMY_MODIFIER);
        ResourceCanvas.Instance.UpdateResourcesUI();

        return researchTemp;
    }

    private void UpdateResearchProgress() {
        if (_research == null)
            return;

        _daysPassed++;

        float duration = TechnicalManager.Instance.debugResearch ? 1 : _research.Duration;

        float progress = (float)_daysPassed / duration;

        if (progress == 1)
            ResearchOnComplete();

        ResearchCanvas.Instance.UpdateResearchProgressUI(progress);
        ResourceCanvas.Instance.UpdateResearchUI(progress);
    }

    private void ResearchOnComplete() {
        _research.IsResearching = false;
        _research.IsResearched = true;

        Cure.Instance.UpdateStats(_research);

        ResourceManager.Instance.GlobalEconomy.RemoveModifier(RESEARCHING_ECONOMY_MODIFIER);
        ResourceCanvas.Instance.UpdateResourcesUI();


        LogManager.Log("Research" + " <b>" + _research.name + "</b> " + "completed!", "", Colors.HEX_BLUE);

        if (SceneManager.GetActiveScene().buildIndex.Equals((int)Scenes.Train)) {
            totalResearched++;
            researchIndex++;
            //Debug.Log(_research.name + " completed | Total -> " + totalResearched);
        }

        Research researchTemp = CancelResearch();

        ResearchCanvas.Instance.ResearchOnComplete(researchTemp);
    }

    #region Serialisation
    public void PopulateSaveData(SaveData saveData) {
        SaveData.ResearchManagerData rmd = new SaveData.ResearchManagerData(this);

        saveData.researchManagerData = rmd;
    }

    public void LoadFromSaveData(SaveData saveData) {
        SaveData.ResearchManagerData rmd = saveData.researchManagerData;

        _daysPassed = rmd.daysPassed;

        foreach (SaveData.ResearchData rd in rmd.researchDatas) {
            foreach (Research r in _researches) {
                if (r.Name.Equals(rd.name)) {
                    r.LoadFromSaveData(rd);
                    if (r.IsResearching)
                        _research = r;
                }
            }
        }

        ///<summary>
        ///In UpdateResearchProgress() daysPassed will be incremented
        /// </summary>
        if (_daysPassed > 0) {
            _daysPassed--;
            UpdateResearchProgress();
        }

        #region AI
        researchIndex = 0;
        totalResearched = 0;
        #endregion
    }
    #endregion
}
