using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calculate : MonoBehaviour
{
    public static float Stat(Stat stat) {
        List<Modifier> modifiers = stat.Modifiers;
        float value = 0;

        foreach (Modifier m in modifiers) {
            if (m.EffectType.Equals(Modifier.effectType.ADDITIVE)) {
                value += (m.Value);
                //Debug.Log("Additive Modifier Applied: " + m.Value + " | New Value: " + value);
            }
        }
        foreach (Modifier m in modifiers) {
            if (m.EffectType.Equals(Modifier.effectType.MULTIPLICATIVE)) {
                value = (value * (1 + m.Value));
                //Debug.Log("Multiplicative Modifier Applied: " + m.Value + " | New Value: " + value);
            }
        }

        return value;
    }

    public static float StatMultiplicative(Stat stat) {
        List<Modifier> modifiers = stat.Modifiers;
        float value = 0;

        foreach (Modifier m in modifiers) {
            if (m.EffectType.Equals(Modifier.effectType.MULTIPLICATIVE)) {
                value += m.Value;
                //Debug.Log("Multiplicative Modifier Applied: " + m.Value + " | New Value: " + value);
            }
        }

        return value;
    }
}
