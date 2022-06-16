using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ProgressMode { WaveProgression, TimeBetweenWaves }
public class InvasionProgress : MonoBehaviour
{
    ProgressMode progressMode;
    public Slider slider;
    public TextMeshProUGUI percentageText;
    Camp currentCamp;

    bool active;
    float timer;

    Vector3 targetSize;
    Vector3 sizeVel;
    public float sizeSmoothTime;
    float Progress { get 
    {
            if (currentCamp != null) return currentCamp.waves[currentCamp.currentWave - 1].Progress;
            return 0;
    } }

    // Update is called once per frame
    void Update()
    {
        if (currentCamp) SetProgress();

        transform.localScale = Vector3.SmoothDamp(transform.localScale,targetSize,ref sizeVel, sizeSmoothTime);

        if (timer <= 0) SwitchProgressMode(ProgressMode.WaveProgression);
    }


    void SetProgress()
    {
        switch (progressMode)
        {
            case ProgressMode.WaveProgression:

                percentageText.text = Mathf.RoundToInt(Progress * 100).ToString() + " %";
                slider.value = Progress;
                slider.maxValue = 1;
                break;

            case ProgressMode.TimeBetweenWaves:
                percentageText.text = Mathf.RoundToInt(timer).ToString();
                timer -= Time.deltaTime;
                slider.value = timer;
                break;
        }
    }

    public void SwitchProgressMode(ProgressMode mode)
    {
        progressMode = mode;
    }

    public void SetTimer(float time)
    {
        slider.maxValue = time;
        timer = time;
        SwitchProgressMode(ProgressMode.TimeBetweenWaves);
    }

    void OnActivate() 
    {
        targetSize = Vector3.one;
    }

    void OnDeactivate()
    {
        targetSize = Vector3.zero;
    }
    public void AssignCamp(Camp current)
    {
        currentCamp = current;

        if (current != null && !active) 
        {
            active = true;
            OnActivate();
        }
        else if(current == null && active)
        {
            active = false;
            OnDeactivate();
        }

    }
}
