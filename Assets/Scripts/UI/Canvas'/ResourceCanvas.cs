using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ResourceCanvas : MonoBehaviour
{
    private static ResourceCanvas _instance;
    public static ResourceCanvas Instance { get => _instance; }

    public TextMeshProUGUI economyTotalValue;
    public TextMeshProUGUI economyDailyChange;

    public Slider researchProgress;
    public Image researchProgressFill;
    public TextMeshProUGUI researchProgressText;
    public Image researchIcon;

    private void Awake() {
        _instance = this;
    }

    private void Start() {
        UpdateResearchUI(0);
    }

    public void UpdateResourcesUI() {
        _instance.economyTotalValue.text = "" + Format.Stat(ResourceManager.Instance.GlobalEconomy.TotalValue);

        float dailyChange = Calculate.Stat(ResourceManager.Instance.GlobalEconomy);
        _instance.economyDailyChange.text = Format.StatColoredString(dailyChange);

        TooltipTrigger ttDailyChange = _instance.economyDailyChange.GetComponent<TooltipTrigger>();
        ttDailyChange.modifiers = Format.Modifiers(ResourceManager.Instance.GlobalEconomy.Modifiers);
    }

    public void UpdateResearchUI(float value, float duration = 0.25f) {
        if (ResearchManager.Instance.Research != null) {
            researchIcon.color = Colors.HexToColor(Colors.HEX_BLUE);
            researchIcon.material = ResearchCanvas.Instance.researchInProgressMaterial;
            researchProgressFill.material = ResearchCanvas.Instance.researchInProgressMaterial;
            researchProgressText.color = Colors.HexToColor(Colors.HEX_BLUE);
            researchProgress.DOValue(value, duration);
            researchProgressText.text = Format.Percentage(value);
        } else {
            researchIcon.color = Colors.WHITE;
            researchIcon.material = null;
            researchProgressFill.material = null;
            researchProgressText.color = Colors.WHITE;
            researchProgress.DOValue(0, duration);
            researchProgressText.text = "---";
        }
    }
}