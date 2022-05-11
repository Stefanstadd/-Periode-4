using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    const int weaponCount = 3; // max 9
    public int selectedWeapon = 1;

    public static int arBullets = 240;

    public Transform WeaponHolder;

    private void Start() => UpdateInventory();
    

    void Update()
    {
        int previousWeapon = selectedWeapon;
        CheckInput();
        if(previousWeapon != selectedWeapon)
        {
            UpdateInventory();
        }
    }

    void UpdateInventory()
    {
        int index = 1;
        foreach (Transform w in WeaponHolder)
        {
            if (index == selectedWeapon)
            {
                w.gameObject.SetActive(true);

                var weapon = w.GetComponent<BaseWeapon>();
                if (weapon != null)
                {
                    weapon.OnSelectWeapon();
                }
            }
            else
            {
                w.gameObject.SetActive(false);

                var weapon = w.GetComponent<BaseWeapon>();
                if (weapon != null)
                {
                    weapon.OnDeselectWeapon();
                }

            }
            index++;
        }
    }

    void CheckInput()
    {
        for (int i = 1; i <= weaponCount; i++)
        {
            if (Input.GetButtonDown(i.ToString()))
            {
                //Als de speler bijvoorbeeld wapen 1 heeft geselecteerd, en drukt er nog een keer op 1, word er geen wapen geselecteerd
                if(selectedWeapon == i)
                    selectedWeapon = weaponCount + 1;
                else
                    selectedWeapon = i;
            }
        }

        if (!Input.GetButton("Left Ctrl"))
        {
            if (Input.mouseScrollDelta.y > 0) // de speler scrolled omhoog
            {
                if (selectedWeapon == weaponCount + 1)
                    selectedWeapon = 1;
                else
                    selectedWeapon++;
            }
            else if (Input.mouseScrollDelta.y < 0) // de speler scrolled omlaag
            {
                if (selectedWeapon == 1)
                    selectedWeapon = weaponCount + 1;
                else
                    selectedWeapon--;
            }
        }
    }
        
}
