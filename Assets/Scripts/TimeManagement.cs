using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManagement : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI timerText;
    private float currentTime;
    private Trigger timeoutTrigger;

    private const string TIMEOUTEVENTKEY = "TIMEOUT";
    private bool running;

    public static string TimeoutEventKey
    {
        get { return TIMEOUTEVENTKEY; }
    }

    public void StartTime(float time)
    {
        var eventPool = GameObject.FindObjectOfType<EventPool>();

        timeoutTrigger = this.gameObject.AddComponent<Trigger>();
        timeoutTrigger.eventKey = TIMEOUTEVENTKEY;
        timeoutTrigger.fireOnce = true;
        
        running = true;
        currentTime = time;
    }

    public void StopTimer()
    {
        var eventPool = GameObject.FindObjectOfType<EventPool>();
        eventPool.RemoveTriggerToEvent(TIMEOUTEVENTKEY, timeoutTrigger);
        running = false;
    }

    private void Update()
    {
        if (!running)
            return;

        if (currentTime <= 0)
        {
            //trigger event
            timeoutTrigger.IsTriggered = true;
        }
        else
        {
            currentTime -= Time.deltaTime;
            timerText.text = currentTime.ToString("0.00");
        }
    }
}
