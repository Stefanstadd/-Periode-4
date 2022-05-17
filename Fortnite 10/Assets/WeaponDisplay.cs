using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeaponDisplay : MonoBehaviour
{
    public BaseWeapon currentWeapon;
    public TextMeshProUGUI bullets;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentWeapon == null) return;
        bullets.text = $"{currentWeapon.currentMagSize} / {PlayerInventory.arBullets} ";
    }
}
