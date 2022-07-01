using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HelpPopup : MonoBehaviour
{
    static List<string> messages = new List<string>();

    public float popuptime = 4;

    public float waitTime = 1;

    public TextMeshProUGUI text;

    public TimeLineMessage[] timeLineMessages;

    float timer;

    Animator animator;
    const string activeName = "Active";
    bool PoppingUp
    {
        get
        {
            return animator.GetBool(activeName);
        }
    }

    void Start()
    {
        timer = popuptime;
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        ManageMessages();

        ManageTimeline();
    }

    void ManageMessages()
    {
        print(messages.Count);
        if (messages.Count > 0 && timer <= 0 - waitTime)
        {
            ShowMessage(messages[0]);
            messages.RemoveAt(0);

            timer = popuptime;
        }
        else if (timer <= 0 && PoppingUp)
        {
            RemoveMessage();
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }

    void ManageTimeline()
    {
        float time = Time.time;
        for (int i = 0; i < timeLineMessages.Length; i++)
        {
            var currentMsg = timeLineMessages[i];
            if(time >= currentMsg.time && currentMsg.executed == false)
            {
                currentMsg.executed = true;
                AddPopup(currentMsg.message);   
            }
        }
    }

    void ShowMessage(string message)
    {
        animator.SetBool(activeName, true);
        text.text = message;
    }

    void RemoveMessage()
    {
        animator.SetBool(activeName, false);
    }

    public static void AddPopup(string message)
    {
        messages.Add(message);
    }

    [System.Serializable]
    public class TimeLineMessage
    {
        public string message;
        public float time;
        public bool executed;
    }
}
