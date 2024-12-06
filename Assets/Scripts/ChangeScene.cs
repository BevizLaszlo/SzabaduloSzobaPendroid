using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    //public Animator transition;
    private void Awake()
    {
            StartCoroutine(LoadLevel("Lv1").GetEnumerator());
        
    }
    IEnumerable LoadLevel(string sceneName)
    {
        //transition.SetTrigger("Start");

        yield return new WaitForSeconds(42.0f);

        SceneManager.LoadScene(sceneName);
    }
}
