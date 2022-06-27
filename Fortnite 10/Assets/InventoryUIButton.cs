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
    public void Init(BaseWeapon weapon)
    {
        this.weapon = weapon;
        if (weapon.weaponUIImage != null)
        {
            weaponImage.sprite = weapon.weaponUIImage;
            weaponImage.SetNativeSize();
            weaponImage.transform.localScale = new Vector3(0.1135948f, 0.1135948f, 0.1135948f);
        }

    }

    private void Update()
    {
        UpdateColor();
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

        switch (mode)
        {
            case ButtonMode.Normal:
                targetColor = colors.normalColor;
                break;

            case ButtonMode.Highlighted:
                targetColor = colors.highlightedColor;
                break;

            case ButtonMode.Pressed:
                targetColor = colors.pressedColor;
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
