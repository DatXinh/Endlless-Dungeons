using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHP = 50f;
    private float currentHP;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(DamageInfo damage)
    {
        float amount = damage.GetTotalDamage();
        currentHP -= amount;
        Debug.Log($"{gameObject.name} bị đánh {amount} dame. HP còn: {currentHP}");

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
