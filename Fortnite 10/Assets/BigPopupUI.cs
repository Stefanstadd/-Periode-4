using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class BigPopupUI : MonoBehaviour
{
    const string active = "Active";

    Animator animator;
    public TextMeshProUGUI mainText, secondaryText;
    public float actionTime;
    public bool popup;
    public float speed;

    [Space(20)]
    public Volume volume;
    Vignette vignette;
    public float strengthSmoothTime, strengthWaitTime;
    public float popupStrength;
    float defaultStrength;
    float targetStrength;
    float stengthVel;
    public bool Active { get { return animator.GetBool(active); } }
    void Start()
    {
        volume.profile.TryGet(out vignette);
        defaultStrength = vignette.intensity.value;
        animator = GetComponent<Animator>();
        targetStrength = defaultStrength;
    }

    private void Update()
    {
        if (popup)
        {
            popup = false;
            Popup("Test","q4waetewt");
        }

        vignette.intensity.value = Mathf.SmoothDamp(vignette.intensity.value, targetStrength, ref stengthVel, strengthSmoothTime);
    }

    public async void Popup(string mainText,string secondaryText)
    {
        animator.SetFloat("Speed", speed);

        if (Active) return;

        this.mainText.text = mainText;
        this.secondaryText.text = secondaryText;

        animator.SetBool(active, true);

        targetStrength = popupStrength;

        await Task.Delay((int)(actionTime * 1000));
        animator.SetBool(active, false);

        await Task.Delay((int)(strengthWaitTime * 1000));
        targetStrength = defaultStrength;
    }
}
