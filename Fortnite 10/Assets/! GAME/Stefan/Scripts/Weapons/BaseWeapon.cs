using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public abstract class BaseWeapon : MonoBehaviour
{
    [Header("Initializing")]
    public Vector3 localPos;
    public Vector3 localRot;

    [Header("References")]
    public Crosshair crosshair;
    public DamageTextManager damageText;
    Animator animator;

    [Header("Weapon Sway")]
    public float swayMultiplier;
    public Vector2 idleSpeed;
    public float idleSwayMultiplier;


    [Header("Weapon Settings")]
    public string weaponName;
    [TextArea(2,5)]public string weaponDescription;

    [Space(20)]
    public int damage;
    public float fireRate;
    public float reloadTime;

    public int maxMagSize;
    public int currentMagSize;
    float nextTimeToFire;

    public bool reloading;

    [Space(20)]
    public float criticalHitChance;
    public float criticalMultiplier;
    
    [Space(20)]
    public Transform shootPos;
    public Transform fpsCam;

    [Header("Upgrades")]
    public WeaponUpgrade[] possibleUpgrades;
    int currentUpgrade;

    public bool autoFire;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateWeaponSway();

        CheckInput();

        //Raycasting
        RaycastHit hit;
        if(Physics.Raycast(transform.position,shootPos.forward,out hit))
        {
            crosshair.SetCrosshair(hit.point);

            if (hit.transform.CompareTag("Enemy"))
            {
                crosshair.Rotate(true);
                crosshair.SetColor(Color.red);
                crosshair.SetTargetSize(crosshair.defaultSize * 1.5f);
            }
            else
            {
                crosshair.Rotate(false);
                crosshair.SetColor(Color.white);
                crosshair.SetTargetSize(crosshair.defaultSize);
            }
        }

        else crosshair.ResetCrosshair();
    }

    public void CheckInput()
    {
        nextTimeToFire -= Time.deltaTime;
        if (Input.GetButton("Fire1") && reloading == false)
        {
            if(currentMagSize > 0) 
            {
                if (nextTimeToFire <= 0)
                {
                    nextTimeToFire = 1 / fireRate;
                    Shoot();
                }
            }
            else
            {
                Reload();
            }
        }
    }

    public abstract void Reload();
    public virtual void Shoot()
    {
        crosshair.OnShoot();
        currentMagSize--;
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

    void UpdateWeaponSway()
    {
        var sway = new Vector3(-Mathf.Sin(Time.time * idleSpeed.x), Mathf.Sin(Time.time * idleSpeed.y),0);
        transform.localEulerAngles += sway * idleSwayMultiplier * Time.deltaTime;
    }

    protected int GetDamage()
    {
        return Random.Range(0, 100) < criticalHitChance ? Mathf.RoundToInt(damage * criticalMultiplier) : damage;
    }

    public bool CanAffordNextUpgrade(int currentCurrency)
    {
        int nextUpgradeIndex = currentUpgrade + 1;
        //Als de speler de hoogste upgrade al heeft
        if (nextUpgradeIndex > possibleUpgrades.Length) return false;

        //als de speler genoeg currency heeft
        if (possibleUpgrades[nextUpgradeIndex].cost < currentCurrency) return true;

        return false;   
    }

    public void OnBuyUpgrade()
    {
        var upgrade = possibleUpgrades[currentUpgrade];
        damage = upgrade.upDamage;
        fireRate = upgrade.upFireRate;

        currentUpgrade++;
    }
}

[System.Serializable]
public struct WeaponUpgrade
{
    [Header("To Upgrade")]
    public int upDamage;
    public float upFireRate;

    [Header("Upgrade Settings")]
    public int count;
    public int cost;
}
