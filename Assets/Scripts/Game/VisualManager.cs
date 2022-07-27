using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPMF;

public class VisualManager : MonoBehaviour
{
    private WorldMap2D map;
    private GameObject frontiers;
    private static VisualManager _instance;
    public static VisualManager Instance { get => _instance; }
    [SerializeField] private List<GameObject> instaniatedCountries;

    [Header("Focused Country")]
    [SerializeField] private Material outlineShaderSelected;
    private Material outlineShaderStandard;
    private int formerCountryIndex = -1;

    private void Awake() {
        _instance = this;
    }

    private void Start() {
        map = WorldMap2D.instance;
        instaniatedCountries = new List<GameObject>();

        SetUp();

        //map.OnCountryClick += HighlightCountryOutline;
    }

    private void SetUp() {
        GameObject countryParent = new GameObject();
        countryParent.name = "Countries";
        bool shaderAssigned = false;

        foreach (Transform child in map.transform) {
            if (child.name.Equals("Frontiers"))
                frontiers = child.gameObject;
        }

        for (int i = 0; i < map.countries.Length; i++) {
            Country country = map.countries[i];
            GameObject regionParent = new GameObject();
            regionParent.name = country.name;
            regionParent.transform.SetParent(countryParent.transform);

            foreach (Region r in country.regions) {
                GameObject regionGameObject = map.ToggleCountryRegionSurface(i, r.regionIndex, true, Color.black);
                GameObject regionInstance = Instantiate(regionGameObject);
                regionInstance.name = "Region #" + r.regionIndex;
                regionInstance.transform.localScale = new Vector2(200, 100);
                regionInstance.transform.SetParent(regionParent.transform);

                if (!shaderAssigned) {
                    outlineShaderStandard = regionGameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material;
                    shaderAssigned = true;
                }
            }

            instaniatedCountries.Add(regionParent);
        }

        frontiers.SetActive(false);
    }

    public void HighlightCountryOutline(int countryIndex) {
        if (formerCountryIndex >= 0) {
            UnhighlightCountryOutline();
        }

        formerCountryIndex = countryIndex;
        foreach (GameObject regionParent in instaniatedCountries) {
            if (regionParent.name.Equals(map.countries[countryIndex].name)) {
                formerCountryIndex = regionParent.transform.GetSiblingIndex();
                foreach (Transform region in regionParent.transform) {
                    region.GetChild(0).GetComponent<MeshRenderer>().material = outlineShaderSelected;
                }
            }
        }
    }

    public void UnhighlightCountryOutline() {
        foreach(Transform region in instaniatedCountries[formerCountryIndex].transform) {
            region.GetChild(0).GetComponent<MeshRenderer>().material = outlineShaderStandard;
        }
    }
}
