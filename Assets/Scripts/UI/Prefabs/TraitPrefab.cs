using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class TraitPrefab : MonoBehaviour
{
    private Trait _reference;

    #region UI Fields
    public Image ImageField;
    private TooltipTrigger ttTrigger;
    #endregion

    private void Awake() {
        ttTrigger = GetComponent<TooltipTrigger>();
    }

    #region Clone
    public void Clone(Trait trait) {
        _reference = trait;

        SetFields();
        SetTooltip();
    }
    #endregion

    #region Set UI Elements
    private void SetFields() {
        ImageField.sprite = _reference.Icon;
    }

    private void SetTooltip() {
        ttTrigger.header = _reference.Name;
        ttTrigger.description = _reference.Description;
        ttTrigger.modifiers = Format.Modifiers(_reference.Modifiers);
        if (_reference.ExpirationDate != DateTime.MinValue)
            ttTrigger.duration = "Expiration: " + _reference.ExpirationDate.Day + "." + _reference.ExpirationDate.Month + "." + _reference.ExpirationDate.Year;
    }
    #endregion
}
