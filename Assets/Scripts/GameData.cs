using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static bool Lv2Done = false;
    public static bool HardMode = false;
    public static float Timer = 1800;
    public static float minutes = 30;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Lv2() => Lv2Done = true;

    public void HardGameMode() => HardMode = true;
}
