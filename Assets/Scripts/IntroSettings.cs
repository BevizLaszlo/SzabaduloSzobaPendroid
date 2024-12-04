using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroSettings : MonoBehaviour
{
    [SerializeField] private Toggle introToggle;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("intro"))
            PlayerPrefs.SetInt("intro", 1);
        Load();
        introToggle.onValueChanged.AddListener(delegate { ChangeIntro(); });
    }

    public void ChangeIntro() => Save();

    private void Load() => introToggle.isOn = PlayerPrefs.GetInt("intro") == 1;
    private void Save() => PlayerPrefs.SetInt("intro", introToggle.isOn ? 1 : 0);
}
