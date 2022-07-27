using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Research", menuName = "Research")]
public class ResearchSO : ScriptableObject
{   
    [SerializeField]
    private string _name;
    public string Name { get => _name; }

    [SerializeField]
    [TextArea(3,10)]
    private string _description;
    public string Description { get => _description; }

    [SerializeField]
    private int _duration;
    public int Duration { get => _duration; }

    [SerializeField]
    private float _cost;
    public float Cost { get => _cost; }

    [SerializeField]
    private float _dailyCost;
    public float DailyCost { get => _dailyCost; }

    [SerializeField]
    private bool _isMandatory;
    public bool IsMandatory { get => _isMandatory; }

    [SerializeField]
    private List<Modifier> _modifiers;
    public List<Modifier> Modifiers { get => _modifiers; }

    [SerializeField]
    private Sprite _icon;
    public Sprite Icon { get => _icon; }
}
