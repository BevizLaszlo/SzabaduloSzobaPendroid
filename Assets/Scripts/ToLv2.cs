using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToLv2 : MonoBehaviour
{
    
    private void Awake()
    {
        
        if(GameData.Lv2Done) StartCoroutine(LoadLevel("Lv3").GetEnumerator());
        else StartCoroutine(LoadLevel("Lv2").GetEnumerator());
    }
    IEnumerable LoadLevel(string sceneName)
    {
        //transition.SetTrigger("Start");

        yield return new WaitForSeconds(10.0f);

        SceneManager.LoadScene(sceneName);
    }
}
