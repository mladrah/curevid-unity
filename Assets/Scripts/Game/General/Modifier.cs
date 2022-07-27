using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Modifier
{
    #region Enums
    public enum effectType
    {
        ADDITIVE,
        MULTIPLICATIVE
    }

    public enum modifierType
    {
        NONE,
        ECONOMY,
        HAPPINESS,
        REVOLT,
        INFECTION,
        VACCINE,
        CURE
    }

    public enum subModifierType
    {
        None,

        Cure_Effectivity,
        Cure_Safety,

        Infection_Socialising,
        Infection_Education,
        Infection_Work,
        Infection_Activities,
        Infection_Gatherings,
        Infection_Revolt,
        Infection_Tourism,
        Infection_Neighboring_Countries,
    }

    public enum valueType
    {
        POSITIVE,
        NEGATIVE
    }
    #endregion

    #region Properties
    [SerializeField] private string _name;
    public string Name { get => _name; }

    [SerializeField] private effectType _effectType;
    public effectType EffectType { get => _effectType; }

    [SerializeField] private modifierType _modifierType;
    public modifierType ModifierType { get => _modifierType; }

    [SerializeField] private subModifierType _subModifierType;
    public subModifierType SubModifierType { get => _subModifierType; }

    [SerializeField] private valueType _valueType;
    public valueType ValueType { get => _valueType; }

    [SerializeField] private float _value;
    public float Value { get => _value; set => _value = value; }
    #endregion

    public Modifier(string name, effectType effectType, modifierType modifierType, valueType valueType, float value, subModifierType subModifierType = subModifierType.None) {
        _name = name;
        _effectType = effectType;
        _valueType = valueType;
        _value = value;
        _modifierType = modifierType;
        _subModifierType = subModifierType;
    }

    //private void SetNameInfectionPostfix() {
    //    switch (_subModifierType) {
    //        case subModifierType.Infection_Social_Distancing:
    //            _name += " SD";
    //            break;
    //        case subModifierType.Infection_Education:
    //            _name += " E";
    //            break;
    //        case subModifierType.Infection_Work:
    //            _name += " W";
    //            break;
    //        case subModifierType.Infection_Activities:
    //            _name += " A";
    //            break;
    //        case subModifierType.Infection_Gatherings:
    //            _name += " G";
    //            break;
    //        case subModifierType.Infection_Revolt:
    //            _name += " R";
    //            break;
    //        case subModifierType.Infection_Neighbor_Countries:
    //            _name += " NC";
    //            break;
    //    }
    //}
}
