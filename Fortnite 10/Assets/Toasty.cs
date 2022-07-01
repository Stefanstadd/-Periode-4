using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toasty : MonoBehaviour
{
    static Animator animator;
    static AudioSource audioSource;
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public static void Toast()
    {
        animator.SetTrigger("Toast");
        audioSource.Play();
    }
}
