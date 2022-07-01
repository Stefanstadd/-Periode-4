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
    bool done;
    // Update is called once per frame
    void Update()
    {
        campsToGo.text = $"|    {camps.Length}";
        currentCamps.text = campsCompleted.ToString();

        if(CampsToGo <= 0 && done == false)
        {
            done = true;
            HelpPopup.AddPopup("You have murdered all the goblins, thank you for your hard work");
        }
    }
}
