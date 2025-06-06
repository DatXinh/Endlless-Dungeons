using UnityEngine;

public class WeaponSoundController : MonoBehaviour
{
    public AudioSource useSound;
    public AudioSource thirdSlashSound;
    public AudioSource hitEnemySound;

    public void PlayUseSound()
    {
        if (useSound != null)
        {
            useSound.Play();
        }
        else
        {
            Debug.LogWarning("Use sound not assigned in WeaponSoundController.");
        }
    }
    public void PlayThirdSlashSound()
    {
        if (thirdSlashSound != null)
        {
            thirdSlashSound.Play();
        }
        else
        {
            Debug.LogWarning("Third slash sound not assigned in WeaponSoundController.");
        }
    }
    public void PlayHitEnemySound()
    {
        if (hitEnemySound != null)
        {
            hitEnemySound.Play();
        }
        else
        {
            Debug.LogWarning("Hit enemy sound not assigned in WeaponSoundController.");
        }
    }
}
