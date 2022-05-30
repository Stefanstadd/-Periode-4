using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    const int weaponCount = 3; // max 9
    public int selectedWeapon = 1;
    [Header("Healing")]
    public int healthPotions = int.MaxValue;
    [Range(2,30)]public float timeBetweenHealing = 15f;
    [SerializeField]float healTimer;
    public GameObject VFX_Healing;

    public static int arBullets = int.MaxValue;

    public Transform WeaponHolder;
    public HUDManager display;
    BaseWeapon currentWeapon;

    public GameObject enemy;

    private void Start()
    {
        UpdateInventory();
        //healTimer = timeBetweenHealing;
    }
    

    void Update()
    {
        if (currentWeapon != null && currentWeapon.reloading) return;

        int previousWeapon = selectedWeapon;
        CheckInput();
        if(previousWeapon != selectedWeapon)
        {
            UpdateInventory();
        }

        CheckHealing();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            print("z");
            for (int i = 0; i < 3; i++)
            {
                Instantiate(enemy,transform.position + new Vector3(Random.insideUnitCircle.x,0,Random.insideUnitCircle.y) * 10,Quaternion.identity);
            }
        }
    }

    void UpdateInventory()
    {
        BaseWeapon _selectedWeapon = null;
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
                    _selectedWeapon = weapon;
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
        currentWeapon = _selectedWeapon;
        display.SetWeapon(_selectedWeapon);
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

    void CheckHealing()
    {
        if(healTimer >= 0)
        healTimer -= Time.deltaTime;

        if (Input.GetButtonDown("Heal"))
        {
            if (healTimer <= 0 && healthPotions > 0)
            {
                OnHeal();
            }
            else
            {
                display.OnHealFailed();
            }
        }

        if(healTimer < 0)
        { 

        }
    }

    void OnHeal()
    {
        healthPotions--;
        display.OnHeal(timeBetweenHealing);
        healTimer = timeBetweenHealing;

        Vector3 pos = transform.position - Vector3.up * 0.49f;
        var a = Instantiate(VFX_Healing, pos, Quaternion.identity).transform;
        a.transform.localEulerAngles = new Vector3(-90, 0, 0);
        Destroy(a.gameObject,8f);
    }
        
}
