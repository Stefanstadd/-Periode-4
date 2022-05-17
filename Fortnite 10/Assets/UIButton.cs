using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    public Animator onClickAnimator;

    public void OnUseButton()
    {
        onClickAnimator.SetTrigger("OnClick");
    }
}
