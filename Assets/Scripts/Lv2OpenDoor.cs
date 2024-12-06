using UnityEngine;

public class Lv2OpenDoor : MonoBehaviour
{
    private void Update()
    {
        if (GameData.Lv2KeyFound && gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
