using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using System.Threading.Tasks;

public class RaycastWeapon : BaseWeapon
{
    public float recoilMultiplier;
    public float range;

    public VisualEffect muzzleFlash;
    public override void Shoot()
    {
        print(currentMagSize);
        muzzleFlash.Play();
        base.Shoot();
        float recoilAmount = 0.1f;

        Vector3 recoil = new Vector3(Random.Range(-recoilAmount, recoilAmount),
                                     Random.Range(-recoilAmount, recoilAmount),
                                     Random.Range(-recoilAmount, recoilAmount)) * recoilMultiplier;

        Vector3 direction = shootPos.forward + recoil;

        RaycastHit hit;
        if(Physics.Raycast(shootPos.position,direction,out hit, range))
        {
            int damage = GetDamage();

            if (hit.transform.CompareTag("Enemy"))
            {
                damageText.SetDamageText(hit.point, damage, damage != this.damage);
                crosshair.OnHitEnemy();
            }
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
