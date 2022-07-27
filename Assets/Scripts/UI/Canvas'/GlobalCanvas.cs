using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WPMF;

public class GlobalCanvas : DynamicCanvas
{
    private static GlobalCanvas _instance;
    public static GlobalCanvas Instance { get => _instance; }

    [Header("Concrete Canvas")]
    public GameObject globalPanel;

    [Header("Global Statistics")]
    [SerializeField] private StatisticCell globalPopulation;
    [SerializeField] private StatisticCell globalInfected;
    [SerializeField] private StatisticCell dailyInfected;
    [SerializeField] private StatisticCell globalFatalities;
    [SerializeField] private StatisticCell dailyFatalities;
    [SerializeField] private StatisticCell globalVaccinated;
    [SerializeField] private StatisticCell dailyVaccinated;
    [SerializeField] private StatisticCell globalRecovered;

    [Header("Global List")]
    [SerializeField] private GameObject listItemPrefab;
    public GameObject globalList;
    public GameObject content;
    private Value currentValue;

    public override void Awake() {
        base.Awake();
        _instance = this;

        foreach (Transform child in content.transform)
            Destroy(child.gameObject);

        //GlobalManager.Instance.allCountriesInitializedEvent += InitGlobalListUI;
        //GlobalManager.Instance.allCountriesInitializedEvent += UpdateStatisticsUI;
        //TimeManager.Instance.dailyUpdateDelegate += UpdateUI;
    }

    private void Start() {
        TimeManager.Instance.dailyUpdateDelegate += UpdateUI;

        HidePanel();
    }

    bool init = false;
    private void Update() {
        if (!init) {
            InitGlobalListUI();
            UpdateStatisticsUI();
            //TimeManager.Instance.dailyUpdateDelegate += UpdateUI;
            init = true;
        }
    }

    public override void HidePanel() {
        base.HidePanel();
        globalPanel.SetActive(false);
    }

    public override void ShowPanel() {
        base.ShowPanel();
        globalPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        globalPanel.SetActive(true);
    }

    public void UpdateUI() {
        UpdateStatisticsUI();
        UpdateGlobalListUI(currentValue);
    }

    private void UpdateStatisticsUI() {
        globalPopulation.num.text = Format.LargeNumber(GlobalManager.Instance.GetGlobalValue(Value.Population));

        globalInfected.num.text = Format.LargeNumber(GlobalManager.Instance.GetGlobalValue(Value.Infection));
        globalInfected.Colorize(Colors.HEX_RED);

        double sum = 0;
        foreach (KeyValuePair<string, Country> element in GlobalManager.Instance.Countries)
            sum += element.Value.Infection.NewCases;
        dailyInfected.num.text = Format.LargeNumber(sum);
        dailyInfected.Colorize(Colors.HEX_RED);

        globalFatalities.num.text = Format.LargeNumber(GlobalManager.Instance.GetGlobalValue(Value.Fatality));
        globalFatalities.Colorize(Colors.HEX_YELLOW);

        sum = 0;
        foreach (KeyValuePair<string, Country> element in GlobalManager.Instance.Countries)
            sum += element.Value.Fatality.NewFatalities;
        dailyFatalities.num.text = Format.LargeNumber(sum);
        dailyFatalities.Colorize(Colors.HEX_YELLOW);

        globalVaccinated.num.text = Format.LargeNumber(GlobalManager.Instance.GetGlobalValue(Value.Vaccination));
        globalVaccinated.Colorize(Colors.HEX_BLUE);

        sum = 0;
        foreach (KeyValuePair<string, Country> element in GlobalManager.Instance.Countries)
            sum += element.Value.Vaccination.NewVaccinated;
        dailyVaccinated.num.text = Format.LargeNumber(sum);
        dailyVaccinated.Colorize(Colors.HEX_BLUE);

        globalRecovered.num.text = Format.LargeNumber(GlobalManager.Instance.GetGlobalValue(Value.Recovery));
        globalRecovered.Colorize(Colors.HEX_GREEN);
    }

    #region Global List Methods
    public void UpdateGlobalListUI(Value type) {
        currentValue = type;
        UpdateGlobalListUIRecursive(type, content.transform);
    }

    private void UpdateGlobalListUIRecursive(Value type, Transform parent) {
        foreach(Transform child in parent) {
            ListItem li = child.gameObject.GetComponent<ListItem>();
            if (parent.Equals(content.transform)) {
                li.num.text = Colorize(GlobalManager.Instance.GetContinentValue(li.name.text, type), type);
                UpdateGlobalListUIRecursive(type, li.body.transform);
            } else if (li.body.transform.childCount != 0) {
                li.num.text = Colorize(GlobalManager.Instance.GetSubregionValue(li.name.text, type), type);
                UpdateGlobalListUIRecursive(type, li.body.transform);
            } else {
                li.num.text = Colorize(GlobalManager.Instance.GetCountryValue(li.name.text, type), type);
            }
        }
    }

    private void InitGlobalListUI() {
        foreach (Transform child in content.transform)
            Destroy(child.gameObject);

        InitGlobalListUIRecursive(GlobalManager.Instance.Continents, content.transform);
        UpdateGlobalListUI(Value.Infection);
    }

    private void InitGlobalListUIRecursive<T>(Dictionary<string, T> dic, Transform parent) {
        foreach (KeyValuePair<string, T> element in dic) {
            GameObject go = Instantiate(listItemPrefab);
            ListItem li = go.GetComponent<ListItem>();
            go.name = element.Key;
            li.name.text = element.Key;
            go.transform.SetParent(parent, false);
            if (typeof(T).Equals(typeof(Continent))) {
                Continent con = (Continent)(object)element.Value;
                li.count.text = "(" + con.Subregions.Count + ")";
                InitGlobalListUIRecursive(con.Subregions, li.body.transform);
            } else if (typeof(T).Equals(typeof(Subregion))) {
                Subregion sub = (Subregion)(object)element.Value;
                li.count.text = "(" + sub.Countries.Count + ")";
                li.ChangeColorBy(-0.065f);
                InitGlobalListUIRecursive(sub.Countries, li.body.transform);
            } else {
                li.count.text = "";
                li.ChangeColorBy(-0.1f);
            }
        }
    }
    #endregion

    private string Colorize(double value, Value type) {
        if(type.Equals(Value.Infection) || type.Equals(Value.Fatality) ||
            type.Equals(Value.Vaccination) || type.Equals(Value.Recovery)){
            string formatted = Format.LargeNumber(value);
            if (type.Equals(Value.Infection))
                return Format.ColorString(formatted, Colors.HEX_RED);
            if (type.Equals(Value.Fatality))
                return Format.ColorString(formatted, Colors.HEX_YELLOW);
            if (type.Equals(Value.Vaccination))
                return Format.ColorString(formatted, Colors.HEX_BLUE);
            if (type.Equals(Value.Recovery))
                return Format.ColorString(formatted, Colors.HEX_GREEN);
        } else if(type.Equals(Value.Economy)){
            return Format.StatColoredString(value);
        } else if(type.Equals(Value.Happiness)){
            if (value >= 75)
                return Format.ColorString(Format.Stat(value), Colors.HEX_GREEN);
            else if (value <= 25)
                return Format.ColorString(Format.Stat(value), Colors.HEX_RED);
            else
                return Format.ColorString(Format.Stat(value), Colors.HEX_YELLOW);
        } else if (type.Equals(Value.Revolt)) {
            return value > 0 ? Format.ColorString(Format.Stat(value), Colors.HEX_RED) : Format.ColorString(Format.Stat(value), Colors.HEX_WHITE);
        }
        return "NoColorMatched";
    }
}
