using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tab : MonoBehaviour, IPointerClickHandler, IPointerExitHandler
{
    #region Tab Parent
    [Header("Tab Parent")]
    public TabList tabList;
    #endregion

    #region Game Object Components
    [System.NonSerialized] public Image background;
    [System.NonSerialized] public Image icon;
    #endregion

    private void Awake() {
        background = GetComponent<Image>();
        icon = transform.GetChild(0).GetComponent<Image>();

        tabList.Subscribe(this);
    }

    #region Events
    public void OnPointerClick(PointerEventData eventData) {
        tabList.OnTabSelected(this);
    }

    public void OnPointerExit(PointerEventData eventData) {
        tabList.OnTabExit();
    }
    #endregion
}
