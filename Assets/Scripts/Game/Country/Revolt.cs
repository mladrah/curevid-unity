using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Revolt : Stat
{
    #region Internal
    private float _phase1Value;
    private float _phase2Value;
    private float _phase3Value;
    #endregion

    #region Properties
    #region Bools
    private bool _isAbove1;
    public bool IsAbove1 { get => _isAbove1; }

    private bool _isAbove2;
    public bool IsAbove2 { get => _isAbove2; }

    private bool _isAbove3;
    public bool IsAbove3 { get => _isAbove3; }

    public bool Is1Active { get; set; }
    public bool Is2Active { get; set; }
    public bool Is3Active { get; set; }
    #endregion

    #region Traits
    private readonly Trait _traitPhase1;
    public Trait TraitPhase1 {
        get {
            Is1Active = !Is1Active;
            return _traitPhase1;
        }
    }

    private readonly Trait _traitPhase2;
    public Trait TraitPhase2 {
        get {
            Is2Active = !Is2Active;
            return _traitPhase2;
        }
    }

    private readonly Trait _traitPhase3;
    public Trait TraitPhase3 {
        get {
            Is3Active = !Is3Active;
            return _traitPhase3;
        }
    }
    #endregion
    #endregion

    public Revolt() : base() {
        _phase1Value = 10;
        _phase2Value = 50;
        _phase3Value = 90;

        List<Modifier> modifiers = new List<Modifier>();
        modifiers.Add(new Modifier("Infection (R)", Modifier.effectType.ADDITIVE, Modifier.modifierType.INFECTION, Modifier.valueType.NEGATIVE, 0.4f, Modifier.subModifierType.Infection_Revolt));
        _traitPhase1 = new Trait("Revolt Phase 1", "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.", -1, Images.Instance.revolt_active1, modifiers);

        modifiers = new List<Modifier>();
        modifiers.Add(new Modifier("Infection (R)", Modifier.effectType.ADDITIVE, Modifier.modifierType.INFECTION, Modifier.valueType.NEGATIVE, 0.6f, Modifier.subModifierType.Infection_Revolt));
        _traitPhase2 = new Trait("Revolt Phase 2", "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.", -1, Images.Instance.revolt_active1, modifiers);

        modifiers = new List<Modifier>();
        modifiers.Add(new Modifier("Infection (R)", Modifier.effectType.MULTIPLICATIVE, Modifier.modifierType.INFECTION, Modifier.valueType.NEGATIVE, 0.75f, Modifier.subModifierType.Infection_Revolt));
        modifiers.Add(new Modifier("Cure Coins", Modifier.effectType.MULTIPLICATIVE, Modifier.modifierType.ECONOMY, Modifier.valueType.NEGATIVE, -0.3f));
        _traitPhase3 = new Trait("Revolt Phase 3", "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.", -1, Images.Instance.revolt_active1, modifiers);

        //List<Modifier> modifiers = new List<Modifier>();
        //modifiers.Add(new Modifier("Infection (R)", Modifier.effectType.ADDITIVE, Modifier.modifierType.INFECTION, Modifier.valueType.NEGATIVE, 0.2f, Modifier.subModifierType.Infection_Revolt));
        //_traitPhase1 = new Trait("Revolt Phase 1", "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.", -1, Images.Instance.revolt_active1, modifiers);

        //modifiers = new List<Modifier>();
        //modifiers.Add(new Modifier("Infection (R)", Modifier.effectType.ADDITIVE, Modifier.modifierType.INFECTION, Modifier.valueType.NEGATIVE, 0.4f, Modifier.subModifierType.Infection_Revolt));
        //_traitPhase2 = new Trait("Revolt Phase 2", "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.", -1, Images.Instance.revolt_active1, modifiers);

        //modifiers = new List<Modifier>();
        //modifiers.Add(new Modifier("Infection (R)", Modifier.effectType.MULTIPLICATIVE, Modifier.modifierType.INFECTION, Modifier.valueType.NEGATIVE, 0.5f, Modifier.subModifierType.Infection_Revolt));
        //modifiers.Add(new Modifier("Cure Coins", Modifier.effectType.MULTIPLICATIVE, Modifier.modifierType.ECONOMY, Modifier.valueType.NEGATIVE, -0.3f));
        //_traitPhase3 = new Trait("Revolt Phase 3", "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.", -1, Images.Instance.revolt_active1, modifiers);


    }

    #region Methods
    public override void ChangeTotalValueBy(double value) {
        base.ChangeTotalValueBy(value);

        _isAbove1 = TotalValue >= _phase1Value ? true : false;
        _isAbove2 = TotalValue >= _phase2Value ? true : false;
        _isAbove3 = TotalValue >= _phase3Value ? true : false;
    }
    #endregion

    #region Serialisation
    public void LoadFromSaveData(SaveData.CountryRevoltData revoltData) {
        base.LoadFromSaveData(revoltData.baseStat);

        _isAbove1 = revoltData.isAbove1;
        _isAbove2 = revoltData.isAbove2;
        _isAbove3 = revoltData.isAbove3;

        Is1Active = revoltData.is1Active;
        Is2Active = revoltData.is2Active;
        Is3Active = revoltData.is3Active;
    }
    #endregion
}
