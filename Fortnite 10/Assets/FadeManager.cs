using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeManager : MonoBehaviour
{
    static Animator animator;

    const string fadeAnimationName = "Fade";
    const string fadeSpeedName = "Speed";
    public static float fadeSpeed = 1;
    [SerializeField] float startTime = 1;
    bool faded;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(startTime <= 0 && !faded)
        {
            Fade(true);
            faded = true;
            return;
        }
        startTime -= Time.deltaTime;
    }

    public static void Fade(bool value)
    {
        animator.SetFloat(fadeSpeedName, fadeSpeed);
        animator.SetBool(fadeAnimationName, value);
    }


}
