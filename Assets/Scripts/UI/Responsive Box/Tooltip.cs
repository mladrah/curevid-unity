using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : ResponsiveContentBox
{
    #region UI Components
    [Header("Concrete UI Components")]
    public TextMeshProUGUI durationText;
    public TextMeshProUGUI modifierText;
    #endregion

    #region Game Object Components
    #endregion

    #region Position Fields
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;
    #endregion

    #region Behaviour Fields
    [Header("Behaviour")]
    public bool isVisible;
    #endregion

    public override void Awake() {
        base.Awake();
    }

    public void Update() {
        TrackMouse();
    }

    #region Set Content
    public void SetText(string description = "", string header = "", string modifiers = "", string duration = "") {
        base.SetText(description, header);
 
        if (string.IsNullOrEmpty(modifiers)) {
            modifierText.gameObject.SetActive(false);
        } else {
            modifierText.gameObject.SetActive(true);
            modifierText.text = modifiers;
        }

        if (string.IsNullOrEmpty(duration)) {
            durationText.gameObject.SetActive(false);
        } else {
            durationText.gameObject.SetActive(true);
            durationText.text = duration;
        }
    }
    #endregion

    #region Behaviour
    public void TrackMouse() {
        if (!isVisible)
            return;

        if (!UIManager.Instance.mouseOverUI) {
            TooltipManager.Hide();
            return;
        }

        Vector2 position = Input.mousePosition;

        float pivotX = 1f;
        float pivotY = 0f;

        // x -121
        // y -125
        // pivotx/y = -0.05f

        #region West Border
        if ((position.x - rectTransform.rect.width * 1.5f) <= 0) {
            position.x = Screen.width + (rectTransform.rect.width * (Screen.width / TechnicalManager.CANVAS_SCALER_X));
        }
        #endregion

        #region North Border
        if ((position.y + rectTransform.rect.height) >= Screen.height) {
            position.y = Screen.height - (rectTransform.rect.height * (Screen.height / TechnicalManager.CANVAS_SCALER_Y));
        }
        #endregion

        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = position;
    }
    #endregion
}
