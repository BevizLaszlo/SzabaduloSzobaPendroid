using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SanityRenderer : MonoBehaviour
{
    [SerializeField] private Image sanityBar;
    [SerializeField] private TextMeshProUGUI sanityText;

    private float sanity;
    private float targetFillAmount;

    private void Start()
    {
        sanity = GameData.sanity;
        targetFillAmount = sanity/10f;
        sanityBar.fillAmount = targetFillAmount;
        sanityText.text = $"10/{sanity}";
    }

    private void Update()
    {
        if (sanity != GameData.sanity)
        {
            sanity = GameData.sanity;
            sanityText.text = $"10/{sanity}";
        }
        if (targetFillAmount > sanity/10f)
        {
            targetFillAmount -= .01f;
            sanityBar.fillAmount = targetFillAmount;
        }

    }
}