using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLost : MonoBehaviour
{
    [SerializeField] Canvas LostGame;
    [SerializeField] TextMeshProUGUI LostText;

    // Update is called once per frame
    void Update()
    {
        if (GameData.sanity <= 0 || GameData.Timer == 0)
        {
            LostGame.gameObject.SetActive(true);
            if (GameData.Timer == 0) LostText.text = "Állj! Az idő lejárt! Nem sikerült befejeznie a tesztet! Az ítélet dehidratáció! Megkezdés, azonnali hatállyal!";
            else LostText.text = "Gratulálok! A teszt véget ért! Bebizonyította, hogy Ön nem lenne képes az életét tovább folytatni az Oaszisz körében. Ítélet nem más, mint dehidratáció az Oaszisz népe javára!";
        }
       
        
    }

    public void GameReset()
    {
        if (GameData.Lv2Done)
        {
            GameData.Lv2Done = false;
            GameData.Lv2KeyFound = false;
        }
        GameData.Timer = 1800;
        GameData.sanity = 10;
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
