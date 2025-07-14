using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHP : MonoBehaviour
{
    public float maxHP = 100; // Maximum health points
    public float currentHP; // Current health points
    public float InvincibilityTime = 0.17f; // Invincibility time in seconds after taking damage
    public GameObject damagePopupPrefab;
    public Image healthBar; // Reference to the health bar UI element
    public TMP_Text maxHeal; // Reference to the maximum health text UI element
    public TMP_Text currentHeal; // Reference to the current health text UI element

    private bool isInvincible = false; // Invincibility state

    void Start()
    {
        currentHP = maxHP; // Initialize current health to maximum health
        if (healthBar != null)
        {
            healthBar.fillAmount = 1f; // Set health bar to full at the start
        }
        if (maxHeal != null)
        {
            maxHeal.text = maxHP.ToString(); // Set the maximum health text
        }
        if (currentHeal != null)
        {
            currentHeal.text = currentHP.ToString(); // Set the current health text
        }
    }
    // Method to take damage
    public void TakeDamage(int damage , bool isCritical)
    {
        if (currentHP > 0)
        {
            if (!isInvincible)
            {
                currentHP -= damage;
                if (currentHP < 0)
                {
                    currentHP = 0;
                }
                if (healthBar != null)
                {
                    healthBar.fillAmount = currentHP / maxHP; // Update health bar
                }
                if (currentHeal != null)
                {
                    currentHeal.text = currentHP.ToString(); // Update current health text
                }
                // Display damage popup
                if (damagePopupPrefab != null)
                {
                    GameObject damagePopup = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
                    FloatingDamage floatingDamage = damagePopup.GetComponent<FloatingDamage>();
                    if (floatingDamage != null)
                    {
                        Color damageColor = isCritical ? new Color(1f, 0.5f, 0f) : Color.yellow; // Cam hoặc vàng
                        floatingDamage.SetDamageValue(damage, damageColor);
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
            if (healthBar != null)
            {
                healthBar.fillAmount = currentHP / maxHP; // Update health bar
            }
            if (currentHeal != null)
            {
                currentHeal.text = currentHP.ToString(); // Update current health text
            }
            if (currentHP > maxHP)
            {
                currentHP = maxHP;
            }
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
