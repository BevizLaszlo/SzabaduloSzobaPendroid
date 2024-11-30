using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetails : MonoBehaviour
{
    [SerializeField ] Canvas Details;
    [SerializeField] Canvas Buttons;
    [SerializeField ] TextMeshProUGUI DetailText;
    [SerializeField] Button InspectButton;
    private bool SelfDestroy = false;

    private void Awake()
    {
        //Details = GetComponent<Canvas>();
    }

    public void ActivateDetailes()
    {
        Details.gameObject.SetActive(true);
        Buttons.gameObject.SetActive(false);
    }
    public void ExitDetailes()
    {
        Details.gameObject.SetActive(false);
        Buttons.gameObject.SetActive(true);
        if (SelfDestroy) gameObject.SetActive(false);
    }

    public void InspectItem() => DetailText.text = "Valami sz�veg seg�ts�gnek";

    public void DestroyItemBad()
    {
        Destroyed();
        DetailText.text = "T�nkretetted a t�rgyat, ez�rt pontot vesztett�l";
        GameData.sanity--;
    }

    public void DestroyItemGood()
    {
        Destroyed();
        DetailText.text = "Megtal�ltad a kulcsot";
        GameData.Lv2KeyFound = true;
    }

    public void Destroyed()
    {
        InspectButton.gameObject.SetActive(false);
        SelfDestroy = true;
    }
}