using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;
public class CanvasController : MonoBehaviour
{
    [SerializeField] Canvas MainMenu;
    [SerializeField] Canvas Credits;
    [SerializeField] Canvas SelectMode;
    [SerializeField] Canvas Settings;

    [SerializeField] Button BackToMenu1;
    [SerializeField] Button BackToMenu2;
    [SerializeField] Button ToSettings;
    [SerializeField] Button ToCredits;
    [SerializeField] Button ToSelectMode;


    // Update is called once per frame
    void Update()
    {
        ToSettings.onClick.AddListener(() =>
        {
            MainMenu.gameObject.SetActive(false);
            Credits.gameObject.SetActive(false);
            SelectMode.gameObject.SetActive(false);
            Settings.gameObject.SetActive(true);
        });

        ToCredits.onClick.AddListener(() =>
        {
            MainMenu.gameObject.SetActive(false);
            Credits.gameObject.SetActive(true);
            SelectMode.gameObject.SetActive(false);
            Settings.gameObject.SetActive(false);
        });

        ToSelectMode.onClick.AddListener(() =>
        {
            MainMenu.gameObject.SetActive(false);
            Credits.gameObject.SetActive(false);
            SelectMode.gameObject.SetActive(true);
            Settings.gameObject.SetActive(false);
        });

        BackToMenu1.onClick.AddListener(() =>
        {
            MainMenu.gameObject.SetActive(true);
            Credits.gameObject.SetActive(false);
            SelectMode.gameObject.SetActive(false);
            Settings.gameObject.SetActive(false);
        });

        BackToMenu2.onClick.AddListener(() =>
        {
            MainMenu.gameObject.SetActive(true);
            Credits.gameObject.SetActive(false);
            SelectMode.gameObject.SetActive(false);
            Settings.gameObject.SetActive(false);
        });
    }

}
