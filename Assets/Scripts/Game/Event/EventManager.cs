using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Event = Game.Event;

public class EventManager : MonoBehaviour, ISaveable
{
    #region Game Object Fields
    private static EventManager _instance;
    public static EventManager Instance { get => _instance; }
    [Header("Content Box")]
    [SerializeField] private Event _event;
    #endregion

    [SerializeField] private List<EventSO> eventSOList;

    private List<Event> _eventList;
    public List<Event> EventList { get => _eventList; }

    public bool isActive;

    private void Awake() {
        _instance = this;

        _eventList = new List<Event>();

        Copy();
    }

    private void Start() {
        _event.gameObject.SetActive(false);
        TimeManager.Instance.dailyUpdateDelegate += CheckEvent;
        CheckEvent();
    }

    private void Copy() {
        foreach (EventSO eso in eventSOList)
            _eventList.Add(new Game.Event(eso));
    }

    private void CheckEvent() {
        if (isActive) {
            if (_event.VisibleEvent.IsOccured)
                Hide();
        }

        foreach (Event e in _eventList) {
            if (!e.IsOccured) {
                if (e.Date.day == TimeManager.Instance.day && e.Date.month == TimeManager.Instance.month && e.Date.year == TimeManager.Instance.year)
                    Show(e);
            }
        }
    }

    public void Show(Event e) {
        TimeManager.Instance.Pause();
        _event.rectTransform.localPosition = new Vector3(0, 0, 0);
        _event.SetEvent(e);
        _event.gameObject.SetActive(true);
        TimeManager.Instance.CanBeModified = false;
        isActive = true;
    }

    public void Hide() {
        TimeManager.Instance.CanBeModified = true;
        _event.gameObject.SetActive(false);
        TimeManager.Instance.Pause();
        isActive = false;
    }

    public void PopulateSaveData(SaveData saveData) {
        SaveData.EventManagerData emd = new SaveData.EventManagerData(this);

        saveData.eventManagerData = emd;
    }

    public void LoadFromSaveData(SaveData saveData) {
        SaveData.EventManagerData emd = saveData.eventManagerData;

        foreach (SaveData.EventData ed in emd.eventDatas) {
            foreach (Event e in _eventList) {
                if (e.Title.Equals(ed.title)) {
                    e.LoadFromSaveData(ed);
                }
            }
        }

        CheckEvent();
    }
}
