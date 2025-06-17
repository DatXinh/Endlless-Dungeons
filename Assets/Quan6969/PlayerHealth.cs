
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHP = 100f;
    private float currentHP;

    public Image healthBarFill; // Kéo Image vào đây

    void Start()
    {
        currentHP = maxHP;
        UpdateHealthBar();
    }

    public void TakeDamage(DamageInfo damage)
    {
        float amount = damage.GetTotalDamage();
        currentHP -= amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        Debug.Log($"Player mất {amount} máu. HP còn: {currentHP}");
        UpdateHealthBar();

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, maxHP);

        Debug.Log($"Player hồi {amount} máu. HP: {currentHP}");
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHP / maxHP;
        }
    }

    void Die()
    {
        Debug.Log("Player đã chết.");
    }
}

