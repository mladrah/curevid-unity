using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Confirmation : ResponsiveContentBox
{
    #region UI Components
    [Header("Concrete UI Components")]
    public TextMeshProUGUI modifierText;
    public Button confirmButton;
    public Button cancelButton;
    #endregion

    #region Behaviour Fields
    public enum Result
    {
        NONE,
        CONFIRM,
        CANCEL
    }
    [System.NonSerialized] public Result result;
    #endregion

    public override void Awake() {
        base.Awake();

        result = Result.NONE;
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(OnCancel);
    }

    #region Set Content
    public void SetText(string header, string description, string modifiers = "") {
        base.SetText(description, header);

        if (string.IsNullOrEmpty(modifiers)) {
            modifierText.gameObject.SetActive(false);
        } else {
            modifierText.gameObject.SetActive(true);
            modifierText.text = modifiers;
        }
    }
    #endregion

    #region Behaviour
    public void Reset() {
        result = Result.NONE;
    }

    public void OnConfirm() {
        result = Result.CONFIRM;
    }

    public void OnCancel() {
        result = Result.CANCEL;
    }
    #endregion
}
