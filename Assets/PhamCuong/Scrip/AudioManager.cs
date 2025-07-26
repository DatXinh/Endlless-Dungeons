using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource; // có thể là 1 hoặc chỉ dùng để test phát

    [Header("Audio Mixer")]
    public AudioMixer audioMixer; // Kéo MainMixer vào đây trong Inspector
    public AudioSource [] AudioSource;
    private void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // giữ lại giữa các scene
        }
        else
        {
            Destroy(gameObject);
        }

        AudioSource = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
    }

    /// <summary>
    /// Set volume for music (expects volume from 0.0 to 1.0)
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("MusicVolume", dB);
    }

    /// <summary>
    /// Set volume for SFX/VFX (expects volume from 0.0 to 1.0)
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("SFXVolume", dB);
    }

    /// <summary>
    /// Play a one-shot SFX clip
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
