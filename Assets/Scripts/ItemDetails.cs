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
        if(DetailText is not null) DetailText.text = string.Empty;
        Buttons.gameObject.SetActive(true);
        if (SelfDestroy) gameObject.SetActive(false);
    }

    public void InspectItem(string text) => DetailText.text = text;

    public void DestroyItemBad()
    {
        Destroyed();
        DetailText.text = "Tönkretetted a tárgyat, ezért pontot vesztettél";
        GameData.sanity--;
    }

    public void DestroyItemGood()
    {
        Destroyed();
        DetailText.text = "Megtaláltad a kulcsot";
        GameData.Lv2KeyFound = true;
    }

    public void Destroyed()
    {
        InspectButton.gameObject.SetActive(false);
        SelfDestroy = true;
    }
}
