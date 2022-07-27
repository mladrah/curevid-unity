using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Images : MonoBehaviour
{
    private static Images _instance;
    public static Images Instance { get => _instance; }

    #region Icons
    [Header("Icons")]
    public Sprite icon_placeholder;
    public Sprite icon_trait_infection;
    public Sprite icon_measurement_mask;
    public Sprite icon_measurement_lockdown;
    #endregion

    #region Happiness
    [Header("Happiness")]
    public Sprite happiness_neutral;
    public Sprite happiness_angry;
    public Sprite happiness_happy;
    public Sprite revolt_inactive;
    public Sprite revolt_active1;
    public Sprite revolt_active2;
    public Sprite revolt_active3;
    #endregion

    #region Areas
    [Header("Areas")]
    public Sprite area_global;
    public Sprite area_continental;
    public Sprite area_subregional;
    public Sprite area_local;
    #endregion

    private void Awake() {
        _instance = this;
    }
}
