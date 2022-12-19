using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public float TimeLeft;
    public bool TimerOn = false; 
    public TMP_Text TimerTxt;

    void Start() 
    {
        TimerOn = true;
        TimeLeft = 10;

    }

    void Update() 
    {
        if (TimerOn) 
        {
            if (TimeLeft > 0)
            {
                updateTimer(TimeLeft);
                TimeLeft -= Time.deltaTime;
                
            }
            else {
                TimerOn = false; 

            }
        }
    }
    void updateTimer(float currentTime)
    { 

        TimerTxt.text= currentTime.ToString("F1");// string.Format("{0:00}",seconds);
    }
         
}

 