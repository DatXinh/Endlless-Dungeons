using System;
using TMPro;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public int maxHP = 100; // Maximum health points
    public int currentHP; // Current health points
    public float InvincibilityTime = 0.1f; // Invincibility time in seconds after taking damage
    public GameObject damagePopupPrefab;

    private bool isInvincible = false; // Invincibility state

    void Start()
    {
        currentHP = maxHP; // Initialize current health to maximum health
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

    // Coroutine to handle invincibility
    private System.Collections.IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(InvincibilityTime);
        isInvincible = false;
    }

    // Method to heal
    public void Heal(int amount)
    {
        if (currentHP > 0)
        {
            currentHP += amount;
            if (currentHP > maxHP)
            {
                currentHP = maxHP;
            }
        }
    }
}
