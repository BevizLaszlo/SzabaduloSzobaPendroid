using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public Animator transition;

    public void MoveToScene(string sceneName)
    {
        if (!this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(true);
        }
        // Start the coroutine properly
        //StartCoroutine(LoadLevel(sceneName));
        SceneManager.LoadScene(sceneName);
    }
    /*
    IEnumerator LoadLevel(string sceneName)
    {
        // Start animation
        transition.SetTrigger("Start");

        // Wait for the animation duration (adjust the duration based on your animation)
        yield return new WaitForSeconds(1.5f);

        // Load the scene
        SceneManager.LoadScene(sceneName);
    }*/

    public void StartGame()
    {
        if(PlayerPrefs.HasKey("intro") && PlayerPrefs.GetInt("intro") == 0)
            SceneManager.LoadScene("Lv1");
        else
            SceneManager.LoadScene("Story");
    }
}
