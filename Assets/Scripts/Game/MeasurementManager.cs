using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasurementManager : MonoBehaviour
{
    private static MeasurementManager _instance;
    public static MeasurementManager Instance { get => _instance; }

    public List<MeasurementSO> generalMeasurements;

    private void Awake() {
        _instance = this;
    }
}
