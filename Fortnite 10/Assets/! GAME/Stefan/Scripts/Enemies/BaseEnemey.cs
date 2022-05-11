using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnemyState
{
    Idle,
    Wandering,
    Fleeing,
    Attacking,
    Threatening
}
public abstract class BaseEnemey : MonoBehaviour
{
    [Header("Settings")]
    public EnemyState state;
    public string enemyName;
    public float hp;
    public float maxHP;
    public float resistance;
    public float detectRadius,threateningRadius,attackRadius;


    [Header("Movement")]
    public float movementSpeed;
    public float rotationSpeed;
    [Header("Other")]
    HealthScript target;

    protected virtual void Start()
    {
        InvokeRepeating("LocateTarget", 0, 0.1f);
    }

    public void TakeDamage(float dmg)
    {
        hp -= dmg * resistance;
        if(hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void LocateTarget()
    {
        var colliders = Physics.OverlapSphere(transform.position,detectRadius);
        foreach (var col in colliders)
        {
            var healthScript = col.GetComponent<HealthScript>();
            if(healthScript != null)
            {
                float dst = Vector3.Distance(transform.position, healthScript.transform.position);
                if(dst < threateningRadius)
                {
                    state = EnemyState.Attacking;
                }
                else
                {
                    state = EnemyState.Threatening;
                }
                target = healthScript;
            }
        }
    }
}
