using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        PlayerPrefs.DeleteAll();

        float masterVol = PlayerPrefs.GetFloat("MasterVolume", 100f);
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 100f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 100f);

        masterSlider.value = masterVol;
        musicSlider.value = musicVol;
        sfxSlider.value = sfxVol;

        // Áp dụng giá trị ngay lúc khởi chạy
        SetMasterVolume(masterVol);
        SetMusicVolume(musicVol);
        SetSFXVolume(sfxVol);

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value / 100f; // Scale đúng
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        float scaledVolume = value / 100f;
        AudioManager.Instance.SetMusicVolume(scaledVolume);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        float scaledVolume = value / 100f;
        AudioManager.Instance.SetSFXVolume(scaledVolume);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

}
