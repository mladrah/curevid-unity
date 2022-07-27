using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using WPMF;

public enum Area
{
    Global,
    Continental,
    Subregional,
    Local
}

[Serializable]
public struct Date
{
    public int day;
    public int month;
    public int year;

    public override string ToString() {
        return day + " " + month + " " + year;
    }
}

[Serializable]
public struct Option
{
    public string description;
    public List<Modifier> modifiers;
}

[CreateAssetMenu(fileName = "New Event", menuName = "Event")]
public class EventSO : ScriptableObject
{
    [SerializeField]
    private string _title;
    public string Title { get => _title; }

    [SerializeField]
    [TextArea(3, 10)]
    private string _description;
    public string Description { get => _description; }

    [SerializeField]
    private int _duration;
    public int Duration { get => _duration; }

    [SerializeField]
    private List<Option> _options;
    public List<Option> Options { get => _options; }

    [SerializeField]
    private Area _area;
    public Area Area { get => _area; }

    [HideInInspector]
    public List<string> AreaNames;

    [SerializeField]
    private Date _date;
    public Date Date { get => _date; }
}

