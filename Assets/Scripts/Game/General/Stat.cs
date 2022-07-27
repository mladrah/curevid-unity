using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat
{
    #region 
    private readonly string _name;
    public string Name { get => _name; }

    private double _totalValue;
    public double TotalValue { get => _totalValue; }

    private List<Modifier> _modifiers;
    public List<Modifier> Modifiers { get => _modifiers; }
    #endregion

    public Stat() { 
        _modifiers = new List<Modifier>();
    }

    public Stat(string name, List<Modifier> modifiers) {
        _name = name;
        _modifiers = modifiers;
    }

    public Stat(string name = default, float totalValue = default) : this(){
        _name = name;
        _totalValue = totalValue;
    }

    public Stat(string name, Modifier baseModifier) : this(){
        _name = name;
        _modifiers.Add(baseModifier);
    }

    #region Methods
    public virtual void RemoveModifier(Modifier modifier) {
        RemoveModifier(modifier.Name);
    }

    public void RemoveModifier(string modifierName) {
        for (int i = 0; i < Modifiers.Count; i++) {
            if (Modifiers[i].Name.Equals(modifierName)) {
                Modifiers.RemoveAt(i);
                //Debug.Log("Modifier (" + modifier.Name + ") removed from " + modifier.ModifierType + " Stat");
            }
        }
    }

    public virtual void AddModifier(Modifier modifier) {
        Modifiers.Add(modifier);
    }

    public void UpdateModifier(string modifierName, float newValue) {
        for(int i = 0; i < Modifiers.Count; i++) {
            Modifier m = Modifiers[i];
            if (Modifiers[i].Name.Equals(modifierName)) {
                m.Value = newValue;
                Modifiers[i] = m;
            }
        }
    }

    public void UpdateModifier(string modifierName, Modifier newModifier) {
        for (int i = 0; i < Modifiers.Count; i++) {
            Modifier m = Modifiers[i];
            if (Modifiers[i].Name.Equals(modifierName)) {
                Modifiers[i] = newModifier;
            }
        }
    }

    public virtual void ChangeTotalValueBy(double value) {
        _totalValue += value;
    }

    public void NewTotalValue(double value) {
        _totalValue = value;
    }
    #endregion

    public void LoadFromSaveData(SaveData.StatData statData) {
        _totalValue = statData.totalValue;
        _modifiers = statData.modifiers;
    }
}