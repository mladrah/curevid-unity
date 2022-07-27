using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WPMF;

namespace Game
{
    public class Event : ResponsiveContentBox
    {
        #region UI Components
        [Header("Concrete UI Components")]
        public TextMeshProUGUI area;
        public GameObject optionsPanel;
        [SerializeField] private GameObject optionsPrefab;
        [SerializeField] private Image areaIcon;
        #endregion

        #region Independent Properties
        private bool _isOccured;
        public bool IsOccured { get => _isOccured; }

        private Event _visibleEvent;
        public Event VisibleEvent { get => _visibleEvent; }
        #endregion

        #region SO Properties
        private EventSO _reference;
        
        public string Title { get => _reference.Title; }

        public string Description { get => _reference.Description; }

        public int Duration { get => _reference.Duration; }

        public List<Option> Options { get => _reference.Options; }

        public Area Area { get => _reference.Area; }

        public Date Date { get => _reference.Date; }
        #endregion

        public Event(EventSO reference) {
            _reference = reference;
        }

        public override void Awake() {
            base.Awake();
        }

        #region Set Content
        public void SetEvent(Event e) {
            _visibleEvent = e;
            _reference = _visibleEvent._reference;

            base.SetText(_reference.Description, _reference.Title);

            foreach (Transform child in optionsPanel.transform)
                Destroy(child.gameObject);

            foreach (Option o in _reference.Options) {
                GameObject go = Instantiate(optionsPrefab);
                OptionPrefab prefab = go.GetComponent<OptionPrefab>();
                prefab.Clone(o, this);
                prefab.transform.SetParent(optionsPanel.transform, false);
            }

            area.text = "";

            switch (_reference.Area) {
                case Area.Global:
                    areaIcon.sprite = Images.Instance.area_global;
                    area.text = "Global";
                    break;
                case Area.Continental:
                    areaIcon.sprite = Images.Instance.area_continental;
                    foreach (string a in _reference.AreaNames)
                        area.text += " " + a + ",";
                    break;
                case Area.Subregional:
                    areaIcon.sprite = Images.Instance.area_subregional;
                    foreach (string a in _reference.AreaNames)
                        area.text += " " + a + ",";
                    break;
                case Area.Local:
                    areaIcon.sprite = Images.Instance.area_local;
                    foreach (string a in _reference.AreaNames)
                        area.text += " " + a + ",";
                    break;
            }

            area.text = area.text.TrimEnd(',');
        }
        #endregion

        public void ApplyEvent(List<Modifier> modifiers) {
            Trait t = new Trait(_reference.Title, _reference.Description, _reference.Duration, Images.Instance.icon_placeholder, modifiers);

            switch (_reference.Area) {
                case Area.Global:
                    foreach (Country c in WorldMap2D.instance.countries)
                        c.AddTrait(t);
                    break;
                case Area.Continental:
                    foreach (string area in _reference.AreaNames) {
                        Continent con = null;
                        if (GlobalManager.Instance.Continents.TryGetValue(area, out con)) {
                            foreach (KeyValuePair<string, Subregion> element in con.Subregions) {
                                Subregion sub = element.Value;
                                foreach (KeyValuePair<string, Country> subElement in sub.Countries) {
                                    Country c = subElement.Value;
                                    c.AddTrait(t);
                                }
                            }
                        }
                    }
                    break;
                case Area.Subregional:
                    foreach (string area in _reference.AreaNames) {
                        Subregion sub = null;
                        if (GlobalManager.Instance.Subregions.TryGetValue(area, out sub)) {
                            foreach (KeyValuePair<string, Country> element in sub.Countries) {
                                Country c = element.Value;
                                c.AddTrait(t);
                            }
                        }
                    }
                    break;
                case Area.Local:
                    foreach (string area in _reference.AreaNames) {
                        Country c = null;
                        if (GlobalManager.Instance.Countries.TryGetValue(area, out c))
                            c.AddTrait(t);
                    }
                    break;
                default:
                    Debug.LogError("Could not match Area | (Event.cs : ApplyEvent(...))");
                    break;
            }

            _visibleEvent._isOccured = true;

            EventManager.Instance.Hide();
        }
        
        public void LoadFromSaveData(SaveData.EventData eventData) {
            _isOccured = eventData.isOccured;
        }
    }

}
