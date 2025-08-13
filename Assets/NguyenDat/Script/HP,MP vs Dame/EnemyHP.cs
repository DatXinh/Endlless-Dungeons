using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHP : MonoBehaviour
{
    public int BaseHP;
    public float maxHP; // Maximum health points
    public float currentHP; // Current health points
    public float InvincibilityTime = 0.17f; // Invincibility time in seconds after taking damage
    public int damageReductionRate = 0; // Tỉ lệ miễn thương (0-100)
    public GameObject damagePopupPrefab;
    public Image healthBar; // Reference to the health bar UI element
    public TMP_Text maxHeal; // Reference to the maximum health text UI element
    public TMP_Text currentHeal; // Reference to the current health text UI element

    private bool isInvincible = false; // Invincibility state
    public BossPlayDeadAnimation bossPlayDeadAnimation;

    void Start()
    {
        if (LoopManager.Instance != null)
        {
            if (LoopManager.Instance.currentLoop > 0)
            {
                maxHP = BaseHP * ( 1 + (LoopManager.Instance.currentLoop * 0.2f)); // Increase max HP based on current loop
            }
            else
            {
                maxHP = BaseHP; // Use base HP if no LoopManager or current loop is 0
            }
        }else
        {
            maxHP = BaseHP; // Use base HP if no LoopManager
        }
        currentHP = maxHP; // Initialize current health to maximum health
        UpdateHPUI(); // Update health UI at the start
        if (bossPlayDeadAnimation == null)
        {
            bossPlayDeadAnimation = GetComponent<BossPlayDeadAnimation>();
        }
    }
    // Method to take damage
    public void TakeDamage(int damage, bool isCritical)
    {
        if (currentHP > 0)
        {
            if (!isInvincible)
            {
                // Giảm sát thương theo tỉ lệ miễn thương
                int reduction = Mathf.Clamp(damageReductionRate, 0, 100);
                float reducedDamage = damage * (1f - reduction / 100f);
                int finalDamage = Mathf.RoundToInt(reducedDamage);

                currentHP -= finalDamage;
                UpdateHPUI(); // Update health UI
                if (currentHP <= 0)
                {
                    currentHP = 0; // Ensure health doesn't go below zero
                    if (bossPlayDeadAnimation != null)
                    {
                        bossPlayDeadAnimation.DisableScripts();
                    }
                }
                // Display damage popup
                if (damagePopupPrefab != null)
                {
                    // Tạo vị trí ngẫu nhiên gần Enemy (bán kính 0.5 đơn vị)
                    Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * 1f;
                    randomOffset.z = 0; // Đảm bảo popup nằm trên mặt phẳng 2D
                    Vector3 spawnPosition = transform.position + randomOffset;

                    GameObject damagePopup = Instantiate(damagePopupPrefab, spawnPosition, Quaternion.identity);
                    FloatingDamage floatingDamage = damagePopup.GetComponent<FloatingDamage>();
                    if (floatingDamage != null)
                    {
                        Color damageColor = isCritical ? new Color(1f, 0.5f, 0f) : Color.yellow; // Cam hoặc vàng
                        floatingDamage.SetDamageValue(finalDamage, damageColor);
                    }
                }
                StartCoroutine(InvincibilityCoroutine());
            }
        }
    }
    // Method to heal
    public void Heal(int amount)
    {
        if (currentHP > 0)
        {
            currentHP += amount;
            UpdateHPUI(); // Update health UI
        }
    }
    public void UpdateHPUI()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHP / maxHP; // Update health bar
        }
        if (maxHeal != null)
        {
            maxHeal.text = maxHP.ToString(); // Update maximum health text
        }
        if (currentHeal != null)
        {
            currentHeal.text = currentHP.ToString(); // Update current health text
        }
    }
    public float GetHealthPercent()
    {
        return (currentHP / maxHP) * 100f;
    }
    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(InvincibilityTime);
        isInvincible = false;
    }
}
