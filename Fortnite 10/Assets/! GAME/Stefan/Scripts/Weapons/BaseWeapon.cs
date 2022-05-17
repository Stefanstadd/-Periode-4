using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public Transform shootPos;
    public Transform fpsCam;
    Animator animator;

    [Header("Weapon Sway")]
    public float swayMultiplier;
    public Vector2 idleSpeed;
    public float idleSwayMultiplier;

    [Header("Upgrades")]
    public WeaponUpgrade[] possibleUpgrades;
    int currentUpgrade;

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
    public bool canChangeFireRate;
    public bool autoFire = true;

    [Space(20)]
    public float criticalHitChance;
    public float criticalMultiplier;

    public bool Critical
    {
        get { return Random.Range(0, 100) < criticalHitChance; }
    }

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
                crosshair.Rotate(135f);
                crosshair.SetColor(Color.red);
                crosshair.SetTargetSize(crosshair.defaultSize * 1.5f);
            }
            else
            {
                crosshair.Rotate(0f);
                crosshair.SetColor(Color.white);
                crosshair.SetTargetSize(crosshair.defaultSize);
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
    public virtual void Shoot()
    {
        crosshair.OnShoot();
        currentMagSize--;
    }

    public void ChangeFireRate()
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

    void UpdateWeaponSway()
    {
        var sway = new Vector3(-Mathf.Sin(Time.time * idleSpeed.x), Mathf.Sin(Time.time * idleSpeed.y),0);
        transform.localEulerAngles += sway * idleSwayMultiplier * Time.deltaTime;
    }

    public bool CanAffordNextUpgrade(int currentCurrency)
    {
        int nextUpgradeIndex = currentUpgrade + 1;
        //Als de speler de hoogste upgrade al heeft
        if (nextUpgradeIndex > possibleUpgrades.Length) return false;

        //als de speler genoeg currency heeft
        if (possibleUpgrades[nextUpgradeIndex].cost <= currentCurrency) return true;

        return false;   
    }

    public void OnBuyUpgrade()
    {
        var upgrade = possibleUpgrades[currentUpgrade];

        damage += upgrade.upDamage;
        fireRate += upgrade.upFireRate;
        reloadTime -= upgrade.reloadTime;
        maxMagSize += upgrade.maxMagSize;

        upgrade.onBuy.Play();

        currentUpgrade++;
    }
}

[System.Serializable]
public struct WeaponUpgrade
{
    [Header("Upgrade Settings")]
    public int upDamage;
    public float upFireRate;
    public float reloadTime;
    public int maxMagSize;

    [Header("Upgrade Settings")]
    public int count;
    public int cost;

    [Header("VFX")]

    public VisualEffect onBuy;
}
