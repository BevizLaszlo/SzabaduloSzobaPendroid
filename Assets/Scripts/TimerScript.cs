using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timer;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameData.Timer > 0 && GameData.HardMode)
        {
            GameData.Timer -= Time.deltaTime;
            int seconds = Mathf.RoundToInt(GameData.Timer % 60);
            if (seconds == 59) GameData.minutes -= Time.deltaTime;
            timer.text = string.Format("{0:00}:{1:00}", GameData.minutes, seconds == 60 ? "00" : seconds);
        }
    }
}
