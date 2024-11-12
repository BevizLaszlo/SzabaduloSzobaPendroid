using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLinks : MonoBehaviour
{
    public void OpenInBrowser(string url)
    {
        Application.OpenURL(url);
    }
}
