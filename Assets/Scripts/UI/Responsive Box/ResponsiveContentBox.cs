using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class ResponsiveContentBox : MonoBehaviour
{
    #region UI Components
    [Header("DynamicContentBox UI Components")]
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI descriptionText;
    #endregion

    #region Game Object Components
    public LayoutElement layoutElement;
    public RectTransform rectTransform;
    #endregion

    #region Size Fields
    public int characterWrapLimit = 80;
    #endregion

    public virtual void Awake() {
        layoutElement = GetComponent<LayoutElement>();
        rectTransform = GetComponent<RectTransform>();
    }

    #region Set Content
    public void SetText(string description = "", string header = "") {
        if (string.IsNullOrEmpty(description))
            descriptionText.gameObject.SetActive(false);
        else {
            descriptionText.gameObject.SetActive(true);
            descriptionText.text = description;
        }

        if (string.IsNullOrEmpty(header))
            headerText.gameObject.SetActive(false);
        else {
            headerText.gameObject.SetActive(true);
            headerText.text = header;
        }

        int headerLength = headerText.text.Length;
        int descriptionLength = descriptionText.text.Length;

        layoutElement.enabled = (headerLength > characterWrapLimit || descriptionLength > characterWrapLimit) ? true : false;
    }
    #endregion
}
