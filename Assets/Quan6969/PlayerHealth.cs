using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHP = 100f;
    private float currentHP;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(DamageInfo damage)
    {
        float amount = damage.GetTotalDamage();
        currentHP -= amount;
        Debug.Log($"Người chơi nhận {amount} dame. HP còn lại: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Người chơi đã chết.");
        // Add logic chết ở đây
    }
}
