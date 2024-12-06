using UnityEngine;

public class Music : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        GameObject[] music = GameObject.FindGameObjectsWithTag("Music");
        if (music.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        AudioListener.volume = PlayerPrefs.GetFloat("musicVolume");
    }
}
