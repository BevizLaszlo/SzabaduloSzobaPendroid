using UnityEngine;

public class OpenLinks : MonoBehaviour
{
    public void OpenInBrowser(string url)
    {
        Application.OpenURL(url);
    }
}
