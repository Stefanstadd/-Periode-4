using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeStationManager : MonoBehaviour
{
    public UpgradeStation[] stations;

    Transform target;
    private void Start()
    {
        target = PlayerMovement.player.transform;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        float minDst = float.MaxValue;
        UpgradeStation closestStation = null;
        foreach (var station in stations)
        {
            station.enabled = false;
            float dst = Vector3.Distance(station.transform.position, target.position);
            if (dst < minDst)
            {
                minDst = dst;
                closestStation = station;
            }
        }

        closestStation.enabled = true;
    }
}
