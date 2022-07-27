using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WPMF
{
    public class Country : IAdminEntity
    {
        #region Properties
        private string _name;
        public string name { get => _name; set => _name = value; }

        private double _population;
        public double Population { get => _population; set => _population = value; }

        private double _openPopulation;
        public double OpenPopulation { get => _openPopulation; set => _openPopulation = value; }

        private Subregion _subregion;
        public Subregion Subregion { get => _subregion; set => _subregion = value; }

        private List<Country> _neighboringCountries;
        public List<Country> NeighboringCountries { get => _neighboringCountries; }

        private Infection _infection;
        public Infection Infection { get => _infection; }

        private Fatality _fatality;
        public Fatality Fatality { get => _fatality; }

        private Recovery _recovery;
        public Recovery Recovery { get => _recovery; }

        private Vaccination _vaccination;
        public Vaccination Vaccination { get => _vaccination; }

        private Stat _economy;
        public Stat Economy { get => _economy; }

        private Happiness _happiness;
        public Happiness Happiness { get => _happiness; }

        private Revolt _revolt;
        public Revolt Revolt { get => _revolt; }

        private readonly List<Trait> _traits;
        public List<Trait> Traits { get => _traits; }

        private List<Measurement> _measurements;
        public List<Measurement> Measurements { get => _measurements; }

        private CountryWorker _worker;
        public CountryWorker Worker { get => _worker; set => _worker = value; }
        #endregion

        #region Event Delegates
        public delegate void countryDelegate();
        public event countryDelegate traitUpdateEvent;
        public event countryDelegate measurementUpdateEvent;
        public static event countryDelegate economyUpdateEvent;
        public event countryDelegate happinessUpdateEvent;
        public event countryDelegate generalUpdateEvent;
        #endregion

        public Country(string name, string continent) {
            _name = name;
            _traits = new List<Trait>();
            _measurements = new List<Measurement>();

            //regions = new List<Region>();
            //this.continent = continent;
        }

        public void Initialize() {
            _neighboringCountries = WorldMap2D.instance.CountryNeighbours(WorldMap2D.instance.GetCountryIndex(name));

            _infection = new Infection(this);

            _fatality = new Fatality(this);

            _recovery = new Recovery(this);

            _vaccination = new Vaccination(this);

            _economy = new Stat();
            _economy.AddModifier(new Modifier("Base", Modifier.effectType.ADDITIVE, Modifier.modifierType.ECONOMY, Modifier.valueType.POSITIVE, (float)_population * 0.000000001f));

            _happiness = new Happiness("Happiness", 50);
            _happiness.AddModifier(new Modifier("Base", Modifier.effectType.ADDITIVE, Modifier.modifierType.HAPPINESS, Modifier.valueType.POSITIVE, 0f));

            _revolt = new Revolt();
            _revolt.AddModifier(new Modifier("Base", Modifier.effectType.ADDITIVE, Modifier.modifierType.REVOLT, Modifier.valueType.POSITIVE, -0.5f));
        }

        #region Trait Methods
        public void AddTrait(Trait trait) {
            _traits.Add(trait);

            foreach (Modifier modifier in trait.Modifiers) {
                Modifier newModifier = new Modifier(trait.Name, modifier.EffectType, modifier.ModifierType, modifier.ValueType, modifier.Value, modifier.SubModifierType);
                switch (modifier.ModifierType) {
                    case Modifier.modifierType.INFECTION:
                        _infection.AddModifier(newModifier);
                        InvokeGeneralUpdateEvent();
                        //Debug.Log("Modifier added to Infection Stat");
                        break;
                    case Modifier.modifierType.ECONOMY:
                        _economy.AddModifier(newModifier);
                        InvokeEconomyEvent();
                        //Debug.Log("Modifier added to Economy Stat");
                        break;
                    case Modifier.modifierType.HAPPINESS:
                        _happiness.AddModifier(newModifier);
                        InvokeHappinessEvent();
                        //Debug.Log("Modifier added to Happiness Stat");
                        break;
                    case Modifier.modifierType.REVOLT:
                        _revolt.AddModifier(newModifier);
                        InvokeHappinessEvent();
                        //Debug.Log("Modifier added to Revolt Stat");
                        break;
                    default:
                        Debug.LogError("Could not Match Modifier to existing Stats | (Country.cs : AddTrait(...)");
                        break;
                }
            }

            InvokeTraitEvent();
        }

        public void RemoveTrait(string name) {
            for (int i = 0; i < _traits.Count; i++) {
                if (_traits[i].Name.Equals(name)) {
                    RemoveTraitModifiersOnStat(_traits[i]);
                    _traits.RemoveAt(i);
                    InvokeTraitEvent();
                    return;
                }
            }
        }

        private void RemoveTraitModifiersOnStat(Trait trait) {
            foreach (Modifier modifier in trait.Modifiers) {
                Modifier toRemoveModifier = new Modifier(trait.Name, modifier.EffectType, modifier.ModifierType, modifier.ValueType, modifier.Value, modifier.SubModifierType);
                switch (modifier.ModifierType) {
                    case Modifier.modifierType.INFECTION:
                        _infection.RemoveModifier(toRemoveModifier);
                        InvokeGeneralUpdateEvent();
                        break;
                    case Modifier.modifierType.ECONOMY:
                        _economy.RemoveModifier(toRemoveModifier);
                        InvokeEconomyEvent();
                        break;
                    case Modifier.modifierType.HAPPINESS:
                        _happiness.RemoveModifier(toRemoveModifier);
                        InvokeHappinessEvent();
                        break;
                    case Modifier.modifierType.REVOLT:
                        _revolt.RemoveModifier(toRemoveModifier);
                        InvokeHappinessEvent();
                        break;
                    default:
                        Debug.LogError("Could not Find existing Stat to Remove Modifier | (Country.cs : RemoveTraitModifiersOnStat(...)");
                        break;
                }
            }
        }

        public int GetTraitCount() {
            return _traits.Count;
        }
        #endregion

        #region Measurement Methods
        public void AddMeasurement(Measurement measurement) {
            _measurements.Add(measurement);
        }
        #endregion

        #region Event Invokes
        public void InvokeTraitEvent() {
            if (traitUpdateEvent != null)
                traitUpdateEvent();
        }

        public void InvokeEconomyEvent() {
            economyUpdateEvent();
        }

        public void InvokeMeasurementEvent() {
            if (measurementUpdateEvent != null)
                measurementUpdateEvent();
        }

        public void InvokeHappinessEvent() {
            if (happinessUpdateEvent != null)
                happinessUpdateEvent();
        }

        public void InvokeGeneralUpdateEvent() {
            if (generalUpdateEvent != null)
                generalUpdateEvent();
        }
        #endregion

        #region Serialisation
        public void LoadFromSaveData(SaveData.CountryData cd) {

            /// <summary>
            /// Load Open Population
            /// </summary>
            _openPopulation = cd.openPopulation;

            /// <summary>
            /// Load Infection
            /// </summary>
            _infection.LoadFromSaveData(cd.infection);

            /// <summary>
            /// Load Fatality
            /// </summary>
            _fatality.LoadFromSaveData(cd.fatality);

            /// <summary>
            /// Load Fatality
            /// </summary>
            _recovery.LoadFromSaveData(cd.recovery);

            /// <summary>
            /// Load Vaccination
            /// </summary>
            _vaccination.LoadFromSaveData(cd.vaccination);

            /// <summary>
            /// Load Economy
            /// </summary>
            _economy.LoadFromSaveData(cd.economy);

            /// <summary>
            /// Load Happiness
            /// </summary>
            _happiness.LoadFromSaveData(cd.happiness);

            /// <summary>
            /// Load Revolt
            /// </summary>
            _revolt.LoadFromSaveData(cd.revolt);

            /// <summary>
            /// Load Country Traits
            /// </summary>
            _traits.Clear();
            foreach (SaveData.CountryTraitData td in cd.traits)
                _traits.Add(new Trait(td.name, td.description, new DateTime(td.expirationDate.year, td.expirationDate.month, td.expirationDate.day), td.icon, td.modifiers));

            /// <summary>
            /// Load Country Measurements State
            /// </summary>
            foreach (Measurement m in _measurements)
                foreach (SaveData.CountryMeasurementData md in cd.measurements)
                    if (m.Name.Equals(md.name))
                        m.SetActive(md.isActive);

        }
        #endregion

        #region AI API
        /*-------------------- AI API --------------------*/
        public void ToggleMeasurement(string measurementName, float state) {
            foreach (Measurement measurement in Measurements) {
                if (measurement.Name.Equals(measurementName)) {
                    int count = 2 + measurement.Family.Count;
                    float stepValue = 1f / count;
                    float step = stepValue;
                    for (int i = 0; i < count; i++) {
                        if (state <= step) {
                            if (i == 0) {
                                if (measurement.IsActive) {
                                    measurement.ExecuteLocal(false);
                                } else {
                                    foreach (Measurement sibling in measurement.Family) {
                                        if (sibling.IsActive) {
                                            sibling.ExecuteLocal(false);
                                            break;
                                        }
                                    }
                                }

                                //Debug.Log(measurementName + " -> Disabled | Output: " + state + " Step: " + step);

                                return;
                            } else if (i == 1) {
                                if (!measurement.IsActive)
                                    measurement.ExecuteLocal(true);

                                //Debug.Log(measurement.Name + " -> Enabled -> " + measurement.IsActive + " Output: " + state + " Step: " + step);

                                #region Statistics
                                if (measurement.IsActive && AIDataManager.Instance.saveGameStatistic) {
                                    AIDataManager.Instance.measurementDict[measurement.FamilyName][measurement.Name] = AIDataManager.Instance.measurementDict[measurement.FamilyName][measurement.Name] + 1;
                                }
                                #endregion

                                return;
                            } else {
                                if (!measurement.Family[i - 2].IsActive)
                                    measurement.Family[i - 2].ExecuteLocal(true);

                                //Debug.Log(measurement.Family[i - 2].Name + " -> Toggled -> " + measurement.Family[i - 2].IsActive + " Output: " + state + " Step: " + step);

                                #region Statistics
                                if (measurement.Family[i - 2].IsActive && AIDataManager.Instance.saveGameStatistic)
                                    AIDataManager.Instance.measurementDict[measurement.FamilyName][measurement.Family[i - 2].Name] = AIDataManager.Instance.measurementDict[measurement.FamilyName][measurement.Family[i-2].Name] + 1;
                                #endregion

                                return;
                            }
                        }

                        step += stepValue;
                    }
                }
            }

            Debug.LogError("Could not find Measurement with name: " + measurementName);
        }

        public float GetMeasurementFamilyState(string measurementName) {
            foreach (Measurement measurement in Measurements) {
                if (measurement.Name.Equals(measurementName)) {
                    if (measurement.IsActive) {
                        return CalculateState(measurement);
                    }
                    foreach (Measurement sibling in measurement.Family) {
                        if (sibling.IsActive) {
                            return CalculateState(sibling);
                        }
                    }
                    return 0;
                }
            }

            Debug.LogError("Could not find Measurement with name: " + measurementName);
            return -1;
        }

        private float CalculateState(Measurement measurement) {
            float steps = 1f / (1f + measurement.Family.Count);
            float order = float.Parse(String.Join("", measurement.SOName.Where(char.IsDigit)));
            float state = steps * order;
            //Debug.Log("M: " + measurement.Name);
            //Debug.Log("Steps: " + steps);
            //Debug.Log("Order: " + order);
            //Debug.Log("State: " + state);
            return state;
        }
        /*-------------------- AI API --------------------*/
        #endregion

        /*-------------------- Asset Code --------------------*/

        #region Asset Code

        /// <summary>
        /// List of all regions for the admin entity.
        /// </summary>
        public List<Region> regions { get; set; }

        public string continent { get; set; }

        /// <summary>
        /// Computed Rect area that includes all regions. Used to fast hovering.
        /// </summary>
        public Rect regionsRect2D;

        /// <summary>
        /// Setting hidden to true will hide completely the country (border, label) and it won't be highlighted
        /// </summary>
        public bool hidden;

        /// <summary>
        /// Center of the admin entity in the plane
        /// </summary>
        public UnityEngine.Vector2 center { get; set; }

        /// <summary>
        /// Index of the biggest region
        /// </summary>
        public int mainRegionIndex { get; set; }

        /// <summary>
        /// List of provinces that belongs to this country.
        /// </summary>
        public Province[] provinces;

        /// <summary>
        /// Optional custom label. It set, it will be displayed instead of the country Name.
        /// </summary>
        public string customLabel;

        /// <summary>
        /// Set it to true to specify a custom color for the label.
        /// </summary>
        public bool labelColorOverride;

        /// <summary>
        /// The color of the label.
        /// </summary>
        public Color labelColor = Color.white;
        Font _labelFont;
        Material _labelShadowFontMaterial;

        /// <summary>
        /// Internal method used to obtain the shadow material associated to a custom Font provided.
        /// </summary>
        /// <value>The label shadow font material.</value>
        public Material labelFontShadowMaterial { get { return _labelShadowFontMaterial; } }

        /// <summary>
        /// Optional font for this label. Note that the font material will be instanced so it can change color without affecting other labels.
        /// </summary>
        public Font labelFontOverride {
            get {
                return _labelFont;
            }
            set {
                if (value != _labelFont) {
                    _labelFont = value;
                    if (_labelFont != null) {
                        Material fontMaterial = UnityEngine.Object.Instantiate(_labelFont.material);
                        _labelFont.material = fontMaterial;
                        _labelShadowFontMaterial = UnityEngine.Object.Instantiate(fontMaterial);
                        _labelShadowFontMaterial.renderQueue--;
                    }
                }
            }
        }

        /// <summary>
        /// Sets whether the country Name will be shown or not.
        /// </summary>
        public bool labelVisible = true;

        /// <summary>
        /// If set to a value > 0 degrees then label will be rotated according to this value (and not automatically calculated).
        /// </summary>
        public float labelRotation = 0;

        /// <summary>
        /// If set to a value != 0 in both x/y then label will be moved according to this value (and not automatically calculated).
        /// </summary>
        public UnityEngine.Vector2 labelOffset = Misc.Vector2zero;


        /// <summary>
        /// If the label has its own font size.
        /// </summary>
        public bool labelFontSizeOverride = false;

        /// <summary>
        /// Manual font size for the label. Must set labelOverridesFontSize = true to have effect.
        /// </summary>
        public float labelFontSize = 0.2f;

        /// <summary>
        /// Used internally by Editor.
        /// </summary>
        public bool foldOut { get; set; }

        /// <summary>
        /// Set to false to prevent drawing provinces for this country
        /// </summary>
        public bool allowShowProvinces = true;


        #region internal fields
        // Used internally. Don't change fields below.
        public GameObject labelGameObject;
        public float labelMeshWidth, labelMeshHeight;
        public UnityEngine.Vector2 labelMeshCenter;
        #endregion

        public Country Clone() {
            Country c = new Country(_name, continent);
            c.center = center;
            c.regions = regions;
            c.customLabel = customLabel;
            c.labelColor = labelColor;
            c.labelColorOverride = labelColorOverride;
            c.labelFontOverride = labelFontOverride;
            c.labelVisible = labelVisible;
            c.labelOffset = labelOffset;
            c.labelRotation = labelRotation;
            c.provinces = provinces;
            c.hidden = this.hidden;
            return c;
        }

        #endregion

        /*-------------------- Asset Code --------------------*/
    }
}
