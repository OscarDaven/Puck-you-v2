using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class Timer : MonoBehaviour
{
    public TMP_Text TimerTxt;
    public GameObject puck;
    public forceapplication fa;
    public bool hasPuck = false;
    public bool hasStarted = false;

    void Start() 
    {
        TimerTxt = GameObject.FindGameObjectWithTag("TimerText").GetComponent<TMP_Text>();
        fa = GetComponent<forceapplication>();
        Debug.Log(fa);
    }

    void Update() 
    {
        
        if (fa.timeTillShoot > 0)
        {
            updateTimerServerRpc(fa.timeTillShoot, "WAIT");
            updateTimer(fa.timeTillShoot, "WAIT");
        }
        else
        {
            updateTimerServerRpc(fa.timeTillReset, "SHOOT!");
            updateTimer(fa.timeTillReset, "SHOOT!");
        }


    }
    
    [ServerRpc(RequireOwnership = false)]
    void updateTimerServerRpc(float currentTime, string label)
    { 

        TimerTxt.text = string.Format("{0}\n{1}", label, currentTime.ToString("F1"));// string.Format("{0:00}",seconds);
    }
    void updateTimer(float currentTime, string label)
    { 

        TimerTxt.text = string.Format("{0}\n{1}", label, currentTime.ToString("F1"));// string.Format("{0:00}",seconds);
    }
         
}

 