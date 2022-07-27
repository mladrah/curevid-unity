using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPMF;
using SimpleJSON;
using System.Linq;
using System.IO;

public class TestCode : MonoBehaviour
{
    public Material shader;
    private GameObject go;
    private GameObject goClone;
    public WorldMap2D map;
    public float timescale = 0.5f;

    public SpriteRenderer sr;

    void Start() {
        map = WorldMap2D.instance;

        GlobalManager.Instance.allCountriesInitializedEvent += test;
    }

    void test() {
    }

    private void Update() {
    }
}
