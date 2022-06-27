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


    [Space(20)]
    [Header("Buttons")]

    public Transform buttonsParent;
    public GameObject buttonPrefab;
    public Vector3 startPos;
    public float minDst;
    public float spaceBtwn;

    private void Start()
    {
        CreateButtons();
    }


    void CreateButtons()
    {
        uIButtons = new InventoryUIButton[weapons.Length];

        for (int i = 0; i < weapons.Length; i++)
        {
            Vector3 pos = startPos + new Vector3(spaceBtwn * i, 0f, 0f);

            uIButtons[i] = Instantiate(buttonPrefab, pos, Quaternion.identity, buttonsParent).GetComponent<InventoryUIButton>();
            if (uIButtons[i])
            {
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
        var curWeaponUpgrade = button.weapon.possibleUpgrades[button.weapon.currentUpgrade];

        foreach (var img in colorChangeables)
        {
            img.color = curWeaponUpgrade.upgradeColor;
        }
    }

    void UpdateButtons()
    {
        InventoryUIButton hoveredButton = null;
        float closestDst = float.MaxValue;

        var mousePos = Input.mousePosition;
        for (int i = 0; i < uIButtons.Length; i++)
        {
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
                print("b");
                ChangeWeaponDisplay(hoveredButton);
                hoveredButton.SetColor(ButtonMode.Pressed);
            }
        }
    }
}
