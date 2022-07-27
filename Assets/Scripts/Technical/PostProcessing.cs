using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessing : MonoBehaviour
{
    public static PostProcessing instance;

    [NonSerialized] public Volume volume;
    [NonSerialized] public Vignette vignette;

    private void Awake() {
        instance = this;

        volume = GetComponent<Volume>();
        volume.profile.TryGet(out vignette);
    }
}
