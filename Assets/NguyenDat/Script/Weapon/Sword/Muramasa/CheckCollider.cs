using UnityEngine;

public class CheckCollider : MonoBehaviour
{
    private WeaponSoundController soundController;

    private void Awake()
    {
        // Ensure the WeaponSoundController is attached to the same GameObject
        soundController = GetComponent<WeaponSoundController>();
    }
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
