using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public BaseWeapon currentWeapon;
    [Header("Bullets Display")]
    public TextMeshProUGUI currentMag,totalBullets;
    public Animator[] bulletsAnimator;

    [Header("Weapon Display")]
    public Animator weaponAnimator;

    [Header("Reload Button")]
    public Animator reloadButtonAnimator;
    public UIButton reloadButton;

    [Header("Fire Rate")]
    public Image fireRateImage;
    public Sprite singleFireSprite, autoFireSprite;
    public Vector3 singleFireScale, autoFireScale;
    public UIButton fireRateButton;
    public float moveDelta;

    bool reloading;

    private void Update()
    {
        if (currentWeapon)
        {
            //Bullets Display
            SetBulletsDisplay();
            CheckReloading();
            CheckFireRate();
        }
    }

    void SetBulletsDisplay()
    {
        currentMag.text = currentWeapon.currentMagSize.ToString();
        totalBullets.text = $"|  {PlayerInventory.arBullets}";

        for (int i = 0; i < bulletsAnimator.Length; i++)
        {
            bulletsAnimator[i].SetBool("FlashAnimation", currentWeapon.currentMagSize <= currentWeapon.displayFlashAmount);
        }
            
    }

    void CheckReloading()
    {
        if(reloading == false && currentWeapon.reloading)
        {
            OnReload();
            reloading = true;
        }
        if(reloading && currentWeapon.reloading == false)
        {
            OnReloadFinished();
            reloading = false;
        }
        weaponAnimator.SetBool("Reloading", currentWeapon.reloading);

        reloadButtonAnimator.SetBool("NoAmmo", currentWeapon.currentMagSize <= 0 && currentWeapon.reloading == false);
        
    }

    void OnReload()
    {
        reloadButton.OnUseButton();
        reloadButtonAnimator.SetBool("Reloading", true);
        
    }

    void OnReloadFinished()
    {
        reloadButtonAnimator.SetBool("Reloading", false);
    }

    void CheckFireRate()
    {
        if (currentWeapon.canChangeFireRate == false) return;

        if (Input.GetKeyDown(KeyCode.B))
        {
            currentWeapon.ChangeFireRate();

            fireRateImage.sprite = currentWeapon.autoFire ? autoFireSprite : singleFireSprite;
            fireRateImage.transform.localScale = Vector3.zero;
            fireRateImage.SetNativeSize();

            fireRateButton.OnUseButton();
        }
        fireRateImage.transform.localScale = 
        Vector3.MoveTowards(fireRateImage.transform.localScale,
        currentWeapon.autoFire ? autoFireScale : singleFireScale,
        moveDelta * Time.deltaTime);
    }

    public void SetWeapon(BaseWeapon weapon)
    {
        currentWeapon = weapon;
    }
}
