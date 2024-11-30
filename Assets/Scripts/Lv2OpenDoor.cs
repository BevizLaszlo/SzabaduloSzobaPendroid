using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Lv2OpenDoor : MonoBehaviour
{
    private void Update()
    {
        if (GameData.Lv2KeyFound && gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
