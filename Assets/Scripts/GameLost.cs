using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLost : MonoBehaviour
{
    [SerializeField] Canvas LostGame;

    // Update is called once per frame
    void Update()
    {
        if(GameData.sanity == 0 || GameData.Timer == 0) LostGame.gameObject.SetActive(true);
        
    }

    public void GameReset()
    {
        if (GameData.Lv2Done)
        {
            GameData.Lv2Done = false;
            GameData.Lv2KeyFound = false;
        }
        GameData.Timer = 1800;
    }

    public void MoveToMenu()
    {
        GameReset();
        if (!this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(true);
        }
        // Start the coroutine properly
        //StartCoroutine(LoadLevel(sceneName));
        SceneManager.LoadScene("Menu");
    }

    public void Restart()
    {
        GameReset();
        if (!this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(true);
        }
        // Start the coroutine properly
        //StartCoroutine(LoadLevel(sceneName));
        SceneManager.LoadScene("Lv1");
    }
}
