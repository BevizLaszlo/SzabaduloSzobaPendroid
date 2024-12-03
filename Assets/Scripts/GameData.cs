using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static bool Lv2Done = false;
    public static bool HardMode = false;
    public static bool Lv2KeyFound = false;
    public static bool Lv3Solved = false;
    public static float Timer = 1800;
    public static int sanity = 10;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Lv2() => Lv2Done = true;
    public void NormalGameMode() => HardMode = false;
    public void HardGameMode() => HardMode = true;

    public void SanityLoss() => sanity--;
}
