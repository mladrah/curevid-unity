using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Measurement", menuName = "Measurement")]
public class MeasurementSO : ScriptableObject
{
    [SerializeField] 
    private string _name;
    public string Name { get => _name; }
    
    [SerializeField] 
    [TextArea(3,10)]
    private string _description;
    public string Description { get => _description; }

    [SerializeField] 
    private int _activeDuration;
    public int ActiveDuration { get => _activeDuration; }

    [SerializeField] 
    private Sprite _icon;
    public Sprite Icon { get => _icon; }

    [SerializeField] 
    private List<Modifier> _modifiers;
    public List<Modifier> Modifiers { get => _modifiers; }

    [SerializeField]
    private List<MeasurementSO> _family;
    public List<MeasurementSO> Family { get => _family; }

    [SerializeField]
    private string _familyName;
    public string FamilyName { get => _familyName; }
}
