using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using WPMF;

public class DataManager : MonoBehaviour
{

    #region API Endpoints
    private const string REST_COUNTRIES_URL = "https://restcountries.eu/rest/v2/";

    private const string COVID19_API_URL = "https://api.covid19api.com/";
    #endregion

    #region Data Paths
    public static string COUNTRY_DATA_PATH = "C:/Users/rahmi/Desktop/CountryData.json";

    public static string COVID_SUMMARY_DATA_PATH = "C:/Users/rahmi/Desktop/Covid19Data/summary/";

    public static string COVID_DAYONE_DATA_PATH = "C:/Users/rahmi/Desktop/Covid19Data/dayone/";
    #endregion

    #region Bools
    [Header("Endpoint restcountries")]
    [SerializeField] private bool downloadRestCountriesData = false;

    [Header("Endpoint covid19api")]
    [SerializeField] private bool downloadCovid19SummaryAPIData = false;
    [SerializeField] private bool downloadCovid19DayOneAPIData = false;
    #endregion

    private void Start() {
        if (downloadRestCountriesData) {
            StartCoroutine(SaveRestCountriesAPIData());
        }

        if (downloadCovid19SummaryAPIData) {
            StartCoroutine(SaveCovid19APIData(1, COVID_SUMMARY_DATA_PATH));
        } else if (downloadCovid19DayOneAPIData) {
            StartCoroutine(SaveCovid19APIData(1, COVID_DAYONE_DATA_PATH));
        }
    }

    #region API Country Data Download
    private IEnumerator SaveRestCountriesAPIData() {
        string countryURL = REST_COUNTRIES_URL + "all";

        UnityWebRequest req = UnityWebRequest.Get(countryURL);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.ConnectionError)
            Debug.LogError("Failed to retrieve Data");

        System.IO.File.WriteAllText(COUNTRY_DATA_PATH, req.downloadHandler.text);

        Debug.Log("Country Data successfully downloaded! Path: " + COUNTRY_DATA_PATH);
    }

    private IEnumerator SaveCovid19APIData(float time, string PATH) {
        for (int i = 0; i < WorldMap2D.instance.countries.Length; i++) {
            yield return new WaitForSeconds(time);
            StartCoroutine(SaveCovid19APIDataRequest(WorldMap2D.instance.countries[i].name, PATH, "2019-12-01", "2021-04-01"));
        }
        Debug.Log("Covid Data succesfully downloaded!");
    }

    private IEnumerator SaveCovid19APIDataRequest(string countryName, string PATH, string from = null, string to = null) {
        string covidURL = "";
        if (downloadCovid19SummaryAPIData)
            covidURL = COVID19_API_URL + "total/country/" + countryName.Replace(" ", "-") + "/status/confirmed?from=" + from + "T00:00:00Z&to=" + to + "T00:00:00Z";
        if (downloadCovid19DayOneAPIData)
            covidURL = COVID19_API_URL + "dayone/country/" + countryName.Replace(" ", "-") + "/status/confirmed";

        UnityWebRequest req = UnityWebRequest.Get(covidURL);

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.ConnectionError)
            Debug.LogError("Failed to retrieve Data");

        System.IO.File.WriteAllText(PATH + countryName + ".json", req.downloadHandler.text);

        Debug.Log(covidURL);
    }
    #endregion

    #region Public Data Retrieve
    public static JSONNode GetCountryData(string countryName) {
        //string jsonContent = System.IO.File.ReadAllText(COUNTRY_DATA_PATH);
        string jsonContent = Resources.Load<TextAsset>("CountryData").text;

        JSONNode countryData = JSON.Parse(jsonContent);

        for (int i = 0; i < countryData.Count; i++) {
            if (countryData[i]["name"].Equals(countryName))
                return countryData[i];
        }

        return null;
    }

    public static string[] GetCountryNames() {
        string jsonContent = System.IO.File.ReadAllText(COUNTRY_DATA_PATH);

        JSONNode countryData = JSON.Parse(jsonContent);

        string[] countries = new string[countryData.Count];

        for (int i = 0; i < countryData.Count; i++) {
            countries[i] = countryData[i]["name"];
        }

        return countries;
    }

    public static JSONNode GetCountryCovidSummaryData(string countryName) {
        //string jsonContent = System.IO.File.ReadAllText(COVID_SUMMARY_DATA_PATH + countryName + ".json");
        string jsonContent = Resources.Load<TextAsset>("Covid19Data/summary/" + countryName).text;

        JSONNode countryData = JSON.Parse(jsonContent);

        return countryData;
    }

    public static JSONNode GetCountryCovidDayOneData(string countryName) {
        //string jsonContent = System.IO.File.ReadAllText(COVID_DAYONE_DATA_PATH + countryName + ".json");
        string jsonContent = Resources.Load<TextAsset>("Covid19Data/dayone/" + countryName).text;

        JSONNode countryData = JSON.Parse(jsonContent);

        return countryData;
    }
    #endregion
}
