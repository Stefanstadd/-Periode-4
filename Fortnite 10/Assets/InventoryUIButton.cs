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
    public Image weaponImage;
    public ColorBlock colors;
    public void Init(BaseWeapon weapon)
    {
        if (weapon.weaponUIImage != null)
        {
            weaponImage.sprite = weapon.weaponUIImage;
            weaponImage.SetNativeSize();
            weaponImage.transform.localScale = new Vector3(0.1135948f, 0.1135948f, 0.1135948f);
        }

    }

    void UpdateColors(ButtonMode mode)
    {
        switch (mode)
        {
            default:
        }
    }
}
