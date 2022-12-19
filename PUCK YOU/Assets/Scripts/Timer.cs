using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public TMP_Text TimerTxt;
    public GameObject puck;
    public forceapplication fa;

    void Start() 
    {
        puck = GameObject.Find("Puck");
        fa = puck.GetComponent<forceapplication>();
    }

    void Update() 
    {
        if (fa.timeTillShoot > 0)
        {
            updateTimer(fa.timeTillShoot, "WAIT");
        }
        else
        {
            updateTimer(fa.timeTillReset, "SHOOT!");
        }
    }
    void updateTimer(float currentTime, string label)
    { 

        TimerTxt.text= string.Format("{0}\n{1}", label, currentTime.ToString("F1"));// string.Format("{0:00}",seconds);
    }
         
}

 