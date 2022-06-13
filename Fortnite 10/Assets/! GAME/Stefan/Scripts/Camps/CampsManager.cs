using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CampsManager : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI currentCamps, campsToGo;

    public Camp[] camps;
    public int campsCompleted;
    public bool invasionActive;

    public int CampsToGo { get { return camps.Length - campsCompleted; } }
    // Update is called once per frame
    void Update()
    {
        campsToGo.text = $"|    {camps.Length}";
        currentCamps.text = campsCompleted.ToString();
    }
}
