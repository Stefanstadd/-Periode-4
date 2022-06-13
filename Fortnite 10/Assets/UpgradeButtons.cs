using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtons : MonoBehaviour
{
    [Header("Upgrade Buttons")]
    public UpgradeButton[] buttons;
    public Transform upgradeButton;
    public float treshold, upgradeButtonTreshold;
    UpgradeButton previousHovered;
    UpgradeButton selected;
    [Header("Upgrade")]
    public Image[] upgradeImages;
    public Color active, hovered, disabled;
    bool hoveredOverButton;
    void Update()
    {
        HoverOverButtons();
        CheckUpgradeButton();
    }

    private void OnDisable()
    {
        selected = null;
        previousHovered = null;
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].Deselect();
            buttons[i].OnUnhover();
        }
    }

    void HoverOverButtons()
    {
        //Upgrades

        float smallestDst = float.MaxValue;
        UpgradeButton hoveredButton = null;

        for (int i = 0; i < buttons.Length; i++)
        {
            float dst = Vector3.Distance(Input.mousePosition, buttons[i].transform.position);
            if (dst < smallestDst && dst < treshold && !hoveredOverButton)
            {
                smallestDst = dst;
                hoveredButton = buttons[i];
            }
        }

        if (previousHovered == hoveredButton && previousHovered != null)
        {
            previousHovered.OnHover();
            if (Input.GetButtonDown("Fire1"))
            {
                if (selected) selected.Deselect();

                selected = previousHovered;
                previousHovered.Select();
            }
        }
        else
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] == hoveredButton) continue;
                buttons[i].OnUnhover();
            }
        }

        previousHovered = hoveredButton;
    }

    void CheckUpgradeButton()
    {
        bool canUseButton = false;
        Color buttonColor = Color.white;
        //Checking if mouse is over the real Upgrade button

        if (selected && selected.weapon != null && selected.weapon.CanAffordNextUpgrade(ByteManager.bytes))
        {
            buttonColor = active;
            canUseButton = true;
        }
        else
        {
            buttonColor = disabled;
        }

        float dst = Vector3.Distance(Input.mousePosition, upgradeButton.position);
        if (dst < upgradeButtonTreshold && canUseButton)
        {
            hoveredOverButton = true;
            buttonColor = hovered;
            if (Input.GetButtonDown("Fire1"))
            {
                BaseWeapon weapon = selected.weapon;
                if (weapon)
                {
                    ByteManager.bytes -= weapon.NextUpgradeCost();
                    weapon.OnBuyUpgrade();
                }
            }
        }
        else
        {
            hoveredOverButton = false;
        }
        
        //assign color
        for (int i = 0; i < upgradeImages.Length; i++)
        {
            upgradeImages[i].color = buttonColor;
        }
    }
}
