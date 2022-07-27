using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabList : MonoBehaviour
{
    /*
       Hierachy Order of Tab must match Hierachy Order of Panel
    */

    #region Tab Buttons
    [Header("Tab Buttons")]
    public List<Tab> tabList;
    private Tab selectedTab;
    #endregion

    #region Tab Panels
    [Header("Tab Panels")]
    public List<GameObject> panelList;
    #endregion

    private void Start() {
        InitState();
    }

    #region Events
    public void OnTabSelected(Tab tab) {
        selectedTab = tab;
        ResetTabs();
        TabSelected(tab);
        SelectPanel(tab);
    }

    public void OnTabExit() {
        ResetTabs();
    }
    #endregion

    #region Tab State
    public void InitState() {
        foreach (Tab tab in tabList) {
            if (tab.transform.GetSiblingIndex() == 0)
                selectedTab = tab;
        }

        if (selectedTab == null)
            Debug.LogError("Selected Tab is Null");

        ResetTabs();
        TabSelected(selectedTab);
        SelectPanel(selectedTab);
    }

    private void ResetTabs() {
        foreach (Tab tab in tabList) {
            if (selectedTab != null && selectedTab == tab) { continue; }
            TabIdle(tab);
        }
    }

    private void TabIdle(Tab tab) {
        tab.background.color = Colors.BLUE_DARK;
        tab.icon.color = Colors.WHITE;
    }

    private void TabSelected(Tab tab) {
        tab.background.color = Colors.BLUE_MEDIUM;
        tab.icon.color = Colors.WHITE_DARK;
    }
    #endregion

    #region Behaviour
    public void Subscribe(Tab tab) {
        if (tabList == null)
            tabList = new List<Tab>();

        tabList.Add(tab);
    }

    private void SelectPanel(Tab tab) {
        int index = tab.transform.GetSiblingIndex();
        for (int i = 0; i < panelList.Count; i++) {
            if (i == index) {
                panelList[i].SetActive(true);
                //panelList[i].transform.SetAsLastSibling();
            }
            else
                panelList[i].SetActive(false);
        }
    }
    #endregion
}
