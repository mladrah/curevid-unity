using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WPMF;

public class ListItem : MonoBehaviour
{
    public new TextMeshProUGUI name;
    public TextMeshProUGUI count;
    public TextMeshProUGUI num;
    public GameObject body;
    public Image shrinkIcon;
    public Image expandIcon;
    public Button flyBtn;
    public Button headerButton;

    private void Start() {
        headerButton.onClick.AddListener(OnHeader);
        flyBtn.onClick.AddListener(OnFly);

        if (!HasChilds()) {
            shrinkIcon.gameObject.SetActive(false);
            expandIcon.gameObject.SetActive(false);
            count.gameObject.SetActive(false);
            flyBtn.gameObject.SetActive(true);
        } else {
            flyBtn.gameObject.SetActive(false);
        }
    }

    public void OnHeader() {
        if (HasChilds()) {
            shrinkIcon.gameObject.SetActive(shrinkIcon.gameObject.activeInHierarchy ? false : true);
            expandIcon.gameObject.SetActive(expandIcon.gameObject.activeInHierarchy ? false : true);
            body.SetActive(body.activeInHierarchy ? false : true);
        } else {
            CountryCanvas.instance.ShowCountryPanel(WorldMap2D.instance.GetCountryIndex(name.text), -1);
        }
    }

    private void OnFly() {
        CountryCanvas.instance.ShowCountryPanel(WorldMap2D.instance.GetCountryIndex(name.text), -1);
        CameraController.Instance.FlyToCountry(name.text);
    }

    private bool HasChilds() {
        return body.transform.childCount != 0;
    }

    public void ChangeColorBy(float value) {
        ColorBlock block = headerButton.colors;
        block.normalColor = new Color(block.normalColor.r + value, block.normalColor.g + value, block.normalColor.b + value);
        headerButton.colors = block;
    }
}
