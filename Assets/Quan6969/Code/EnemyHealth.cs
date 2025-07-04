using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHP = 50f;
    public float currentHP;
    public bool isInvulnerable = false;

    private SpawnAtDestroy spawnAtDestroy;

    void Start()
    {
        currentHP = maxHP;
        spawnAtDestroy = GetComponent<SpawnAtDestroy>();
        if (spawnAtDestroy == null)
        {
            return;
        }
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
        spawnAtDestroy?.TriggerDestroy();
        Destroy(gameObject); 
    }
    public float GetHealthPercent()
    {
        return (currentHP / maxHP) * 100f;
    }
}
