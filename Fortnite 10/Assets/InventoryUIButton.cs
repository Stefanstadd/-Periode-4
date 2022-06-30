using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ButtonMode
{
    Normal,
    Highlighted,
    Pressed,
    Selected,
    Disabled
}

public class InventoryUIButton : MonoBehaviour
{
    public BaseWeapon weapon;

    public Image weaponImage;
    public Image border;
    public ColorBlock colors;

    Color targetColor;
    public float colorSmoothTime = 5;
    float smoothTimer;
    bool changed;

    //default 0.6 hovered 0.8

    Vector3 defaultSize;
    Vector3 targetSize;
    public float smoothSpeed = 0.3f;
    private Vector3 scaleVelocity;

    bool hovered;
    public bool selected;
    public void Init(BaseWeapon weapon)
    {
        this.weapon = weapon;
        if (weapon.weaponUIImage != null)
        {
            weaponImage.sprite = weapon.weaponUIImage;
            weaponImage.SetNativeSize();
            weaponImage.transform.localScale = new Vector3(0.1135948f, 0.1135948f, 0.1135948f);
        }

        defaultSize = transform.localScale;
    }

    private void Update()
    {
        UpdateColor();
        UpdateButtonSize();
    }

    void UpdateButtonSize()
    {
        transform.localScale = Vector3.SmoothDamp(transform.localScale,targetSize, ref scaleVelocity, smoothSpeed);

        if (hovered)
            targetSize = new Vector3(0.8f, 0.8f, 0.8f);
        else
            targetSize = defaultSize;
    }

    void UpdateColor()
    {
        border.color = /*Color.Lerp(weaponImage.color,targetColor,smoothTimer)*/ targetColor;

        if (smoothTimer <= 1) 
            smoothTimer += Time.deltaTime * colorSmoothTime;

        if (changed)
        {
            changed = false;
            smoothTimer = 0;
        }
    }

    public void SetColor(ButtonMode mode)
    {
        hovered = false;
        switch (mode)
        {
            case ButtonMode.Normal:
                targetColor = colors.normalColor;
                break;

            case ButtonMode.Highlighted:
                targetColor = colors.highlightedColor;
                hovered = true;
                break;

            case ButtonMode.Pressed:
                targetColor = colors.pressedColor;
                selected = true;
                break;

            case ButtonMode.Selected:
                targetColor = colors.selectedColor;
                break;

            case ButtonMode.Disabled:
                targetColor = colors.disabledColor;
                break;

            default:
                break;
        }
    }
}
