using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : MonoBehaviour
{
    public float healAmount;
    public float lifeTime = 6;
    public float radius;

    void Update()
    {
        if(Vector3.Distance(transform.position,PlayerMovement.player.transform.position) < radius)
        {
            PlayerMovement.currentHP += healAmount / lifeTime * Time.deltaTime;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
