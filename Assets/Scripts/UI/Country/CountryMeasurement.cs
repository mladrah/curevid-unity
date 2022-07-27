using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WPMF;
using TMPro;
using DG.Tweening;

public class CountryMeasurement : MonoBehaviour
{
    private Country country;
    private Country formerCountry;

    #region General
    [Header("General")]
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject measurementFamilyPrefab;
    [SerializeField] private GameObject measurementPrefab;
    private List<MeasurementPrefab> instantiatedMeasurements;
    #endregion

    #region Details Panel
    [Header("Details Panel")]
    public DetailsPanel detailsPanel;
    [SerializeField] private ScrollRect detailsScrollRect;
    #endregion

    #region Measurement On Detail
    [Header("Measurement On Detail")]
    [SerializeField] private Image measurementImage;
    [SerializeField] private TextMeshProUGUI measurementName;
    [SerializeField] private TextMeshProUGUI measurementDuration;
    [SerializeField] private TextMeshProUGUI measurementFamily;
    [SerializeField] private TextMeshProUGUI measurementModifiers;
    [SerializeField] private TextMeshProUGUI measurementDescription;
    [SerializeField] private Toggle localToggle;
    [SerializeField] private Toggle subregionalToggle;
    [SerializeField] private Toggle continentalToggle;
    [SerializeField] private Button executeBtn;
    [SerializeField] private Button disableBtn;
    private Measurement measurement;
    #endregion

    private void Start() {
        if (!detailsPanel.gameObject.activeInHierarchy)
            Debug.LogError("Details Panel is InActive! | (CountryMeasurement.cs)");

        executeBtn.onClick.AddListener(OnClickExecute);
        disableBtn.onClick.AddListener(OnClickDisable);
        detailsPanel.onHideAction = () => localToggle.isOn = true;
    }

    private void OnDisable() {
        detailsPanel.HideTween();
    }

    #region General UI Update
    public void UpdateUI(Country country) {
        formerCountry = this.country;
        this.country = country;

        if (CountryCanvas.instance.isNewCountry) {
            InitUI();
        } else {
            foreach (MeasurementPrefab mp in instantiatedMeasurements) {
                mp.SetVisual();
            }
        }

        if (detailsPanel.IsVisible)
            UpdateMeasurementDetailsUI();
    }

    private void InitUI() {
        instantiatedMeasurements = new List<MeasurementPrefab>();
        foreach (Transform child in content.transform)
            Destroy(child.gameObject);

        foreach (Measurement m in country.Measurements) {
            if (IsAlreadyInstantiated(m.Name))
                continue;

            GameObject measurementFamilyGameObject = Instantiate(measurementFamilyPrefab);
            measurementFamilyGameObject.name = m.Name + " Family";
            measurementFamilyGameObject.transform.SetParent(content.transform, false);

            GameObject measurementGameObject = Instantiate(measurementPrefab);
            measurementGameObject.name = m.Name;
            measurementGameObject.transform.SetParent(measurementFamilyGameObject.transform, false);
            MeasurementPrefab prefab = measurementGameObject.GetComponent<MeasurementPrefab>();
            prefab.Clone(m);
            prefab.OnClickEvent += ShowDetailsPanel;

            instantiatedMeasurements.Add(prefab);

            foreach (Measurement mSibling in m.Family) {
                measurementGameObject = Instantiate(measurementPrefab);
                measurementGameObject.name = mSibling.Name;
                measurementGameObject.transform.SetParent(measurementFamilyGameObject.transform, false);
                prefab = measurementGameObject.GetComponent<MeasurementPrefab>();
                prefab.Clone(mSibling);
                prefab.OnClickEvent += ShowDetailsPanel;

                instantiatedMeasurements.Add(prefab);
            }
        }
    }

    private bool IsAlreadyInstantiated(string name) {
        return instantiatedMeasurements.Any(m => m.name.Equals(name));
    }
    #endregion

    #region Details Behaviour
    private void ShowDetailsPanel(Measurement measurement) {
        this.measurement = measurement;

        detailsPanel.ShowTween();

        UpdateMeasurementDetailsUI();
    }
    #endregion

    #region Details UI Update
    private void UpdateMeasurementDetailsUI() {
        if (formerCountry != null) {
            foreach (Measurement m in country.Measurements) {
                if (m.Name.Equals(measurement.Name)) {
                    measurement = m;
                }
            }
        }

        detailsScrollRect.verticalNormalizedPosition = 1;
        measurementImage.sprite = measurement.Icon;
        measurementName.text = measurement.Name;
        measurementDuration.text = measurement.ActiveDuration >= 0 ? measurement.ActiveDuration + " days" : "Unlimited";

        //measurementFamily.text = "";
        //foreach (Measurement m in measurement.Family)
        //    measurementFamily.text += ("<color=" + (m.IsActive ? Colors.HEX_YELLOW : Colors.HEX_WHITE)) + ">" + m.Name + "</color>" + "\n";

        //if (measurement.Family.Count == 0)
        //    measurementFamily.gameObject.SetActive(false);
        //else
        //    measurementFamily.gameObject.SetActive(true);

        measurementModifiers.text = Format.Modifiers(measurement.Modifiers);
        measurementDescription.text = measurement.Description;

        if (measurement.IsActive) {
            disableBtn.gameObject.SetActive(true);
            executeBtn.gameObject.SetActive(false);
        } else {
            disableBtn.gameObject.SetActive(false);
            executeBtn.gameObject.SetActive(true);
        }
    }

    private void OnClickExecute() {
        if (localToggle.isOn)
            measurement.ExecuteLocal(true);
        else if (subregionalToggle.isOn)
            measurement.ExecuteSubregional(true);
        else if (continentalToggle.isOn)
            measurement.ExecuteContinental(true);

        detailsPanel.HideTween();
    }

    private void OnClickDisable() {
        if (localToggle.isOn)
            measurement.ExecuteLocal(false);
        else if (subregionalToggle.isOn)
            measurement.ExecuteSubregional(false);
        else if (continentalToggle.isOn)
            measurement.ExecuteContinental(false);

        detailsPanel.HideTween();
    }
    #endregion
}
