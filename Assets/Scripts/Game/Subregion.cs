using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPMF;

[System.Serializable]
public class Subregion
{
    private readonly string _name;
    public string Name { get => _name; }

    private readonly Continent _continent;
    public Continent Continent { get => _continent; }

    private Dictionary<string, Country> _countries;
    public Dictionary<string, Country> Countries { get => _countries; }

    public Subregion(string name, Continent continent) {
        _name = name;
        _continent = continent;
        _countries = new Dictionary<string, Country>();
    }
}
