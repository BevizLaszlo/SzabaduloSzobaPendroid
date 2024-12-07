using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameBackToMenu : MonoBehaviour
{
    private void Awake()
    {
        if(GameData.sanity < 10) GameData.sanity = 10;
        if (GameData.Timer < 1800) GameData.Timer = 1800;
        GameData.Lv2Done = false;
        GameData.Lv2KeyFound = false;
        GameData.Lv3Solved = false;
        StartCoroutine(LoadLevel("Menu").GetEnumerator());
    }
    IEnumerable LoadLevel(string sceneName)
    {
        //transition.SetTrigger("Start");

        yield return new WaitForSeconds(42.0f);

        SceneManager.LoadScene(sceneName);
    }

}
