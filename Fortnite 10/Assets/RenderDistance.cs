using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderDistance : MonoBehaviour
{
    Transform target;

    public float disableDistance = 150;
    public Color gizmoColor = Color.yellow;
    public bool active = true;
    void Start()
    {
        if (PlayerMovement.player == null) return;
        target = PlayerMovement.player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;

        float dst = Vector3.Distance(target.position, transform.position);
        if (dst > disableDistance && active)
        {
            Change(false);
            active = false;
        }
        else if(dst < disableDistance && !active)
        {
            Change(true);
            active = true;
        }
    }

    void Change(bool value)
    {
        foreach (Transform transform in transform)
        {
            transform.gameObject.SetActive(value);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, disableDistance);
    }
}
