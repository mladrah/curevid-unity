using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPMF;

public class ResourceManager : MonoBehaviour, ISaveable
{
    public static ResourceManager _instance;
    public static ResourceManager Instance { get => _instance; }

    private Stat _globalEconomy;
    public Stat GlobalEconomy { get => _globalEconomy; }

    public delegate void resourceChange();
    public resourceChange globalEconomyChangeDelegate;

    private void Awake() {
        _instance = this;
        _globalEconomy = new Stat();
    }

    private void Start() {
        TimeManager.Instance.dailyUpdateDelegate += DailyChange;
        Country.economyUpdateEvent += UpdateResources;

        GlobalManager.Instance.allCountriesInitializedEvent += Initialize;
    }

    private void Initialize() {
        float totalEconomy = (float) GlobalManager.Instance.GetGlobalValue(Value.Economy);
        _globalEconomy.Modifiers.Add(new Modifier("Countries", Modifier.effectType.ADDITIVE, Modifier.modifierType.ECONOMY, totalEconomy > 0 ? Modifier.valueType.POSITIVE : Modifier.valueType.NEGATIVE, totalEconomy));

        ResourceCanvas.Instance.UpdateResourcesUI();
    }

    public void DailyChange() {
        _globalEconomy.ChangeTotalValueBy(Calculate.Stat(_globalEconomy));
        ResourceCanvas.Instance.UpdateResourcesUI();

        if (globalEconomyChangeDelegate != null)
            globalEconomyChangeDelegate();
    }

    public void UpdateResources() {
        if (!GlobalManager.Instance.allCountriesInitialized)
            return;

        float totalEconomy = (float)GlobalManager.Instance.GetGlobalValue(Value.Economy);
        Modifier m = new Modifier("Countries", Modifier.effectType.ADDITIVE, Modifier.modifierType.ECONOMY, totalEconomy > 0 ? Modifier.valueType.POSITIVE : Modifier.valueType.NEGATIVE, totalEconomy);
        _globalEconomy.UpdateModifier("Countries", m);

        ResourceCanvas.Instance.UpdateResourcesUI();
    }

    #region Serialisation
    public void PopulateSaveData(SaveData saveData) {
        SaveData.ResourceManagerData rmd = new SaveData.ResourceManagerData(this);

        saveData.resourceManagerData = rmd;
    }

    public void LoadFromSaveData(SaveData saveData) {
        SaveData.ResourceManagerData rmd = saveData.resourceManagerData;

        _globalEconomy.LoadFromSaveData(rmd.globalEconomy);

        UpdateResources();
    }
    #endregion
}
