using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPMF;

public class Measurement
{
    #region Independent Properties
    private readonly Country _country;
    public Country Country { get => _country; }

    private bool _isActive;
    public bool IsActive { get => _isActive; }
    #endregion

    #region SO Properties
    private readonly MeasurementSO _reference;

    public string SOName { get => _reference.name; }

    public string Name { get => _reference.Name; }

    public string Description { get => _reference.Description; }

    public int ActiveDuration { get => _reference.ActiveDuration; }

    public Sprite Icon { get => _reference.Icon; }

    public List<Modifier> Modifiers { get => _reference.Modifiers; }

    private readonly List<Measurement> _family;
    public List<Measurement> Family { get => _family; }

    public string FamilyName { get => _reference.FamilyName; }
    #endregion

    public Measurement(MeasurementSO reference, Country country) {
        _reference = reference;
        _country = country;
        _family = new List<Measurement>();
    }

    #region Methods
    public void SetActive(bool isActive) {
        _isActive = isActive;
    }

    public void FindSiblings() {
        if (_family.Count < _reference.Family.Count) {
            foreach (MeasurementSO mso in _reference.Family) {
                foreach (Measurement m in _country.Measurements) {
                    if (m.Name.Equals(mso.Name)) {
                        if (!_family.Contains(m)) {
                            _family.Add(m);
                        }
                    }
                }
            }
        }
    }

    public void ExecuteLocal(bool toActivate) {
        if(_isActive != toActivate) {
            _isActive = toActivate;
        
            foreach (Measurement m in _family) {
                if (toActivate) {
                    if (m.IsActive) {
                        m.ExecuteLocal(false);
                    }
                }
            }
            
            _country.Worker.UpdateMeasurement(this);
        }
    }

    public void ExecuteSubregional(bool toActivate, Subregion subregion = null) {
        if (subregion == null)
            subregion = _country.Subregion;

        foreach (KeyValuePair<string, Country> element in subregion.Countries) {
            Country c = element.Value;
            Measurement cM = c.Measurements.Find(m => m.Name.Equals(_reference.Name));
            cM.ExecuteLocal(toActivate);
        }
    }

    public void ExecuteContinental(bool toActivate) {
        foreach (KeyValuePair<string, Subregion> element in _country.Subregion.Continent.Subregions) {
            Subregion s = element.Value;
            ExecuteSubregional(toActivate, s);
        }
    }
    #endregion
}
