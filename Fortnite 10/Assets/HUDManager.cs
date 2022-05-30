using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public GameObject alreadyReloadingText;

    [Header("Fire Rate")]
    public Image fireRateImage;
    public GameObject fireModeText;
    public Sprite singleFireSprite, autoFireSprite;
    public Vector3 singleFireScale, autoFireScale;
    public UIButton fireRateButton;
    public float moveDelta;

    [Header("Healing")]
    public UIButton healButton;
    public Animator healAnimator;
    public GameObject failedHealText;

    bool reloading;

    private void Start()
    {
        //Update de firerate button
    }
    private void Update()
    {
        if (currentWeapon)
        {
            //Bullets Display
            SetBulletsDisplay();
            CheckReloading();
            CheckFireMode();
        }

        if (currentWeapon)
        {
            fireRateImage.transform.localScale =
            Vector3.MoveTowards(fireRateImage.transform.localScale,
            currentWeapon.autoFire ? autoFireScale : singleFireScale,
            moveDelta * Time.deltaTime);
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

        if (reloading && Input.GetButtonDown("Reload"))
        {
            alreadyReloadingText.SetActive(true);
            fireModeText.SetActive(false);
            failedHealText.SetActive(false);

            if (!IsInvoking("DisableAlreadyReloading"))
            {
                Invoke("DisableAlreadyRelaoding", 1.5f);
            }
        }

        reloadButtonAnimator.SetBool("NoAmmo", currentWeapon.currentMagSize <= 0 && currentWeapon.reloading == false);
        
    }

    void DisableAlreadyReloading() => alreadyReloadingText.SetActive(false);

    void OnReload()
    {
        reloadButton.OnUseButton();
        reloadButtonAnimator.SetBool("Reloading", true);
        
    }

    void OnReloadFinished()
    {
        reloadButtonAnimator.SetBool("Reloading", false);
    }

    void CheckFireMode()
    {

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (currentWeapon.canChangeFireMode)
            {

                currentWeapon.ChangeFireMode();

                fireRateImage.sprite = currentWeapon.autoFire ? autoFireSprite : singleFireSprite;
                fireRateImage.transform.localScale = Vector3.zero;
                fireRateImage.SetNativeSize();

                fireRateButton.OnUseButton();
            }
            else
            {
                fireModeText.SetActive(true);
                alreadyReloadingText.SetActive(false);
                failedHealText.SetActive(false);

                if(!IsInvoking("DisableText"))
                Invoke("DisableText", 1.5f);

            }

        }
    }

    void DisableText() => fireModeText.SetActive(false);

    public async void OnHeal(float time)
    {
        print("a");
        healButton.OnUseButton();

        //healAnimator.SetInteger("HealStage", 1);
        //await Task.Delay(0625);
        print("b");
        
        float length = (time - 0.625f) / 8;

        for (int i = 0; i <= 8; i++)
        {
            healAnimator.SetInteger("HealStage", i + 1);
            await Task.Delay(Mathf.RoundToInt(length * 1000));
            print("c");   
        }
        healAnimator.SetInteger("HealStage", 0);

    }

    public void OnHealFailed()
    {
        failedHealText.SetActive(true);
        alreadyReloadingText.SetActive(false);
        fireModeText.SetActive(false);

        if (!IsInvoking("DisableFailedHealing"))
        {
            Invoke("DisableFailedHealing", 1.5f);
        }
    }

    void DisableFailedHealing()
    {
        failedHealText.SetActive(false);
    }

    void OnSwitchWeapons()
    {
        if (currentWeapon == null) return;
        fireRateImage.sprite = currentWeapon.autoFire ? autoFireSprite : singleFireSprite;
        fireRateImage.SetNativeSize();
    }


    public void SetWeapon(BaseWeapon weapon)
    {
        currentWeapon = weapon;
        OnSwitchWeapons();
    }
}
