using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using WPMF;
using TMPro;
using DG.Tweening;

public class CountryStatistic : MonoBehaviour
{
    private Country country;

    #region General
    [Header("General")]
    [SerializeField] private TextMeshProUGUI populationNum;
    #endregion

    #region Details Panels
    [Header("Details Panels")]
    public DetailsPanel detailsPanel;
    [SerializeField] private GameObject infectionDetailsPanel;
    [SerializeField] private ScrollRect infectionDetailsScrollRect;

    [SerializeField] private GameObject fatalityDetailsPanel;
    [SerializeField] private ScrollRect fatalityDetailsScrollRect;

    [SerializeField] private GameObject vaccinationDetailsPanel;
    [SerializeField] private ScrollRect vaccinationDetailsScrollRect;
    #endregion

    #region Infection On Detail
    [Header("Infection")]
    [SerializeField] private Button infectionBtn;
    [SerializeField] private TextMeshProUGUI infectionNum;
    [SerializeField] private TextMeshProUGUI infectionPercentage;
    [SerializeField] private StatisticCell dailyConfirmedCases;
    [SerializeField] private StatisticCell basicReproductionRate;
    [SerializeField] private StatisticCell incidenceRate;
    [SerializeField] private List<StatisticCell> causeStatistics;
    #endregion

    #region Fatality On Detail
    [Header("Fatality")]
    [SerializeField] private Button fatalityBtn;
    [SerializeField] private TextMeshProUGUI fatalityNum;
    [SerializeField] private TextMeshProUGUI fatalityPercentage;
    [SerializeField] private StatisticCell dailyFatalities;
    [SerializeField] private StatisticCell crudeMortalitiyRate;
    [SerializeField] private StatisticCell fatalityVirusSource;
    [SerializeField] private StatisticCell fatalityCureSource;
    #endregion

    #region Vaccination On Detail
    [Header("Vaccination")]
    [SerializeField] private Button vaccinationBtn;
    [SerializeField] private TextMeshProUGUI vaccinationNum;
    [SerializeField] private TextMeshProUGUI vaccinationPercentage;
    [SerializeField] private StatisticCell dailyVaccinations;
    [SerializeField] private StatisticCell sideEffectFatalities;
    #endregion

    #region Recovery On Detail
    [Header("Recovery")]
    [SerializeField] private TextMeshProUGUI recoveryNum;
    [SerializeField] private TextMeshProUGUI recoveryPercentage;
    #endregion

    #region Animation
    [Header("Animation")]
    private Tween punchTween;
    public const float punchScale = 0.3f;
    #endregion
    private void Start() {
        if (!detailsPanel.gameObject.activeInHierarchy)
            Debug.LogError("Details Panel is InActive! | (CountryStatistic.cs)");

        detailsPanel.onHideAction = ResetDetailsButtons;

        infectionBtn.onClick.AddListener(OnClickInfection);
        fatalityBtn.onClick.AddListener(OnClickFatality);
        vaccinationBtn.onClick.AddListener(OnClickVaccination);
    }

    private void OnDisable() {
        detailsPanel.HideTween();
        ResetDetailsPanels();
    }

    #region General UI Update
    public void UpdateUI(Country country) {
        this.country = country;
        if (this.country == null)
            return;

        populationNum.text = Format.LargeNumber(country.Population);

        UpdateInfectionUI();
        UpdateFatalityUI();
        UpdateVaccinationUI();
        UpdateRecoveryUI();
    }
    #endregion

    #region Details Behaviour
    public void OnClickInfection() {
        PunchAnimation(infectionBtn.transform);
        infectionDetailsScrollRect.verticalNormalizedPosition = 1;
        ResetDetailsPanels();
        ShowDetailsPanel(infectionDetailsPanel);

        infectionBtn.image.color = Colors.HexToColor(Colors.HEX_RED);
    }

    public void OnClickFatality() {
        PunchAnimation(fatalityBtn.transform);
        fatalityDetailsScrollRect.verticalNormalizedPosition = 1;
        ResetDetailsPanels();
        ShowDetailsPanel(fatalityDetailsPanel);

        fatalityBtn.image.color = Colors.HexToColor(Colors.HEX_YELLOW);
    }

    public void OnClickVaccination() {
        PunchAnimation(vaccinationBtn.transform);
        vaccinationDetailsScrollRect.verticalNormalizedPosition = 1;
        ResetDetailsPanels();
        ShowDetailsPanel(vaccinationDetailsPanel);

        vaccinationBtn.image.color = Colors.BLUE;
    }

    private void PunchAnimation(Transform transform) {
        punchTween.Kill();
        transform.localScale = new Vector2(1f, 1);
        punchTween = UIManager.Instance.PunchScale(transform, new Vector2(punchScale, punchScale), 0.2f);
    }

    private void ResetDetailsPanels() {
        infectionDetailsPanel.SetActive(false);
        fatalityDetailsPanel.SetActive(false);
        vaccinationDetailsPanel.SetActive(false);

        ResetDetailsButtons();
    }

    private void ResetDetailsButtons() {
        infectionBtn.image.color = Colors.HexToColor(Colors.HEX_WHITE);
        fatalityBtn.image.color = Colors.HexToColor(Colors.HEX_WHITE);
        vaccinationBtn.image.color = Colors.HexToColor(Colors.HEX_WHITE);
    }

    private void ShowDetailsPanel(GameObject panel) {
        panel.SetActive(true);

        detailsPanel.ShowTween();
    }
    #endregion

    #region Infection UI Update
    private void UpdateInfectionUI() {
        infectionNum.text = Format.LargeNumber(country.Infection.TotalValue);
        infectionPercentage.text = Format.Percentage(country.Infection.TotalValue/country.Population);

        dailyConfirmedCases.num.text = Format.LargeNumber(country.Infection.NewCases);
        dailyConfirmedCases.ColorizeCondition(() => country.Infection.NewCases >= 1, Colors.HEX_RED, Colors.HEX_GREEN);

        UpdateIncidenceCell();

        UpdateRAndCauseCells();
    }

    private void UpdateIncidenceCell() {
        incidenceRate.num.text = Format.LargeNumber(country.Infection.IncidenceRate);
        incidenceRate.ColorizeCondition(() => country.Infection.IncidenceRate >= 1, Colors.HEX_RED, Colors.HEX_GREEN);

        TooltipTrigger tt = incidenceRate.GetComponent<TooltipTrigger>();
        if (country.Infection.IncidenceHistory.Count > 0) {
            string formattedIncidenceHistoryString = "";

            foreach (Cases c in country.Infection.IncidenceHistory)
                formattedIncidenceHistoryString += c.date.Day + "." + c.date.Month + "." + c.date.Year + "" +
                    "\t" + Format.ColorString(Format.LargeNumber(c.count), Colors.HEX_RED) + "\n";

            tt.modifiers = formattedIncidenceHistoryString;
        } else {
            tt.modifiers = "No History";
        }
    }

    private void UpdateRAndCauseCells() {
        basicReproductionRate.num.text = Format.Stat(country.Infection.RValue);
        basicReproductionRate.ColorizeCondition(() => country.Infection.RValue >= 1, Colors.HEX_RED, Colors.HEX_GREEN);

        TooltipTrigger ttR = basicReproductionRate.GetComponent<TooltipTrigger>();
        ttR.header = "<b>" + Format.ColorStringCondition(() => country.Infection.RValue >= 1, Format.Stat(country.Infection.RValue), Colors.HEX_RED, Colors.HEX_GREEN) + "</b>";
        string formattedBasicReproductionRateStats = "";

        int index = 0;
        foreach (KeyValuePair<string, Stat> element in country.Infection.SocietyDictionary) {
            Stat stat = element.Value;

            if (causeStatistics[index].label.text != stat.Name) {
                //Debug.LogWarning("Cause Statistic List does not Match SocietyDictionary Order");
                //return;
            }

            formattedBasicReproductionRateStats += Format.ColorString("+" + Format.Stat(Calculate.Stat(stat)), Colors.HEX_RED) + "\t" + stat.Name + "\n";

            causeStatistics[index].num.text = Format.LargeNumber(country.Infection.CalculateCauseShare(stat));
            causeStatistics[index].ColorizeCondition(() => country.Infection.CalculateCauseShare(stat) > 0, Colors.HEX_RED, Colors.HEX_GREEN);

            TooltipTrigger ttC = causeStatistics[index].GetComponent<TooltipTrigger>();
            ttC.header = Format.Stat(Calculate.Stat(stat));
            ttC.modifiers = Format.Modifiers(stat.Modifiers);

            index++;
        }

        formattedBasicReproductionRateStats += Format.ColorString("-" + Format.Stat(country.Infection.RecoveryRInfluence), Colors.HEX_GREEN) + "\tRecovered\n";
        formattedBasicReproductionRateStats += Format.ColorString("-" + Format.Stat(country.Infection.VaccinatedRInfluence), Colors.HEX_GREEN) + "\tVaccinated";

        ttR.modifiers = formattedBasicReproductionRateStats;
    }
    #endregion

    #region Fatality UI Update
    private void UpdateFatalityUI() {
        fatalityNum.text = Format.LargeNumber(country.Fatality.TotalValue);

        fatalityPercentage.text = Format.Percentage(country.Fatality.TotalValue / country.Population);

        dailyFatalities.num.text = Format.LargeNumber(country.Fatality.NewFatalities);
        dailyFatalities.ColorizeCondition(() => country.Fatality.NewFatalities >= 1, Colors.HEX_RED, Colors.HEX_GREEN);

        crudeMortalitiyRate.num.text = Format.LargeNumber(country.Fatality.CrudeMortalityRate);
        crudeMortalitiyRate.ColorizeCondition(() => country.Fatality.CrudeMortalityRate > 0, Colors.HEX_RED, Colors.HEX_GREEN);

        fatalityVirusSource.num.text = Format.LargeNumber(country.Fatality.VirusSource);
        fatalityVirusSource.ColorizeCondition(() => country.Fatality.VirusSource >= 1, Colors.HEX_RED, Colors.HEX_GREEN);

        fatalityCureSource.num.text = Format.LargeNumber(country.Fatality.CureSource);
        fatalityCureSource.ColorizeCondition(() => country.Fatality.CureSource >= 1, Colors.HEX_RED, Colors.HEX_GREEN);
    }
    #endregion

    #region Vaccination UI Update
    private void UpdateVaccinationUI() {
        if (country == null)
            return;

        vaccinationNum.text = Format.LargeNumber(country.Vaccination.TotalValue);

        vaccinationPercentage.text = Format.Percentage(country.Vaccination.TotalValue / country.Population);

        dailyVaccinations.num.text = Format.LargeNumber(country.Vaccination.NewVaccinated);
        dailyVaccinations.ColorizeCondition(() => country.Vaccination.NewVaccinated >= 0, Colors.HEX_GREEN, Colors.HEX_RED);

        sideEffectFatalities.num.text = Format.LargeNumber(country.Vaccination.SideEffectFatalities);
        sideEffectFatalities.ColorizeCondition(() => country.Vaccination.SideEffectFatalities > 0, Colors.HEX_RED, Colors.HEX_GREEN);
    }
    #endregion

    #region Recovery UI Update
    private void UpdateRecoveryUI() {
        recoveryNum.text = Format.LargeNumber(country.Recovery.TotalValue);

        recoveryPercentage.text = Format.Percentage(country.Recovery.TotalValue / country.Population);
    }
    #endregion
}
