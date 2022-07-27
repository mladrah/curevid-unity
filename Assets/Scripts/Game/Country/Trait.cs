using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Trait
{
    #region Properties
    protected string _name;
    public string Name { get => _name; }

    protected string _description;
    public string Description { get => _description; }

    protected Sprite _icon;
    public Sprite Icon { get => _icon; }

    protected List<Modifier> _modifiers;
    public List<Modifier> Modifiers { get => _modifiers; }

    protected DateTime _expirationDate;
    public DateTime ExpirationDate { get => _expirationDate; }
    #endregion

    public Trait(string name, string description, int durationInDays, Sprite icon, List<Modifier> modifiers) {
        _name = name;
        _description = description;
        _icon = icon;
        _modifiers = modifiers;
        _expirationDate = durationInDays <= 0 ? DateTime.MinValue : TimeManager.Instance.GetCurrentDate().AddDays(durationInDays);
    }

    public Trait(string name, string description, DateTime expiratonDate, Sprite icon, List<Modifier> modifiers) {
        _name = name;
        _description = description;
        _icon = icon;
        _modifiers = modifiers;
        _expirationDate = expiratonDate;
    }

    public Trait() { }

    #region Methods
    public void AddModifier(Modifier modifier) {
        _modifiers.Add(modifier);
    }
    #endregion
}
