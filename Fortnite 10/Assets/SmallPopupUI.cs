using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class SmallPopupUI : MonoBehaviour
{
    const string active = "Active";

    public TextMeshProUGUI text;
    public bool popup;
    public float popupTime;
    public float speed;

    [Space (20)]
    public Volume volume;
    Vignette vignette;
    public float strengthSmoothTime, strengthWaitTime;
    public float popupStrength;
    float defaultStrength;
    float targetStrength;
    float stengthVel;

    public bool Active { get { return animator.GetBool(active); } }


    bool canChangeVignette;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        volume.profile.TryGet(out vignette);
        defaultStrength = vignette.intensity.value;
        animator = GetComponent<Animator>();
        targetStrength = defaultStrength;

        gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (popup)
        {
            popup = false;
            Popup("Test");
        }

        if(canChangeVignette)
            vignette.intensity.value = Mathf.SmoothDamp(vignette.intensity.value, targetStrength, ref stengthVel, strengthSmoothTime);
    }

    public async void Popup(string text)
    {
        if (Active) return;
        canChangeVignette = true;
        animator.SetFloat("Speed", speed);

        this.text.text = text;

        animator.SetBool(active,true);

        targetStrength = popupStrength;

        await System.Threading.Tasks.Task.Delay((int)(popupTime * 1000));

        animator.SetBool(active, false);

        await System.Threading.Tasks.Task.Delay((int)(strengthWaitTime * 1000));

        targetStrength = defaultStrength;
        await System.Threading.Tasks.Task.Delay(1500);
        canChangeVignette = false;
    }
}
