using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cure : MonoBehaviour, ISaveable
{
    #region Properties
    private static Cure _instance;
    public static Cure Instance { get => _instance; }

    private string _name;
    public string Name { get => _name; }

    private string _description;
    public string Description { get => _description; }

    private Stat _effectivity;
    public Stat Effectivity { get => _effectivity; }

    private Stat _safety;
    public Stat Safety { get => _safety; }

    private const float _baseRisk = (float)1 / 50;
    #endregion

    private void Awake() {
        _instance = this;
        Init();
    }

    #region Methods
    private void Init() {
        _name = "Curevid";
        _description = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.";
        _effectivity = new Stat("Effectivity");
        _safety = new Stat("Safety");

        _effectivity.AddModifier(new Modifier("Base Value", Modifier.effectType.MULTIPLICATIVE, Modifier.modifierType.CURE, Modifier.valueType.POSITIVE, 0f, Modifier.subModifierType.Cure_Effectivity));
        _safety.AddModifier(new Modifier("Base Value", Modifier.effectType.MULTIPLICATIVE, Modifier.modifierType.CURE, Modifier.valueType.POSITIVE, 0f, Modifier.subModifierType.Cure_Safety));
    }

    public void UpdateStats(Research research) {
        List<Modifier> modifierList = research.Modifiers;
        foreach (Modifier modifier in modifierList) {
            Modifier newModifier = new Modifier(research.name, modifier.EffectType, modifier.ModifierType, modifier.ValueType, modifier.Value, modifier.SubModifierType);
            switch (modifier.SubModifierType) {
                case Modifier.subModifierType.Cure_Effectivity:
                    _effectivity.AddModifier(newModifier);
                    break;
                case Modifier.subModifierType.Cure_Safety:
                    _safety.AddModifier(newModifier);
                    break;
                default:
                    Debug.LogWarning("Could not match Modifier to Cure Stats | (Cure.cs : UpdateStats(...))");
                    break;
            }
        }
    }

    public double SafetyCasualities() {
        return _baseRisk * (1 - Calculate.StatMultiplicative(Safety));
    }

    public void ChangeName(string name) {
        _name = name;
    }
    #endregion

    #region Serialisation
    public void PopulateSaveData(SaveData saveData) {
        SaveData.CureData cd = new SaveData.CureData(this);

        saveData.cureData = cd;
    }

    public void LoadFromSaveData(SaveData saveData) {
        SaveData.CureData cd = saveData.cureData;

        _effectivity.LoadFromSaveData(cd.effectivity);
        _safety.LoadFromSaveData(cd.safety);

        ResearchCanvas.Instance.UpdateCureUI();
    }
    #endregion
}
