using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    public BaseWeapon weapon;
    public TextMeshProUGUI tier, weaponName, cost, damage, damageNumber, fireRate, fireRateNumber, reload, reloadNumber, magSize, magSizeNumber;
    public Image[] imagesColorChange;
    public TextMeshProUGUI[] textColorChange;
    public bool selected;
    public bool CanAfford { get {return weapon.CanAffordNextUpgrade(ByteManager.bytes); } }
    private void Start()
    {
        if (weapon == null) return;

        SetText(damage, "Damage");
        SetText(fireRate, "Fire Rate");
        SetText(reload, "Reload Time");
        SetText(magSize, "Mag Size");
    }
    private void Update()
    {
        if (weapon == null) return;

        int nextUpgradeTier = weapon.currentUpgrade;
        if(weapon.currentUpgrade == weapon.possibleUpgrades.Length)
        {
            gameObject.SetActive(false);
            return;
        }

        SetText(tier, "Tier " + (nextUpgradeTier+1).ToString());
        SetText(weaponName, weapon.weaponName);
        SetText(cost, weapon.NextUpgradeCost());
        cost.color = CanAfford ? new Color(255, 255, 255, 255) : new Color(255, 0, 0, 255);


        if (nextUpgradeTier > weapon.possibleUpgrades.Length) nextUpgradeTier--;

        SetText(damageNumber, "+ " + weapon.possibleUpgrades[nextUpgradeTier].upDamage);
        SetText(fireRateNumber, "+ " + weapon.possibleUpgrades[nextUpgradeTier].increaseFireRate);
        SetText(reloadNumber, "- " + weapon.possibleUpgrades[nextUpgradeTier].reloadTime);
        SetText(magSizeNumber, "+ " + weapon.possibleUpgrades[nextUpgradeTier].maxMagSize);
    }

    public void OnHover()
    {
        if (selected) return;
        for (int i = 0; i < imagesColorChange.Length; i++)
        {
            imagesColorChange[i].color = new Color32(0, 255, 255, 255);
        }

        for (int i = 0; i < textColorChange.Length; i++)
        {
            textColorChange[i].color = new Color32(0, 255, 255, 255);
        }
    }
    public void OnUnhover()
    {
        if (selected) return;
        for (int i = 0; i < imagesColorChange.Length; i++)
        {
            imagesColorChange[i].color = new Color32(255,255,255,255);
        }

        for (int i = 0; i < textColorChange.Length; i++)
        {
            textColorChange[i].color = new Color32(255, 255, 255, 255);
        }
    }

    public void Select()
    {
        selected = true;
        for (int i = 0; i < imagesColorChange.Length; i++)
        {
            imagesColorChange[i].color = new Color32(0, 255, 0, 255);
        }
        for (int i = 0; i < textColorChange.Length; i++)
        {
            textColorChange[i].color = new Color32(0, 255, 0, 255);
        }
    }

    public void Deselect()
    {
        selected = false;
        for (int i = 0; i < imagesColorChange.Length; i++)
        {
            imagesColorChange[i].color = new Color32(255, 255, 255, 255);
        }
        for (int i = 0; i < textColorChange.Length; i++)
        {
            textColorChange[i].color = new Color32(255, 255, 255, 255);
        }
    }

    void SetText(TMP_Text text, float number)
    {
        SetText(text, number.ToString());
    }
    void SetText(TMP_Text text, int number)
    {
        SetText(text, number.ToString());
    }
    void SetText(TMP_Text text, string _text)
    {
        text.text = _text;
    }
}
