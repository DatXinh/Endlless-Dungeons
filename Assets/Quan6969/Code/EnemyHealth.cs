using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHP = 50f;
    private float currentHP;
    public bool isInvulnerable = false;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(DamageInfo damage)
    {
        float amount = damage.GetTotalDamage();
        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        Debug.Log($"{gameObject.name} bị nhận {amount} dame. HP còn lại: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} đã chết.");
        Destroy(gameObject); 
    }
}
