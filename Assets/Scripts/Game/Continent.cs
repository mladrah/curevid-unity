using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Continent 
{   
    [SerializeField]
    private readonly string _name;
    public string Name { get => _name; }

    [SerializeField]
    private readonly Dictionary<string, Subregion> _subregions;
    public Dictionary<string, Subregion> Subregions { get => _subregions; }

    public Continent(string name) {
        _name = name;
        _subregions = new Dictionary<string, Subregion>();
    }
}
