using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using System.Threading.Tasks;

public class RaycastWeapon : BaseWeapon
{
    [Space(20)]
    public float recoilMultiplier;
    public float recoilAimMultiplier;
    public float range;
    public float recoilAmount;
    public int amountOfShots = 1;

    [Header("VFX")]
    public VisualEffect muzzleFlash;
    public VisualEffect impact;

    protected override void Update()
    {
        base.Update();
        if (playerMovement.Aiming)
        {
            camScript.SetFOV(aimFOVDecrease);
        }
        else
        {
            camScript.ResetToDefault();
        }
    }
    public override void Shoot()
    {
        if(muzzleFlash) muzzleFlash.Play();
        base.Shoot();

        camScript.AddRecoil(recoilAmount);

        List<DamageInfo> damageInfos = new List<DamageInfo>();

        for (int i = 0; i < amountOfShots; i++)
        {
            float recoilAmount = 0.1f;

            float aimingAccuracyMultiplier = playerMovement.Aiming ? recoilAimMultiplier : 1;

            Vector3 accuracy = new Vector3(Random.Range(-recoilAmount, recoilAmount),
                                         Random.Range(-recoilAmount, recoilAmount),
                                         Random.Range(-recoilAmount, recoilAmount)) * recoilMultiplier * aimingAccuracyMultiplier;

            Vector3 shootDirection = shootPos.forward + accuracy;

            RaycastHit hit;
            if(Physics.Raycast(shootPos.position,shootDirection,out hit, range))
            {

                bool critical = Critical;
                float damage = critical? this.damage / amountOfShots * criticalMultiplier :this.damage / amountOfShots ;

                if (hit.transform.CompareTag("Enemy"))
                {
                    BaseEnemey enemy = hit.transform.GetComponent<BaseEnemey>();
                    if (damageInfos.Count == 0)
                    {
                        damageInfos.Add(new DamageInfo(enemy, damage, critical, hit.point,hit.transform.position));
                    }
                    else
                    {
                        int amount = damageInfos.Count;
                        bool foundSameTarget = false;

                        for (int y = 0; y < amount; y++)
                        {
                            print(damageInfos.Count);
                            if (damageInfos[y].hitId.Equals(hit.transform.position))
                            {
                                var info = damageInfos[y];
                                info.inflictedDamage += damage;
                                if (!info.critical && critical) info.critical = critical;

                                damageInfos[y] = info;

                                print("Zelfde target gevonden");

                                foundSameTarget = true;
                                break;
                            }
                        }
                        if (!foundSameTarget)
                        {
                            damageInfos.Add(new DamageInfo(enemy, damage, critical, hit.point, hit.transform.position));

                            print("Nieuwe damageInfo tiegevoegd");
                        }
                    }
                    
                    
                }

                if (this.impact)
                {
                    var impact = Instantiate(this.impact, hit.point, Quaternion.identity);
                    impact.transform.LookAt(transform);
                    impact.transform.position += impact.transform.forward * 0.4f;
                    impact.Play();
                    Destroy(impact.gameObject, 3f);
                }
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
                if (info.enemy)
                {
                    info.enemy.TakeDamage(info.inflictedDamage);
                }
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

    public override bool CanHitTarget(RaycastHit hit)
    {
        if(hit.distance <= range)
            return base.CanHitTarget(hit);

        return false;
    }

    public struct DamageInfo
    {
        public BaseEnemey enemy; //Enemy script refenece
        public float inflictedDamage; // aantal damage genomen
        public bool critical; // heeft deze enemy extra damage genomen?
        public Vector3 hitPoint; // locatie voor damage text
        public Vector3 hitId; // wordt gebruikt om te checken of deze target al gehit is, de transform.position

        public DamageInfo(BaseEnemey _enemy, float _inflictedDamage, bool _critical, Vector3 _hitPoint, Vector3 _hitId)
        {
            enemy = _enemy;
            inflictedDamage = _inflictedDamage;
            critical = _critical;
            hitPoint = _hitPoint;
            hitId = _hitId;
        }
    }
}
