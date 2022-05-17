using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using System.Threading.Tasks;

public class RaycastWeapon : BaseWeapon
{
    [Space(20)]
    public float recoilMultiplier;
    public float range;
    public int amountOfShots = 1;

    [Header("VFX")]
    public VisualEffect muzzleFlash;
    public VisualEffect impact;
    public override void Shoot()
    {
        muzzleFlash.Play();
        base.Shoot();

        List<DamageInfo> damageInfos = new List<DamageInfo>();

        for (int i = 0; i < amountOfShots; i++)
        {
            float recoilAmount = 0.1f;

            Vector3 accuracy = new Vector3(Random.Range(-recoilAmount, recoilAmount),
                                         Random.Range(-recoilAmount, recoilAmount),
                                         Random.Range(-recoilAmount, recoilAmount)) * recoilMultiplier;

            Vector3 shootDirection = shootPos.forward + accuracy;

            RaycastHit hit;
            if(Physics.Raycast(shootPos.position,shootDirection,out hit, range))
            {
                bool critical = Critical;
                float damage = critical? this.damage / amountOfShots * criticalMultiplier :this.damage / amountOfShots ;

                if (hit.transform.CompareTag("Enemy"))
                {
                    if (damageInfos.Count == 0)
                    {
                        damageInfos.Add(new DamageInfo(hit.transform, damage, critical, hit.point));
                    }
                    else
                    {
                        for (int y = 0; y < damageInfos.Count; y++)
                        {
                            print(damageInfos[y].target);
                            print(hit.transform);
                            if (damageInfos[y].target == hit.transform)
                            {
                                print("a");
                                print(damage);

                                var info = damageInfos[y];
                                info.inflictedDamage += damage;
                                if (!info.critical && critical) info.critical = critical;

                                damageInfos[y] = info;
                            }
                            else
                            {
                                damageInfos.Add(new DamageInfo(hit.transform, damage, critical, hit.point));
                            }
                        }
                    }
                    
                    
                }

                var impact = Instantiate(this.impact, hit.point, Quaternion.identity);
                impact.transform.LookAt(transform);
                impact.transform.position += impact.transform.forward * 0.4f;
                impact.Play();
                Destroy(impact.gameObject,3f);
            }
        }

        print(damageInfos.Count);
        if (damageInfos.Count != 0)
        {
            crosshair.OnHitEnemy();
            for (int i = 0; i < damageInfos.Count; i++)
            {
                var info = damageInfos[i];
                damageText.SetDamageText(info.hitPoint, Mathf.RoundToInt(info.inflictedDamage), info.critical);
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
            PlayerInventory.arBullets = 0;
        }
    
        print(PlayerInventory.arBullets);
        reloading = false;
    }

    public struct DamageInfo
    {
        public Transform target;
        public float inflictedDamage;
        public bool critical;
        public Vector3 hitPoint;

        public DamageInfo(Transform _target, float _inflictedDamage, bool _critical,Vector3 _hitPoint)
        {
            target = _target;
            inflictedDamage = _inflictedDamage;
            critical = _critical;
            hitPoint = _hitPoint;
        }
    }
}
