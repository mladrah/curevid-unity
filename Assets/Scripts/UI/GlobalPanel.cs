using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPanel : MonoBehaviour
{
    [SerializeField] private Value currentTab;

    private void OnEnable() {
        if (GlobalCanvas.Instance == null)
            return;

        switch (currentTab) {
            case Value.Infection:
                GlobalCanvas.Instance.UpdateGlobalListUI(currentTab);
                break;
            case Value.Fatality:
                GlobalCanvas.Instance.UpdateGlobalListUI(currentTab);
                break;
            case Value.Vaccination:
                GlobalCanvas.Instance.UpdateGlobalListUI(currentTab);
                break;
            case Value.Recovery:
                GlobalCanvas.Instance.UpdateGlobalListUI(currentTab);
                break;
            case Value.Economy:
                GlobalCanvas.Instance.UpdateGlobalListUI(currentTab);
                break;
            case Value.Happiness:
                GlobalCanvas.Instance.UpdateGlobalListUI(currentTab);
                break;
            case Value.Revolt:
                GlobalCanvas.Instance.UpdateGlobalListUI(currentTab);
                break;
            default:
                Debug.LogError("Current Tab does not match Value | (GlobalPanel.cs : OnEnable())");
                break;
        }
    }
}
