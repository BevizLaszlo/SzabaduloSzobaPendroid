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
            float minutes = Mathf.Floor(GameData.Timer / 60);
            int seconds = Mathf.RoundToInt(GameData.Timer % 60);
            timer.text = string.Format("{0:00}:{1:00}", minutes, seconds == 60 ? "00" : seconds);
            if(timer.text == "00:00") GameData.Timer = 0;
        }
    }
}
