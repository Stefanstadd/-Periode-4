using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.VFX;

public abstract class BaseWeapon : MonoBehaviour
{
    [Header("Weapon Positioning")]
    public Vector3 localPos;
    public Vector3 localRot;

    [Header("References")]
    public Crosshair crosshair;
    public DamageTextManager damageText;
    public CameraScript camScript;
    public Transform shootPos;
    public Transform fpsCam;
    public InventoryManager inventoryManager;
    public Sprite weaponUIImage;
    Animator animator;

    [Header("Upgrades")]
    public WeaponUpgrade[] possibleUpgrades;
    public int currentUpgrade;

    [Header("Weapon Settings")]
    public string weaponName;
    [TextArea(2,5)]public string weaponDescription;

    [Space(20)]
    public int damage;
    public float fireRate;
    public float reloadTime;

    [Space(20)]
    public int maxMagSize;
    public int currentMagSize;
    public int displayFlashAmount;

    [Space(20)]
    public float nextTimeToFire;
    public bool reloading;
    public bool canChangeFireMode;
    public bool autoFire = true;

    [Space(20)]
    public float criticalHitChance;
    public float criticalMultiplier;

    [Space(20)]
    public float aimFOVDecrease;

    public PlayerMovement playerMovement;

    [Header("SFX")]
    public AudioSource audioSource;
    public AudioClip weaponSwitch,shoot;

    public bool Critical
    {
        get { return Random.Range(0, 100) < criticalHitChance; }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = transform.root.GetComponent<PlayerMovement>();
        currentMagSize = maxMagSize;
    }

    private void OnEnable()
    {
        if (weaponSwitch != null)
        {
            audioSource.clip = weaponSwitch;
            audioSource.Play();
        }
    }

    protected virtual void Update()
    {
        if (inventoryManager.IsInInventory() || GameMenuManager.Paused) return;
        CheckInput();

        //Raycasting
        RaycastHit hit;
        if(Physics.Raycast(shootPos.position,shootPos.forward,out hit))
        {
            crosshair.SetCrosshair(hit.point);

            if (CanHitTarget(hit))
            {
                crosshair.Rotate(135f);
                crosshair.SetColor(Color.red);
            }
            else
            {
                crosshair.Rotate(0f);
                crosshair.SetColor(Color.white);
            }
        }

        else crosshair.ResetCrosshair();
    }

    public void CheckInput()
    {
        nextTimeToFire -= Time.deltaTime;

        if ((Input.GetButtonDown("Fire1") && currentMagSize <= 0 ) || (Input.GetButtonDown("Reload") && currentMagSize != maxMagSize && reloading == false))
        {
            Reload();
        }

        if ((autoFire && Input.GetButton("Fire1") && reloading == false) ||
            (autoFire == false && Input.GetButtonDown("Fire1") && reloading == false))
            {
                if(currentMagSize > 0) 
                {
                    if (nextTimeToFire <= 0)
                    {
                        nextTimeToFire = 1 / fireRate;
                        Shoot();
                    }
                }
            }
    }

    public abstract void Reload();

    public virtual bool CanHitTarget(RaycastHit hit)
    {
        if (hit.collider.transform.root.CompareTag("Enemy")) return true;
        return false;
    }
    public virtual void Shoot()
    {
        crosshair.OnShoot();
        currentMagSize--;
        if(shoot != null)
        {
            audioSource.clip = shoot;
            audioSource.Play();
        }
    }

    public void ChangeFireMode()
    {
        autoFire = !autoFire;
    }

    public void OnSelectWeapon()
    {
        transform.localPosition = localPos;
        transform.localEulerAngles = localRot;
        GetComponent<Collider>().enabled = false;
    }

    public void OnDeselectWeapon()
    {
        GetComponent<Collider>().enabled = true;
        crosshair.ResetCrosshair();
    }

    public bool CanAffordNextUpgrade(int currentCurrency)
    {
        int nextUpgradeIndex = currentUpgrade;
        //Als de speler de hoogste upgrade al heeft
        if (nextUpgradeIndex >= possibleUpgrades.Length) return false;

        //als de speler genoeg currency heeft
        if (possibleUpgrades[nextUpgradeIndex].cost <= currentCurrency) return true;

        return false;   
    }

    public int NextUpgradeCost()
    {
        int index = currentUpgrade;
        if (index > possibleUpgrades.Length) return 0;
        return possibleUpgrades[index].cost;
    }

    public void OnBuyUpgrade()
    {
        var upgrade = possibleUpgrades[currentUpgrade];

        damage += upgrade.upDamage;
        fireRate += upgrade.increaseFireRate;
        reloadTime -= upgrade.reloadTime;
        maxMagSize += upgrade.maxMagSize;

        if(upgrade.onBuy != null)
        upgrade.onBuy.Play();

        currentUpgrade++;
    }
}

[System.Serializable]
public struct WeaponUpgrade
{
    [Header("Upgrade Settings")]
    public int upDamage;
    public float increaseFireRate;
    public float reloadTime;
    public int maxMagSize;

    [Header("Upgrade Settings")]
    public int count;
    public int cost;

    [Header("VFX")]

    public VisualEffect onBuy;
}
