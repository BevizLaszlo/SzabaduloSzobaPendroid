using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameBackToMenu : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(LoadLevel("Menu").GetEnumerator());

    }
    IEnumerable LoadLevel(string sceneName)
    {
        //transition.SetTrigger("Start");

        yield return new WaitForSeconds(42.0f);

        SceneManager.LoadScene(sceneName);
    }

}
