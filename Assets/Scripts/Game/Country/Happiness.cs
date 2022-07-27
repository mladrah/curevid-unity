using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Happiness : Stat
{
    #region Internal
    private float _aboveValue;

    private float _belowValue;
    #endregion

    #region Properties
    #region Bools
    private bool _isBelow;
    public bool IsBelow { get => _isBelow; }

    private bool _isAbove;
    public bool IsAbove { get => _isAbove; }

    private bool _isHappy;
    public bool IsHappy { get => _isHappy; }

    private bool _isAngry;
    public bool IsAngry { get => _isAngry; }
    #endregion

    #region Traits
    private readonly Trait _happyTrait;
    public Trait HappyTrait {
        get {
            _isHappy = !_isHappy;
            return _happyTrait;
        }
    }

    private readonly Trait _angryTrait;
    public Trait AngryTrait {
        get {
            _isAngry = !_isAngry;
            return _angryTrait;
        }
    }
    #endregion
    #endregion

    public Happiness(string name, float value) : base(name, value) {
        _aboveValue = 75;
        _belowValue = 25;

        List<Modifier> modifiers = new List<Modifier>();
        modifiers.Add(new Modifier("Cure Coins", Modifier.effectType.MULTIPLICATIVE, Modifier.modifierType.ECONOMY, Modifier.valueType.POSITIVE, 0.3f));
        modifiers.Add(new Modifier("Revolt", Modifier.effectType.ADDITIVE, Modifier.modifierType.REVOLT, Modifier.valueType.POSITIVE, -1.5f));
        _happyTrait = new Trait("Happy Population", "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.", -1, Images.Instance.happiness_happy, modifiers);

        modifiers = new List<Modifier>();
        modifiers.Add(new Modifier("Revolt", Modifier.effectType.ADDITIVE, Modifier.modifierType.REVOLT, Modifier.valueType.NEGATIVE, 1f));
        _angryTrait = new Trait("Angry Population", "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.", -1, Images.Instance.happiness_angry, modifiers);
    }

    #region Methods
    public override void ChangeTotalValueBy(double value) {
        base.ChangeTotalValueBy(value);

        _isAbove = TotalValue >= _aboveValue ? true : false;
        _isBelow = TotalValue <= _belowValue ? true : false;
    }
    #endregion

    #region Serialisation
    public void LoadFromSaveData(SaveData.CountryHappinessData happinessData) {
        base.LoadFromSaveData(happinessData.baseStat);

        _isAbove = happinessData.isAbove;
        _isBelow = happinessData.isBelow;

        _isHappy = happinessData.isHappy;
        _isAngry = happinessData.isAngry;
    }
    #endregion
}
