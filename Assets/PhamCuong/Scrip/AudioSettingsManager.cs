using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // Lấy giá trị đã lưu hoặc mặc định là 100 (thang 0–100)
        float masterVol = PlayerPrefs.GetFloat("MasterVolume", 100f);
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 100f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 100f);

        // Cập nhật slider và áp dụng
        masterSlider.value = masterVol;
        musicSlider.value = musicVol;
        sfxSlider.value = sfxVol;

        SetMasterVolume(masterVol);
        SetMusicVolume(musicVol);
        SetSFXVolume(sfxVol);

        // Lắng nghe thay đổi
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value / 100f;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        float scaled = value / 100f;
        AudioManager.Instance.SetMusicVolume(scaled);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        float scaled = value / 100f;
        AudioManager.Instance.SetSFXVolume(scaled);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
}
