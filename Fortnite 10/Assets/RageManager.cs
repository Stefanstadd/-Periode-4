using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RageManager : MonoBehaviour
{
    public Volume processVolume;


    public float vignetteMin, vignetteMax,abberationMin,abberationMax,temperatureMin,temperatureMax,tintMin,tintMax;
    Vignette vignette;
    ChromaticAberration aberration;
    WhiteBalance whiteBalance;

    [Range(0, 1)] public float rageMeter;

    public float rageSmoothTime;
    public float rageDecrease;

    [Range(0, 1)] public static float targetRage;
    float rageVelocity;

    // Start is called before the first frame update
    void Start()
    {
        processVolume.profile.TryGet(out vignette);
        processVolume.profile.TryGet(out aberration);
        processVolume.profile.TryGet(out whiteBalance);
    }

    // Update is called once per frame
    void Update()
    {
        ApplyRage();
    }

    void ApplyRage()
    {
        rageMeter = Mathf.SmoothDamp(rageMeter,targetRage,ref rageVelocity, rageSmoothTime);

        vignette.intensity.value = Mathf.Lerp(vignetteMin, vignetteMax, rageMeter);

        aberration.intensity.value = Mathf.Lerp(abberationMin,abberationMax,rageMeter);

        whiteBalance.temperature.value = Mathf.Lerp(temperatureMin,temperatureMax,rageMeter);
        whiteBalance.tint.value = Mathf.Lerp(tintMin, tintMax, rageMeter);

        targetRage -= rageDecrease * Time.deltaTime;

        targetRage = Mathf.Clamp01(targetRage);
    }

    public static void AddRage()
    {
        targetRage += 0.1f;
    }
}
