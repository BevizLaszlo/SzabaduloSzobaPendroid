using TMPro;
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
        if(Buttons is not null) Buttons.gameObject.SetActive(false);
    }
    public void ExitDetailes()
    {
        Details.gameObject.SetActive(false);
        if(DetailText is not null) DetailText.text = string.Empty;
        if(Buttons is not null) Buttons.gameObject.SetActive(true);
        if (SelfDestroy) gameObject.SetActive(false);
    }

    public void InspectItem(string text) => DetailText.text = text;

    public void DestroyItemBad()
    {
        if (SelfDestroy) return;
        Destroyed();
        DetailText.text = "T�nkretetted a t�rgyat, ez�rt pontot vesztett�l";
        GameData.sanity-=2;
    }

    public void DestroyItemGood()
    {
        if (SelfDestroy) return;
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
