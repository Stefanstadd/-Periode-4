using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

    public void FindStations()
    {
        var a = GameObject.FindGameObjectsWithTag("Upgrade Station");

        stations = new UpgradeStation[a.Length];

        for (int i = 0; i < stations.Length; i++)
        {
            stations[i] = a[i].GetComponent<UpgradeStation>();
        }

        print("Fetched stations");
    }
}
#if UNITY_EDITOR

[CustomEditor(typeof(UpgradeStationManager))]
[CanEditMultipleObjects]
public class UpgradeStationManagerEditor : Editor
{
    UpgradeStationManager obj;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        obj = target as UpgradeStationManager;

        if(GUILayout.Button("Fetch Stations"))
        {
            obj.FindStations();
        }
    }
}
#endif
