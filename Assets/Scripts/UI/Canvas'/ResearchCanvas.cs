using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using WPMF;

public class ResearchCanvas : DynamicCanvas
{
    private static ResearchCanvas _instance;
    public static ResearchCanvas Instance { get => _instance; }

    public GameObject researchPanel;

    [Header("Cure")]
    [SerializeField] private TextMeshProUGUI cureName;
    [SerializeField] private TextMeshProUGUI statEffectivity;
    [SerializeField] private TextMeshProUGUI statSafety;
    [SerializeField] private TextMeshProUGUI cureDescription;

    [Header("Research Tree")]
    [SerializeField] private ScrollRect treeScrollRect;

    [Header("Research Details")]
    [SerializeField] private DetailsPanel detailsPanel;
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI researchButtonLabel;
    public Material researchInProgressMaterial;
    [SerializeField] private GameObject researchInProgressPanel;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TextMeshProUGUI alreadyResearchedText;
    [SerializeField] private Image researchIcon;
    [SerializeField] private TextMeshProUGUI researchName;
    [SerializeField] private TextMeshProUGUI researchDuration;
    [SerializeField] private TextMeshProUGUI researchModifiers;
    [SerializeField] private TextMeshProUGUI researchDescription;
    [SerializeField] private TextMeshProUGUI researchCost;
    [SerializeField] private TextMeshProUGUI researchDailyCost;
    [SerializeField] private ScrollRect researchScrollRect;
    private Research focusedResearch;

    [Header("Animation")]
    private Tween progressTween;

    public override void Awake() {
        base.Awake();
        _instance = this;

        progressSlider.value = 0;
    }

    private void Start() {
        ResourceManager.Instance.globalEconomyChangeDelegate += EventResearchCostCheck;

        startButton.onClick.AddListener(StartResearch);

        UpdateCureUI();
        HidePanel();
    }

    public override void HidePanel() {
        base.HidePanel();
        researchPanel.SetActive(false);

        if (detailsPanel.gameObject.activeInHierarchy)
            detailsPanel.ResetPanel();
        treeScrollRect.verticalNormalizedPosition = 0;
    }

    public override void ShowPanel() {
        base.ShowPanel();
        researchPanel.SetActive(true);
    }

    public void UpdateFocusedResearch(Research research) {
        if (focusedResearch != research)
            researchScrollRect.verticalNormalizedPosition = 1;

        focusedResearch = research;
        researchIcon.sprite = research.Icon;
        researchName.text = research.Name;
        researchDuration.text = research.Duration + " days";
        researchModifiers.text = Format.Modifiers(research.Modifiers);
        researchDescription.text = research.Description;
        researchCost.text = "" + Format.Stat(research.Cost);
        researchDailyCost.text = "" + Format.Stat(research.DailyCost) + " /day";

        UpdateResearchButtonState();

        ShowDetailsPanel();
    }

    /// <summary>
    /// Determines the interactability and state of the Button
    /// </summary>
    private void UpdateResearchButtonState() {
        if (focusedResearch == null)
            return;
        
        alreadyResearchedText.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
        researchInProgressPanel.SetActive(false);

        if (focusedResearch.IsResearched) {

            alreadyResearchedText.gameObject.SetActive(true);

        } else if (focusedResearch.IsResearching) {

            researchInProgressPanel.SetActive(true);

        } else if (focusedResearch.CanBeResearched) {

            if (ResearchManager.Instance.Research != null) {
                startButton.interactable = false;
                researchButtonLabel.text = "Currently Researching...";
                researchButtonLabel.color = new Color(Colors.WHITE.r, Colors.WHITE.g, Colors.WHITE.b, 0.5f);
            } else {
                if (ResourceManager.Instance.GlobalEconomy.TotalValue < Mathf.Abs(focusedResearch.Cost)) {
                    startButton.interactable = false;
                    researchButtonLabel.text = "Not enough Cure Coins";
                    researchButtonLabel.color = new Color(researchButtonLabel.color.r, researchButtonLabel.color.g, researchButtonLabel.color.b, 0.5f);
                } else {
                    researchButtonLabel.color = Colors.WHITE;
                    startButton.interactable = true;
                    researchButtonLabel.text = "Start";
                }
            }

            startButton.gameObject.SetActive(true);
        }
    }

    public void StartResearch() {
        if (focusedResearch == null) {
            Debug.LogError("Can not start Research: Focused Research is null");
            return;
        }
        UpdateResearchProgressUI(0, 0);

        ResearchManager.Instance.StartResearch(focusedResearch);

        ResearchManager.Instance.Research.UpdateUI();

        UpdateResearchButtonState();
    }

    public void CancelResearch() {
        UpdateResearchProgressUI(0, 0);
        
        ResearchManager.Instance.CancelResearch().UpdateUI();

        ResourceCanvas.Instance.UpdateResearchUI(0);

        UpdateResearchButtonState();
    }

    public void ResearchOnComplete(Research completedResearch) {
        UpdateResearchProgressUI(0, 0);

        UpdateCureUI();

        completedResearch.UpdateUI();

        UpdateNextResearch(completedResearch);

        UpdateResearchButtonState();
    }

    public void UpdateResearchProgressUI(float value, float duration = 0.5f) {
        progressTween = progressSlider.DOValue(value, duration).SetEase(Ease.OutExpo);
    }

    private void UpdateNextResearch(Research completedResearch) {
        if (completedResearch.Parents.Count == 0 && completedResearch.Siblings.Count == 0) {

            LogManager.Log("<b>Vaccines will be distributed!</b>", "", Colors.HEX_GREEN);
            GlobalManager.Instance.DistributeVaccines();

        } else {

            bool nextResearchOpen = true;

            foreach (Research research in completedResearch.Siblings) {

                if (research.IsMandatory && !research.IsResearched)
                    nextResearchOpen = false;

            }

            if (nextResearchOpen) {

                foreach (Research research in completedResearch.Parents) {
                    research.CanBeResearched = true;
                    research.UpdateUI();

                }
            }
        }
    }

    public void UpdateCureUI() {
        cureName.text = Cure.Instance.Name;
        statEffectivity.text = Format.StatColoredString(Calculate.StatMultiplicative(Cure.Instance.Effectivity), "", " " + Cure.Instance.Effectivity.Name, false, true);
        statSafety.text = Format.StatColoredString(Calculate.StatMultiplicative(Cure.Instance.Safety), "", " " + Cure.Instance.Safety.Name, false, true);

        statEffectivity.GetComponent<TooltipTrigger>().modifiers = Format.Modifiers(Cure.Instance.Effectivity.Modifiers);
        statSafety.GetComponent<TooltipTrigger>().modifiers = Format.Modifiers(Cure.Instance.Safety.Modifiers);

        cureDescription.text = Cure.Instance.Description;
    }

    private void EventResearchCostCheck() {
        if (detailsPanel.IsVisible) {
            UpdateFocusedResearch(focusedResearch);
        }
    }

    private void ShowDetailsPanel() {
        if (detailsPanel.IsVisible)
            return;

        detailsPanel.ShowTween();
    }

    /*-------------------- AI API --------------------*/
    public Research SetFocusedResearch(Research r) {
        return focusedResearch = r;
    }
    /*-------------------- AI API --------------------*/

}
