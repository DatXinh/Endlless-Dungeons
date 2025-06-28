using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHP = 100f;
    private float currentHP;

    [Header("Thanh máu")]
    public Image healthBarFill; // Kéo Image vào đây

    [Header("Hiển thị damage popup")]
    public GameObject damagePopupPrefab; // Prefab DamagePopup
    public Transform damagePopupCanvas;  // Canvas (World Space) gắn trên đầu Player

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
        ShowDamagePopup(amount, damage.isCritical); // 👈 Gọi popup ở đây

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
        // Bạn có thể gọi ShowHealPopup() nếu cần hiệu ứng hồi máu
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHP / maxHP;
        }
    }

    void ShowDamagePopup(float amount, bool isCrit)
    {
        if (damagePopupPrefab != null && damagePopupCanvas != null)
        {
            GameObject popup = Instantiate(damagePopupPrefab, damagePopupCanvas.position, Quaternion.identity, damagePopupCanvas);
            popup.GetComponent<DamagePopup>()?.SetDamage(amount, isCrit);
        }
    }

    void Die()
    {
        Debug.Log("Player đã chết.");
        // Gọi GameOver, animation chết,... tại đây nếu cần
    }
}
