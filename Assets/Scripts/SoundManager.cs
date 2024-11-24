using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Slider VolumeSlider;
    void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
            PlayerPrefs.SetFloat("musicVolume", .5f);
        Load();
    }

    public void ChangeVolume()
    {
        AudioListener.volume = VolumeSlider.value;
        Save();
    }
    private void Load() => VolumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
    private void Save() => PlayerPrefs.SetFloat("musicVolume", VolumeSlider.value);
}
