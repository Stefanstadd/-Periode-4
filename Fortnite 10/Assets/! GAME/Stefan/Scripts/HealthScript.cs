using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public float hp;
    public float maxHP;
    public float resistance;
    
    public void TakeDamage(float dmg)
    {
        hp -= dmg * resistance;
        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
