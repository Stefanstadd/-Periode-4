using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlickering : MonoBehaviour
{
    Light target;
    public float min, max;

    [Range(0, 0.25f)] public float time;
    float timer;
    float targetLighting;
    float velocity;
    public float smoothSpeed;

    private void Start()
    {
        target = GetComponent<Light>();
    }
    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            UpdateLight();
            timer = time;
        }

        target.intensity = Mathf.SmoothDamp(target.intensity, targetLighting, ref velocity, smoothSpeed * Time.deltaTime);
    }

    void UpdateLight()
    {
        targetLighting = Random.Range(min, max);
    }
}
