using UnityEngine;

public class PlayUseSound : MonoBehaviour
{
    public AudioSource AudioSource;

    public void PlayUse()
    {
        if (AudioSource != null)
        {
            AudioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource is not assigned in PlayUseSound script.");
        }
    }
}
