using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lv3OpenDoor : MonoBehaviour
{
    private void Update()
    {
        if (GameData.Lv3Solved && gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
