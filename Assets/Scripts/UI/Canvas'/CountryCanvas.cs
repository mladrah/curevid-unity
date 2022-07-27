using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WPMF;
using DG.Tweening;
public class CountryCanvas : DynamicCanvas
{
    public static CountryCanvas instance;
    private WorldMap2D map;
    private Country country;

    [SerializeField] private CountryStatistic countryStatistic;
    [SerializeField] private CountryMeasurement countryMeasurement;
    [SerializeField] private CountryEconomy countryEconomy;
    [SerializeField] private CountryHappiness countryHappiness;

    #region Overview UI Fields
    [Header("Overview")]
    [SerializeField] private GameObject countryPanel;
    private bool isCountryPanelVisible;
    [SerializeField] private TextMeshProUGUI countryNameText;
    [SerializeField] private TextMeshProUGUI countryContinentNameText;
    [SerializeField] private TextMeshProUGUI countrySubregionNameText;
    [System.NonSerialized] public bool isNewCountry;
    #endregion

    #region Trait UI Fields
    [Header("Traits")]
    [SerializeField] private GameObject traitList;
    [SerializeField] private GameObject traitPrefab;
    #endregion

    public override void Awake() {
        base.Awake();
    }

    private void Start() {
        instance = this;
        map = WorldMap2D.instance;
        CountryPanelSetup();
    }

    private void CountryPanelSetup() {
        map.OnCountryClick += ShowCountryPanel;

        countryPanel.SetActive(false);
    }

    public void ShowCountryPanel(int countryIndex, int regionIndex) {
        base.ShowPanel();

        isNewCountry = true;

        VisualManager.Instance.HighlightCountryOutline(countryIndex);

        isCountryPanelVisible = true;
        country = map.countries[countryIndex];
        countryPanel.SetActive(true);

        country.traitUpdateEvent += UpdateTraits;
        country.measurementUpdateEvent += UpdateMeasurement;
        Country.economyUpdateEvent += UpdateEconomy;
        country.happinessUpdateEvent += UpdateHappiness;
        country.generalUpdateEvent += UpdateStatistic;

        UpdateOverview();
        UpdateTraits();
        UpdateStatistic();
        UpdateMeasurement();
        UpdateEconomy();
        UpdateHappiness();

        isNewCountry = false;
    }

    public override void HidePanel() {
        base.HidePanel();

        country = null;

        isCountryPanelVisible = false;
        countryPanel.SetActive(false);

        countryMeasurement.detailsPanel.ResetPanel();
        countryStatistic.detailsPanel.ResetPanel();

        VisualManager.Instance.UnhighlightCountryOutline();
    }

    private void UpdateOverview() {
        countryNameText.text = country.name;
        countryContinentNameText.text = country.Subregion.Continent.Name;
        countrySubregionNameText.text = country.Subregion.Name;
    }

    private void UpdateTraits() {
        if (isCountryPanelVisible) {
            foreach (Transform child in traitList.transform)
                Destroy(child.gameObject);
            for (int i = 0; i < country.GetTraitCount(); i++) {
                Trait trait = country.Traits[i];
                GameObject traitGameObject = Instantiate(traitPrefab);
                traitGameObject.transform.SetParent(traitList.transform, false);
                TraitPrefab prefab = traitGameObject.GetComponent<TraitPrefab>();
                prefab.Clone(trait);
            }
        }
    }

    public void UpdateStatistic() {
        if (country == null)
            return;

        countryStatistic.UpdateUI(country);
    }

    public void UpdateMeasurement() {
        if (country == null)
            return;

        countryMeasurement.UpdateUI(country);
    }
        
    public void UpdateEconomy() {
        if (country == null)
            return;

        countryEconomy.UpdateUI(country);
    }

    public void UpdateHappiness() {
        if (country == null)
            return;

        countryHappiness.UpdateUI(country);
    }
}
