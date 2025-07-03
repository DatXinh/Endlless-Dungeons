using UnityEngine;

public class SoundPlay: MonoBehaviour
{
    public WeaponSoundController soundController;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            if (soundController != null)
            {
                soundController.PlayHitEnemySound();
            }
        }
    }
}
