using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public int maxHP = 100; // Maximum health points
    public int currentHP; // Current health points
    public int InvincibilityTime = 1; // Time in seconds for invincibility after taking damage
    public GameObject damagePopupPrefab; // Prefab for the damage popup
    public Image healthBar; // Reference to the health bar UI element
    public TMP_Text maxHeal; // Reference to the health text UI element
    public TMP_Text currentHeal; // Reference to the current health text UI element

    private bool isInvincible = false; // Trạng thái bất tử

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
        UpdateHealthUI(); // Update the health UI at the start
    }
    // Method to take damage
    public void TakeDamage(int damage)
    {
        if (currentHP > 0) // Check if the player is alive
        {
            if (!isInvincible)
            {
                currentHP -= damage; // Reduce current health by damage amount
                UpdateHealthUI(); // Update the health UI
                if (currentHP < 0) // Ensure current health does not go below zero
                {
                    currentHP = 0;
                }
                // Display damage popup
                if (damagePopupPrefab != null)
                {
                    GameObject damagePopup = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
                    FloatingDamage floatingDamage = damagePopup.GetComponent<FloatingDamage>();
                    if (floatingDamage != null)
                    {
                        floatingDamage.SetDamageValue(damage, Color.red); // Set the damage value and color
                    }
                }
                StartCoroutine(InvincibilityCoroutine());
            }
        }
    }

    // Coroutine để xử lý bất tử
    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(InvincibilityTime);
        isInvincible = false;
    }

    // Hàm hồi máu
    public void Heal(int amount)
    {
        if (currentHP > 0)
        {
            currentHP += amount;
            if (currentHP > maxHP)
            {
                currentHP = maxHP;
            }
            UpdateHealthUI();
            // Display damage popup
            if (damagePopupPrefab != null)
            {
                GameObject damagePopup = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
                FloatingDamage floatingDamage = damagePopup.GetComponent<FloatingDamage>();
                if (floatingDamage != null)
                {
                    floatingDamage.SetDamageValue(amount, Color.green);
                }
            }
        }
    }
    private void UpdateHealthUI()
    {
        float healthPercent = (float)currentHP / maxHP;

        if (healthBar != null)
        {
            healthBar.fillAmount = healthPercent;

            Color healthColor;

            if (healthPercent > 0.8f)
            {
                healthColor = new Color(0f, 1f, 0f); // Xanh lá
            }
            else if (healthPercent > 0.6f)
            {
                healthColor = new Color(0.5f, 1f, 0f); // Vàng xanh
            }
            else if (healthPercent > 0.4f)
            {
                healthColor = new Color(1f, 1f, 0f); // Vàng
            }
            else if (healthPercent > 0.2f)
            {
                healthColor = new Color(1f, 0.5f, 0f); // Cam
            }
            else
            {
                healthColor = new Color(1f, 0f, 0f); // Đỏ
            }

            healthBar.color = healthColor;
        }

        if (currentHeal != null)
        {
            currentHeal.text = currentHP.ToString();
        }
    }

}
