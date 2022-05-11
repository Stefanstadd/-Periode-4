using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    public Transform target;

    public float upSpeed = 1;

    float startLifeTime;
    float lifeTime;
    float upTimer;
    bool initialized;
    Vector3 scale;
    public AnimationCurve scaleCurve;
    public void Initialize(float lifeTime, Transform target, float upTimer, Vector3 scale)
    {
        startLifeTime = lifeTime;
        this.lifeTime = lifeTime;
        this.target = target;
        this.scale = scale;
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized)
        {
            upTimer -= Time.deltaTime;

            if(upTimer < 0)
            {
                transform.Translate(Vector3.up * upSpeed * Time.deltaTime, Space.World);
            }
            lifeTime -= Time.deltaTime;
            if(lifeTime < 0)
            {
                Destroy(gameObject);
            }

            transform.LookAt(target.position);
            transform.localScale = scale * scaleCurve.Evaluate(Mathf.InverseLerp(startLifeTime,0,lifeTime)) * Vector3.Distance(transform.position, target.position) * 0.05f;
        }

        
    }
}
