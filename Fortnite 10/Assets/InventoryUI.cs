using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public BaseWeapon[] weapons;
    InventoryUIButton[] uIButtons;

    public Image[] colorChangeables;

    [Space(20)]
    [Header("UI")]
    public Image gunImage;
    public TextMeshProUGUI weaponName, weaponDescription, weaponTier, weaponBullets, weaponUpgrades;

    public int upgradeSpacesAmount;


    [Space(20)]
    [Header("Buttons")]

    public Transform buttonsParent;
    public GameObject buttonPrefab;
    public Vector3 startPos;
    public float minDst;
    public float spaceBtwn;


    InventoryUIButton previousSelected;
    private void Start()
    {
        CreateButtons();

        OnClickButton(uIButtons[0]);
    }


    void CreateButtons()
    {
        uIButtons = new InventoryUIButton[weapons.Length];

        for (int i = 0; i < weapons.Length; i++)
        {
            Vector3 pos = startPos + new Vector3(spaceBtwn * i, 0f, 0f);

            uIButtons[i] = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity, buttonsParent).GetComponent<InventoryUIButton>();
            if (uIButtons[i])
            {
                uIButtons[i].transform.localPosition = pos;
                uIButtons[i].Init(weapons[i]);
            }
        }
    }

    private void Update()
    {
        UpdateButtons();
    }

    void ChangeWeaponDisplay(InventoryUIButton button)
    {
        //Reference to current upgrade and weapon
        var weapon = button.weapon;
        var curWeaponUpgrade = weapon.currentUpgrade == 0? WeaponUpgrade.First : weapon.possibleUpgrades[weapon.currentUpgrade -1];

        bool hasNextUpgrade = weapon.currentUpgrade < weapon.possibleUpgrades.Length;

        //Change colors of the inventory
        foreach (var img in colorChangeables)
        {
            img.color = curWeaponUpgrade.upgradeColor;
        }

        //Set gun image
        gunImage.sprite = weapon.weaponUIImage;

        //Set name and description
        weaponName.text = weapon.weaponName;

        weaponDescription.text = weapon.weaponDescription;

        //Set bullets left
        weaponBullets.text = weapon.ammoLeft.ToString();

        //set next upgrade data

        weaponUpgrades.overrideColorTags = true;
        weaponUpgrades.color = Color.red;
        string upgradeData = " You have no more available upgrades for this weapon";


        if (hasNextUpgrade)
        {
            //Reference to next upgrade
            WeaponUpgrade nextUpgrade = weapon.possibleUpgrades[weapon.currentUpgrade];

            string spaces = "";

            for (int i = 0; i < upgradeSpacesAmount; i++)
            {
                spaces += " ";
            }

            string damage = $"Damage: +  {nextUpgrade.upDamage}";
            string fireRate = $"Fire Rate + {nextUpgrade.increaseFireRate}";
            string reloadTime = $"Reload Time - {nextUpgrade.reloadTime}";
            string maxMagSize = $"Max Magazine Size + {nextUpgrade.maxMagSize}";

            upgradeData = damage + spaces + fireRate + spaces + reloadTime + spaces + maxMagSize;
            
            weaponUpgrades.color = nextUpgrade.upgradeColor;
        }

        weaponUpgrades.text = upgradeData;
    }

    void UpdateButtons()
    {
        InventoryUIButton hoveredButton = null;
        float closestDst = float.MaxValue;

        var mousePos = Input.mousePosition;
        for (int i = 0; i < uIButtons.Length; i++)
        {
            if(uIButtons[i] != previousSelected)
                uIButtons[i].SetColor(ButtonMode.Normal);

            float dst = Vector3.Distance(uIButtons[i].transform.position, mousePos);
            if(dst< closestDst && dst < minDst)
            {
                hoveredButton = uIButtons[i];
                closestDst = dst;
            }
        }

        if (hoveredButton)
        {
            hoveredButton.SetColor(ButtonMode.Highlighted);

            print("a");
            if (Input.GetButtonDown("Fire1"))
            {
                OnClickButton(hoveredButton);
            }
        }
    }

    void OnClickButton(InventoryUIButton button)
    {
        print("b");
        ChangeWeaponDisplay(button);
        previousSelected = button;
        button.SetColor(ButtonMode.Pressed);
    }
}
