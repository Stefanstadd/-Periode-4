using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using System.Threading.Tasks;

public class AssaultRifle : BaseWeapon
{
    [Space(20)]
    public float recoilMultiplier;
    public float range;

    [Header("VFX")]
    public VisualEffect muzzleFlash;
    public VisualEffect impact;
    public override void Shoot()
    {
        muzzleFlash.Play();
        base.Shoot();

        float recoilAmount = 0.1f;

        Vector3 recoil = new Vector3(Random.Range(-recoilAmount, recoilAmount),
                                     Random.Range(-recoilAmount, recoilAmount),
                                     Random.Range(-recoilAmount, recoilAmount)) * recoilMultiplier;

        Vector3 shootDirection = shootPos.forward + recoil;

        RaycastHit hit;
        if(Physics.Raycast(shootPos.position,shootDirection,out hit, range))
        {
            int damage = GetDamage();

            if (hit.transform.CompareTag("Enemy"))
            {
                damageText.SetDamageText(hit.point, damage, damage != this.damage);
                crosshair.OnHitEnemy();
            }

            var impact = Instantiate(this.impact, hit.point, Quaternion.identity);
            impact.transform.LookAt(transform);
            impact.transform.position += impact.transform.forward * 0.4f;
            impact.Play();
            Destroy(impact.gameObject,3f);
        }
    }

    public override async void Reload()
    {
        if (PlayerInventory.arBullets <= 0) return;
        reloading = true;

        await Task.Delay(Mathf.RoundToInt(reloadTime * 1000));

        //bereken hoeveel bullets de speler gebruikt
    
        int bulletsNeeded = maxMagSize - currentMagSize;
        currentMagSize += bulletsNeeded;
        PlayerInventory.arBullets -= bulletsNeeded;

        if (PlayerInventory.arBullets < 0)
        {
            currentMagSize += PlayerInventory.arBullets;
        }
    
        print(PlayerInventory.arBullets);
        reloading = false;
    }
}
