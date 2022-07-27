using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class SaveFilePrefab : MonoBehaviour, IPointerClickHandler
{
    public Image panel;
    public TextMeshProUGUI saveName;
    public TextMeshProUGUI ingameTime;
    public Button deleteBtn;

    #region Delegate
    public delegate void SaveFilePrefabDelegate(SaveFilePrefab saveFilePrefab);
    public event SaveFilePrefabDelegate OnClickEvent;
    #endregion

    private void Start() {
        deleteBtn.onClick.AddListener(Delete);
    }

    public void OnPointerClick(PointerEventData eventData) {
        OnClickEvent(this);
    }

    public void Focus() {
        panel.DOColor(Colors.DEFAULT_GREEN, 0.5f).SetEase(Ease.OutExpo);
    }

    public void Unfocus() {
        panel.DOColor(Colors.BLUE_1, 0.5f).SetEase(Ease.OutExpo);
    }

    public void Delete() {
        SerialisationCanvas.Instance.Delete(saveName.text);
    }
}
