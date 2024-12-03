using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class DisplayMode : MonoBehaviour
{
    [SerializeField] private Button NormalButton;
    [SerializeField] private Button HardButton;
    [SerializeField] private TextMeshProUGUI ModeDesc;

    private bool isHardMode;

    private void Start()
    {
        isHardMode = GameData.HardMode;
        RenderMode();
    }

    private void Update()
    {
        if (isHardMode != GameData.HardMode)
        {
            isHardMode = GameData.HardMode;
            RenderMode();
        }
    }

    private void RenderMode()
    {
        if (isHardMode)
        {
            HardButton.GetComponent<Image>().color = new Color(221 / 255f, 231 / 255f, 5 / 255f);
            HardButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color(24 / 255f, 24 / 255f, 24 / 255f);

            NormalButton.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            NormalButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color(226 / 255f, 226 / 255f, 226 / 255f);

            ModeDesc.text = $"Nehéz módban 30 perces időkorlát van.";
        }
        else
        {
            NormalButton.GetComponent<Image>().color = new Color(221 / 255f, 231 / 255f, 5 / 255f);
            NormalButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color(24 / 255f, 24 / 255f, 24 / 255f);

            HardButton.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            HardButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color(226 / 255f, 226 / 255f, 226 / 255f);

            ModeDesc.text = "Normál módban korlátlan idővel rendelkezik.";
        }
    }
}
